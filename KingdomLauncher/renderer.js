const fs = require('fs');
const path = require('path');
const https = require('https');
const child_process = require('child_process');
const { ipcRenderer } = require('electron');
const sevenBin = require('7zip-bin');
const { extractFull } = require('node-7z');

// --- CONFIGURATION ---
const WORKER_BASE_URL = "https://kingdomlauncher.wikinothow.workers.dev";
const TOOLKIT_URL = "https://github.com/DaRealLando123/KingdomLauncher/releases/download/Tools/KH2FM.Toolkit.exe"; 
const ENGLISH_PATCH_URL = "https://github.com/DaRealLando123/KingdomLauncher/releases/download/Tools/English.Patch.kh2patch";
const PCSX2_URL = "https://github.com/DaRealLando123/KingdomLauncher/releases/download/Tools/PCSX2.1.6.0.7z"; 

// --- GLOBAL STATE ---
let globalDiscordToken = null; 
const gameDir = path.join(__dirname, 'downloads');

// --- DOM ELEMENTS ---
const playButton = document.getElementById('playButton');
const installButton = document.getElementById('installButton');
const authButton = document.getElementById('authButton');
const status = document.getElementById('status');
const versionContainer = document.getElementById('versionContainer');
const versionSelect = document.getElementById('versionSelect');
const overlay = document.getElementById('overlay');
const closeButton = document.getElementById('overlayClose');
const discordButton = document.getElementById('discordAuthButton');

// --- INITIAL STATE ---
versionContainer.style.display = 'none';
installButton.style.display = 'none';
playButton.style.display = 'none';
// authButton visibility is handled by tryAutoLogin

// --- 1. GAME CHECKER ---
function checkGame() {
    const version = versionSelect.value;
    // If no version is selected (e.g. before login), do nothing
    if (!version) return;

    const gamePath = path.join(gameDir, version, 'KH2FM.iso'); 
    const emuPath = path.join(gameDir, version, 'PCSX2', 'pcsx2.exe');

    let isInstalled = false;
    try {
        if (fs.existsSync(gamePath) && fs.statSync(gamePath).size > 0 && fs.existsSync(emuPath)) {
            isInstalled = true;
        }
    } catch (err) { }

    // hide the auth button if checking the game (implies logged in)
    authButton.style.display = 'none'; 

    if (isInstalled) {
        status.textContent = `DaysFM installed (v${version})`;
        playButton.style.display = 'block';
        installButton.style.display = 'none';
    } else {
        status.textContent = `DaysFM not installed (v${version})`;
        playButton.style.display = 'none';
        installButton.style.display = 'block';
    }
}
versionSelect.addEventListener('change', checkGame);

// --- 2. PLAY LOGIC ---
playButton.addEventListener('click', () => {
    const version = versionSelect.value;
    const isoPath = path.join(gameDir, version, 'KH2FM.iso');
    const emuDir = path.join(gameDir, version, 'PCSX2');
    const emuExe = path.join(emuDir, 'pcsx2.exe');

    console.log("Launching Emulator:", emuExe);
    
    status.textContent = "Sending run command - Have fun!";

    const args = [ isoPath, '--fullscreen', '--nogui' ];

    const gameProcess = child_process.spawn(emuExe, args, {
        cwd: emuDir 
    });

    gameProcess.on('error', (err) => {
        console.error(err);
        status.textContent = `Launch Error: ${err.message}`;
    });
});

