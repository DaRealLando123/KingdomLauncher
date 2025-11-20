const fs = require('fs');
const path = require('path');
const https = require('https');
const child_process = require('child_process');
const { ipcRenderer } = require('electron');

const sevenBin = require('7zip-bin');
const { extractFull } = require('node-7z');

// --- CONFIGURATION ---
const TOOLKIT_URL = "https://github.com/DaRealLando123/DaysFM/releases/download/Tools/KH2FM.Toolkit.exe"; 
const ENGLISH_PATCH_URL = "https://github.com/DaRealLando123/DaysFM/releases/download/Tools/English.Patch.kh2patch";

// REPLACE THIS with a direct download link to a portable PCSX2 .7z or .zip
const PCSX2_URL = "https://github.com/DaRealLando123/DaysFM/releases/download/Tools/PCSX2.1.6.0.7z"; 

const playButton = document.getElementById('playButton');
const installButton = document.getElementById('installButton');
const status = document.getElementById('status');
const versionContainer = document.getElementById('versionContainer');
const versionSelect = document.getElementById('versionSelect');

const gameDir = path.join(__dirname, 'downloads');

// --- 1. GAME CHECKER ---
function checkGame() {
    const version = versionSelect.value;
    const gamePath = path.join(gameDir, version, 'KH2FM.iso'); 
    const emuPath = path.join(gameDir, version, 'PCSX2', 'pcsx2.exe');

    let isInstalled = false;
    try {
        // Check for BOTH the ISO and the Emulator EXE
        if (fs.existsSync(gamePath) && fs.statSync(gamePath).size > 0 && fs.existsSync(emuPath)) {
            isInstalled = true;
        }
    } catch (err) { }

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

// --- 2. PLAY LOGIC (LAUNCH PCSX2) ---
playButton.addEventListener('click', () => {
    const version = versionSelect.value;
    
    // Paths
    const isoPath = path.join(gameDir, version, 'KH2FM.iso');
    const emuDir = path.join(gameDir, version, 'PCSX2');
    const emuExe = path.join(emuDir, 'pcsx2.exe'); // Ensure your zip contains this specific filename

    console.log("Launching Emulator:", emuExe);
    console.log("Loading ISO:", isoPath);
    
    status.textContent = "Sending run command- Have fun!";

    // Launch Args based on PCSX2 documentation
    // --fullscreen : Starts in fullscreen
    // --nogui : Hides the PCSX2 configuration window (seamless experience)
    const args = [
        isoPath, 
        '--fullscreen', 
        '--nogui'
    ];

    const gameProcess = child_process.spawn(emuExe, args, {
        cwd: emuDir // Important: Run inside the emulator folder so it finds plugins/bios
    });

    gameProcess.on('error', (err) => {
        console.error(err);
        status.textContent = `Launch Error: ${err.message}`;
    });
});

// --- 3. INSTALL LOGIC ---
installButton.addEventListener('click', async () => {
    status.textContent = "Preparing to install...";
    playButton.style.display = 'none';
    installButton.style.display = 'none';
    versionSelect.style.display = 'none';
    
    const version = versionSelect.value;
    
    // Define Paths
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
        const userReady = await ipcRenderer.invoke('confirm-dialog', 
            "DaysFM requires a clean KH2FM ISO. Do you have one ready to select?"
        );
        if (!userReady) { resetUI("Failed to locate ISO."); return; }
        status.textContent = "Waiting for ISO...";
        const userSelectedPath = await ipcRenderer.invoke('select-iso');
        if (!userSelectedPath) { resetUI("Failed to locate ISO."); return; }

        status.textContent = "Preparing... (This may freeze for a minute.)";
        await new Promise(r => setTimeout(r, 100)); 
        try {
            await fs.promises.copyFile(userSelectedPath, workIsoPath);
        } catch (err) {
            resetUI(`Error copying ISO: ${err.message}`);
            return;
        }
    }

    // B. DOWNLOAD MAIN PATCH
    if (version === "0.034") archiveUrl = "https://github.com/DaRealLando123/DaysFM/releases/download/Alpha/v0.034.Alpha.7z";

    const zipPath = path.join(workDir, 'patch_archive.zip');

    status.textContent = `Starting DaysFM download...`;

    downloadFile(archiveUrl, zipPath, (percent) => {
        status.textContent = `Downloading DaysFM v${version}: ${percent}%`;
    })
    .then(async () => {
        status.textContent = `Starting DaysFM extraction...`;
        await extractZip(zipPath, workDir, (percent) => {
            status.textContent = `Extracting DaysFM v${version}: ${percent}%`;
        });
    })
    .then(async () => {
        // C. DOWNLOAD ENGLISH PATCH
        status.textContent = `Starting Patch download...`;

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
        // F. DOWNLOAD PCSX2 (NEW STEP)

        status.textContent = `Starting download...`;

        const pcsx2Archive = path.join(finalDir, "pcsx2_install.7z");
        const pcsx2Folder = path.join(finalDir, "PCSX2"); // Destination folder

        status.textContent = "Downloading PCSX2 Emulator...";

        await downloadFile(PCSX2_URL, pcsx2Archive, (percent) => {
            status.textContent = `Downloading PCSX2: ${percent}%`;
        });

        status.textContent = "Installing Emulator...";
        // Extract to a 'PCSX2' folder inside the version folder
        await extractZip(pcsx2Archive, pcsx2Folder);
        
        // Cleanup the archive
        try { fs.unlinkSync(pcsx2Archive); } catch(e) {}
    })
    .then(async () => {
        // G. FINALIZE ISO
        status.textContent = `Finalizing...`;
        const newIsoResult = path.join(workDir, 'KH2FM.NEW.ISO');
        
        if (fs.existsSync(newIsoResult)) {
            
            await fs.promises.rename(newIsoResult, finalIsoPath);
            
            status.textContent = "Deleting temporary files...";
            try {
                fs.rmSync(workDir, { recursive: true, force: true });
            } catch (e) { console.warn("Temp folder warning:", e); }

        } else {
            throw new Error("Patcher finished, but KH2FM.NEW.ISO was not found.");
        }

        checkGame();
        versionSelect.style.display = 'block';
    })
    .catch((err) => {
        resetUI(`Error: ${err.message}`);
    });
});

// --- HELPERS ---

function resetUI(msg) {
    console.error(msg);
    status.textContent = msg;
    installButton.style.display = 'block';
    versionSelect.style.display = 'block';
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
                return reject(new Error(`HTTP ${res.statusCode} at ${url}`));
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
        // 7-Zip handles standard zips, 7z, rar, everything.
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

checkGame();