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
    
    let isInstalled = false;
    try {
        if (fs.existsSync(gamePath) && fs.statSync(gamePath).size > 0) {
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

// --- 2. PLAY LOGIC ---
playButton.addEventListener('click', () => {
    const version = versionSelect.value;
    const gamePath = path.join(gameDir, version, 'KH2FM.iso');
    console.log("Launching:", gamePath);
    status.textContent = "Game Running...";
    // child_process.spawn('pcsx2', [gamePath]); 
});

// --- 3. INSTALL LOGIC ---
installButton.addEventListener('click', async () => {
    status.textContent = "Starting Install...";
    playButton.style.display = 'none';
    installButton.style.display = 'none';
    versionSelect.style.display = 'none';
    
    const version = versionSelect.value;
    
    // Define Paths
    const finalDir = path.join(__dirname, 'downloads', version);
    
    // We do all work inside this temp folder
    const workDir = path.join(finalDir, 'temp_build'); 

    // Ensure directories exist
    if (!fs.existsSync(finalDir)) fs.mkdirSync(finalDir, { recursive: true });
    if (!fs.existsSync(workDir)) fs.mkdirSync(workDir, { recursive: true });
    
    status.textContent = "Looking for a clean KH2FM ROM...";
    // A. ISO MANAGEMENT (Move logic into the temp folder)
    const finalIsoPath = path.join(finalDir, 'KH2FM.iso');
    const workIsoPath = path.join(workDir, 'KH2FM.iso'); // The copy we will patch

    // Strategy: Ensure 'workIsoPath' exists before we start downloading patches.
    
    // 2. Check if we have a main ISO (might be clean, might be patched, we assume clean if no backup exists)
    if (fs.existsSync(finalIsoPath)) {
        console.log("Found ISO. Moving to workspace...");
        fs.renameSync(finalIsoPath, workIsoPath);
    }
    // 3. Ask User
    else {
        status.textContent = "Failed to find existing ISO.";
        const userReady = await ipcRenderer.invoke('confirm-dialog', 
            "DaysFM requires a clean KH2FM ISO. Do you have one ready to select?"
        );
        if (!userReady) { resetUI("Cancelled."); return; }
        
        const userSelectedPath = await ipcRenderer.invoke('select-iso');
        if (!userSelectedPath) { resetUI("Cancelled."); return; }

        status.textContent = "Copying KH2FM to be modded..."
        await new Promise(r => setTimeout(r, 100)); 
        try {
            await fs.promises.copyFile(userSelectedPath, workIsoPath);
        } catch (err) {
            resetUI(`Error copying ISO: ${err.message}`);
            return;
        }
    }

    // B. DOWNLOAD MAIN ARCHIVE
    let archiveUrl = `https://dl.lando.run/KHD/${version}.zip`; 
    if (version === "0.034") archiveUrl = "https://github.com/DaRealLando123/DaysFM/releases/download/Alpha/v0.034.Alpha.7z";

    const zipPath = path.join(workDir, 'patch_archive.zip');

    status.textContent = `Starting DaysFM download...`;

    downloadFile(archiveUrl, zipPath, (percent) => {
        status.textContent = `Downloading DaysFM v${version}: ${percent}%`;
    })
    .then(async () => {
        // C. EXTRACT ARCHIVE (Into Temp Folder)
        await extractZip(zipPath, workDir, (percent) => {
            status.textContent = `Extracting DaysFM v${version}: ${percent}%`;
        });
    })
    .then(async () => {
        // D. DOWNLOAD ENGLISH PATCH (Into Temp Folder)
        const englishPatchDest = path.join(workDir, "English.kh2patch");
        status.textContent = "Starting Patch download...";
        
        await downloadFile(ENGLISH_PATCH_URL, englishPatchDest, (percent) => {
            status.textContent = `Downloading English Patch: ${percent}%`;
        });
    })
    .then(async () => {
        // E. DOWNLOAD TOOLKIT (Into Temp Folder)
        const toolkitDest = path.join(workDir, "KH2FM Toolkit.exe");
        status.textContent = "Starting Patcher download...";
        
        await downloadFile(TOOLKIT_URL, toolkitDest, (percent) => {
            status.textContent = `Downloading Patcher: ${percent}%`;
        });
        return toolkitDest;
    })
    .then((toolkitPath) => {
        // F. RUN TOOLKIT
        status.textContent = `Preparing to modify KH2...`;

        // Scan temp dir for patches
        const allFiles = fs.readdirSync(workDir);
        const patchFiles = allFiles.filter(file => file.endsWith('.kh2patch'));

        if (patchFiles.length === 0) throw new Error("No patches found!");

        return runToolkit(toolkitPath, patchFiles);
    })
    .then(async () => {
        // G. FINALIZE & CLEANUP
        status.textContent = `Cleaning up...`;

        const newIsoResult = path.join(workDir, 'KH2FM.NEW.ISO');
        
        if (fs.existsSync(newIsoResult)) {

            // 2. Move the NEW patched ISO to the root as the MAIN game
            // This overwrites any existing game file there
            await fs.promises.rename(newIsoResult, finalIsoPath);
            
            console.log("Patched ISO moved to final location.");

            // 3. DELETE TEMP FOLDER
            // This deletes the zip, the toolkit, the patch files, everything else.
            status.textContent = "Deleting temporary files...";
            try {
                fs.rmSync(workDir, { recursive: true, force: true });
            } catch (e) {
                console.warn("Could not fully delete temp folder:", e);
            }

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

        // 1. Start Command
        setTimeout(() => {
            if (!hasStarted) {
                console.log("Sending Start Command...");
                try { child.stdin.write('\r\n'); } catch(e) {}
                hasStarted = true;
            }
        }, 2000);

        // 2. Monitor Output
        child.stdout.on('data', (data) => {
            const output = data.toString().toLowerCase();
            console.log(`Toolkit: ${data.toString()}`); 
            status.textContent = `Installing...\n${data.toString()}`

            if (hasStarted && (output.includes("press enter to exit") || output.includes("press return to exit"))) {
                console.log("Completion detected. Exiting...");
                setTimeout(() => {
                    try { child.stdin.write('\r\n'); } catch(e) {}
                }, 500);
            }
        });

        child.on('close', (code) => {
            if (code === 0 || code === 3762504530) {
                console.log(`Toolkit Success (Code: ${code})`);
                resolve();
            } else {
                reject(new Error(`Toolkit Error Code: ${code}`));
            }
        });
        child.on('error', (err) => reject(err));
    });
}

checkGame();