// --- 3. INSTALL LOGIC ---
installButton.addEventListener('click', async () => {
    if (!globalDiscordToken) {
        // Fallback if somehow clicked without token
        authButton.style.display = 'block';
        installButton.style.display = 'none';
        return;
    }

    const version = versionSelect.value;
    
    status.textContent = `Preparing to install v${version}...`;
    playButton.style.display = 'none';
    installButton.style.display = 'none';
    versionContainer.style.display = 'none';
    authButton.style.display = 'none';
    
    const finalDir = path.join(__dirname, 'downloads', version);
    const workDir = path.join(finalDir, 'temp_build'); 

    if (!fs.existsSync(finalDir)) fs.mkdirSync(finalDir, { recursive: true });
    if (!fs.existsSync(workDir)) fs.mkdirSync(workDir, { recursive: true });
    
    // A. ISO MANAGEMENT
    status.textContent = "Locating ISO...";
    const finalIsoPath = path.join(finalDir, 'KH2FM.iso');
    const workIsoPath = path.join(workDir, 'KH2FM.iso'); 

    if (fs.existsSync(finalIsoPath)) {
        console.log("Found ISO. Moving to workspace...");
        fs.renameSync(finalIsoPath, workIsoPath);
    }
    else {
        status.textContent = "Asking user for ISO...";
        const userReady = await ipcRenderer.invoke('confirm-dialog', 
            "Kingdom Launcher requires a clean Kingdom Hearts II - Final Mix+ (Japan) ISO.", "A *legally* obtained ISO is required to continue."
        );
        if (!userReady) { resetUI("Cancelled."); return; }
        
        const userSelectedPath = await ipcRenderer.invoke('select-iso');
        if (!userSelectedPath) { resetUI("Cancelled."); return; }

        status.textContent = "Preparing... (This may freeze for a minute.)";
        await new Promise(r => setTimeout(r, 100)); 
        try {
            await fs.promises.copyFile(userSelectedPath, workIsoPath);
        } catch (err) {
            resetUI(`Error copying ISO: ${err.message}`);
            return;
        }
    }

    // B. DOWNLOAD MAIN PATCH (Secure Worker URL)
    const archiveUrl = `${WORKER_BASE_URL}/download?token=${globalDiscordToken}&version=${version}`;
    const zipPath = path.join(workDir, 'patch_archive.zip');

    status.textContent = `Requesting DaysFM v${version} from server...`;

    downloadFile(archiveUrl, zipPath, (percent) => {
        status.textContent = `Downloading DaysFM v${version}: ${percent}%`;
    })
    .then(async () => {
        status.textContent = `Starting DaysFM v${version} extraction...`;
        await extractZip(zipPath, workDir, (percent) => {
            status.textContent = `Extracting DaysFM v${version}: ${percent}%`;
        });
    })
    .then(async () => {
        // C. DOWNLOAD ENGLISH PATCH
        status.textContent = `Starting Patches download...`;

        const englishPatchDest = path.join(workDir, "English.kh2patch");
        await downloadFile(ENGLISH_PATCH_URL, englishPatchDest, (percent) => {
            status.textContent = `Downloading English Patch: ${percent}%`;
        });
    })
    .then(async () => {
        // D. DOWNLOAD TOOLKIT
        status.textContent = `Starting Toolkit download...`;

        const toolkitDest = path.join(workDir, "KH2FM Toolkit.exe");
        await downloadFile(TOOLKIT_URL, toolkitDest, (percent) => {
            status.textContent = `Downloading Toolkit: ${percent}%`;
        });
        return toolkitDest;
    })
    .then((toolkitPath) => {
        // E. RUN TOOLKIT
        status.textContent = `Applying Patches...`;
        const allFiles = fs.readdirSync(workDir);
        const patchFiles = allFiles.filter(file => file.endsWith('.kh2patch'));
        if (patchFiles.length === 0) throw new Error("No patches found!");
        return runToolkit(toolkitPath, patchFiles);
    })
    .then(async () => {
        // F. DOWNLOAD PCSX2
        const pcsx2Archive = path.join(finalDir, "pcsx2_install.7z");
        const pcsx2Folder = path.join(finalDir, "PCSX2"); 

        // Only download if missing
        if (!fs.existsSync(pcsx2Folder)) {
            status.textContent = "Starting Emulator Download...";

            await downloadFile(PCSX2_URL, pcsx2Archive, (percent) => {
                status.textContent = `Downloading PCSX2: ${percent}%`;
            });
            
            status.textContent = "Installing PCSX2...";
            await extractZip(pcsx2Archive, pcsx2Folder,  (percent) => {
                status.textContent = `Installing PCSX2: ${percent}%`;
            });
            try { fs.unlinkSync(pcsx2Archive); } catch(e) {}
        }
    })
    .then(async () => {
        // G. FINALIZE ISO
        status.textContent = `Finalizing...`;
        const newIsoResult = path.join(workDir, 'KH2FM.NEW.ISO');
        
        if (fs.existsSync(newIsoResult)) {
            await fs.promises.rename(newIsoResult, finalIsoPath);
            status.textContent = "Deleting temporary files...";
            try { fs.rmSync(workDir, { recursive: true, force: true }); } catch (e) { }
        } else {
            throw new Error("Patcher finished, but KH2FM.NEW.ISO was not found.");
        }

        checkGame();
        versionContainer.style.display = 'flex'; // Ensure dropdown is visible again
    })
    .catch((err) => {
        resetUI(`Error: ${err.message}`);
    });
});

// --- 4. AUTH & OVERLAY LOGIC ---

authButton.addEventListener('click', () => {
    overlay.style.display = 'flex';
});

overlay.addEventListener('click', (e) => {
    if (e.target === overlay) overlay.style.display = 'none';
});
if(closeButton) {
    closeButton.addEventListener('click', () => overlay.style.display = 'none');
}

// Discord auth button click
discordButton.addEventListener('click', async () => {
    status.textContent = "Authenticating...";

    const authData = await ipcRenderer.invoke('oauth-discord');

    if (authData && authData.error) {
        // 1. Close the overlay immediately
        overlay.style.display = 'none';
            
        // 2. Check specific error message
        if (authData.error.includes('Not')) {
            status.textContent = "That Discord account does not have access to anything.";
            return
        } else {
            status.textContent = `Login Failed, ${authData.error}`;
            return
        }
            
        status.textContent = "Login Failed.";
        return;
    }

    completeLogin(authData.access_token, authData.versions);
});


// --- HELPERS ---

function resetUI(msg) {
    console.error(msg);
    status.textContent = msg;
    // If logged in, show install button, otherwise show auth button
    if (globalDiscordToken) {
        installButton.style.display = 'block';
        versionContainer.style.display = 'flex';
    } else {
        authButton.style.display = 'block';
    }
}

function downloadFile(url, savePath, onProgress) {
    return new Promise((resolve, reject) => {
        const file = fs.createWriteStream(savePath);
        const req = https.get(url, (res) => {
            if (res.statusCode >= 300 && res.statusCode < 400 && res.headers.location) {
                file.close();
                return downloadFile(res.headers.location, savePath, onProgress).then(resolve).catch(reject);
            }
            if (res.statusCode !== 200) {
                file.close();
                fs.unlink(savePath, ()=>{});
                let data = ''; res.on('data', c => data += c);
                res.on('end', () => reject(new Error(`HTTP ${res.statusCode}: ${data.substring(0, 100)}`)));
                return;
            }
            const total = parseInt(res.headers['content-length'], 10);
            let cur = 0;
            res.on('data', (chunk) => {
                cur += chunk.length;
                if (total && onProgress) onProgress(((cur/total)*100).toFixed(1));
            });
            res.pipe(file);
            file.on('finish', () => file.close(resolve));
        });
        req.on('error', (err) => {
            file.close();
            fs.unlink(savePath, ()=>{});
            reject(err);
        });
    });
}

function extractZip(zipPath, targetDir, onProgress) {
    return new Promise((resolve, reject) => {
        const stream = extractFull(zipPath, targetDir, {
            $bin: sevenBin.path7za,
            $progress: true 
        });
        stream.on('progress', (p) => { if(onProgress) onProgress(p.percent); });
        stream.on('end', () => resolve());
        stream.on('error', (err) => reject(new Error(`7-Zip: ${err.message}`)));
    });
}

function runToolkit(exePath, patchFileList) {
    return new Promise((resolve, reject) => {
        if (!fs.existsSync(exePath)) return resolve();

        console.log(`Spawning: ${exePath}`);
        const child = child_process.spawn(exePath, patchFileList, {
            cwd: path.dirname(exePath) 
        });

        let hasStarted = false;
        setTimeout(() => {
            if (!hasStarted) {
                try { child.stdin.write('\r\n'); } catch(e) {}
                hasStarted = true;
            }
        }, 2000);

        child.stdout.on('data', (data) => {
            const output = data.toString().toLowerCase();
            const cleanMsg = data.toString().replace(/\r?\n|\r/g, " ").substring(0, 60);
            status.textContent = `Installing: ${cleanMsg}...`;
            if (hasStarted && (output.includes("press enter to exit") || output.includes("press return to exit"))) {
                setTimeout(() => {
                    try { child.stdin.write('\r\n'); } catch(e) {}
                }, 500);
            }
        });

        child.on('close', (code) => {
            if (code === 0 || code === 3762504530) resolve();
            else reject(new Error(`Toolkit Error Code: ${code}`));
        });
        child.on('error', (err) => reject(err));
    });
}

async function tryAutoLogin() {
    const savedToken = localStorage.getItem("discord_token");
    
    if (!savedToken) {
        const oneLiners = [`"There’s no way you’re taking Kairi’s heart!"`,`"Is any of this for real... or not?"`,`"It looks like my summer vacation is... over."`,`"My friends are my power!"`,`"Two?!"`]
        status.textContent = oneLiners[Math.floor(Math.random() * oneLiners.length)];
        
        // IMPORTANT: Ensure Auth Button is visible when not logged in
        authButton.style.display = 'block'; 
        return;
    }

    status.textContent = "Verifying Saved Session...";

    try {
        const response = await fetch(`${WORKER_BASE_URL}/versions?token=${savedToken}`);
        
        if (response.ok) {
            const data = await response.json();
            console.log("Auto-login success!");
            completeLogin(savedToken, data.versions);
        } else {
            console.warn("Saved token expired.");
            localStorage.removeItem("discord_token");
            status.textContent = "Session expired. Please login again.";
            authButton.style.display = 'block';
        }
    } catch (e) {
        console.error("Auto-login network error:", e);
        status.textContent = "Connection failed.";
        authButton.style.display = 'block';
    }
}

function completeLogin(token, versions) {
    globalDiscordToken = token;
    localStorage.setItem("discord_token", token);

    if (versions.length === 0) {
        alert("Login success, but no versions found on GitHub.");
        return;
    }

    versionSelect.innerHTML = "";
    versions.forEach(ver => {
        const opt = document.createElement("option");
        opt.value = ver;
        opt.textContent = `358/2 Days Final Mix | Alpha v${ver}`;
        versionSelect.appendChild(opt);
    });
    versionSelect.value = versions[0];

    // FIXED: Show the container, not just the select element
    versionContainer.style.display = 'flex'; 
    
    authButton.style.display = 'none'; 
    overlay.style.display = 'none';
    status.textContent = `${versions.length} versions available.`;
    
    checkGame(); 
}

tryAutoLogin();