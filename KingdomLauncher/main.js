// main.js
const { app, BrowserWindow, ipcMain, dialog } = require('electron');

let mainWindow;
let tosWindow;

function createMainWindow() {
  mainWindow = new BrowserWindow({
    width: 900,
    height: 600,
    minWidth: 500,
    minHeight: 300,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
    },
    autoHideMenuBar: true,
    show: false, // show only after TOS is accepted
  });

  mainWindow.loadFile("index.html");

  mainWindow.on("closed", () => {
    mainWindow = null;
  });
}

function createTOSWindow() {
  tosWindow = new BrowserWindow({
    width: 500,
    height: 580,
    frame: false, // clean look
    resizable: false,
    alwaysOnTop: true,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
    },
  });

  tosWindow.loadFile("tos.html");

  tosWindow.on("closed", () => {
    tosWindow = null;
  });
}

app.on("ready", () => {
  createTOSWindow(); // Show TOS first
});

ipcMain.on("open-tos", () => {
  mainWindow.close();
  createTOSWindow();
});

// Handle Accept / Decline from TOS
ipcMain.on("tos-accepted", () => {
  if (tosWindow) tosWindow.close();
  createMainWindow();
  mainWindow.show();
});

ipcMain.on("tos-declined", () => {
  app.quit();
});

ipcMain.handle('select-iso', async () => {
  const result = await dialog.showOpenDialog({
    title: 'Select your clean Kingdom Hearts 2 Final Mix ISO',
    filters: [
      { name: 'ISO Files', extensions: ['iso'] }
    ],
    properties: ['openFile']
  });

  if (result.canceled) {
    return null;
  } else {
    return result.filePaths[0];
  }
});

ipcMain.handle('confirm-dialog', async (event, message, detail) => {
  const result = await dialog.showMessageBox({
    type: 'question',
    buttons: ['Yes', 'No'], // Index 0 = Yes, Index 1 = No
    defaultId: 0,
    title: 'Confirmation',
    message: message,
    detail: detail
  });

  // Returns true if "Yes" (index 0) was clicked, false otherwise
  return result.response === 0;
});


ipcMain.handle("oauth-discord", async (event) => {
  return new Promise((resolve) => {
    const authWindow = new BrowserWindow({
      width: 600, height: 700,
      webPreferences: { nodeIntegration: false, contextIsolation: true }
    });

    const CLIENT_ID = "1441558099973640304";
    const REDIRECT_URI = "http://localhost/callback";

    const authUrl = `https://discord.com/oauth2/authorize?client_id=${CLIENT_ID}&response_type=code&redirect_uri=${encodeURIComponent(REDIRECT_URI)}&scope=identify%20guilds`;

    authWindow.loadURL(authUrl);

    authWindow.webContents.on("will-navigate", async (event, url) => {
      if (url.startsWith(REDIRECT_URI)) {
        event.preventDefault();
        const urlObj = new URL(url);
        const code = urlObj.searchParams.get("code");

        if (code) {
          try {
            // Ask Cloudflare Worker to swap the code
            const workerResponse = await fetch("https://kingdomlauncher.wikinothow.workers.dev/auth", {
              method: "POST",
              headers: { "Content-Type": "application/json" },
              body: JSON.stringify({ code: code })
            });

            // Parse response
            const text = await workerResponse.text(); // Get raw text first to debug non-JSON errors
            let data;
            try {
                data = JSON.parse(text);
            } catch (e) {
                console.error("Worker returned non-JSON:", text);
                resolve({ error: "Worker returned invalid JSON", details: text });
                return;
            }
            
            console.log("Worker Response:", data);

            if (data.access_token) {
                resolve(data); 
            } else {
                // PASS THE ERROR BACK TO THE UI
                resolve({ error: data.error || "Unknown Worker Error", details: data });
            }
            
          } catch (err) {
            console.error("Worker Auth Error:", err);
            resolve({ error: "Network Error", details: err.message });
          }
        } else {
            resolve({ error: "No Code", details: "User closed window or URL mismatch" });
        }
        authWindow.close();
      }
    });

    authWindow.on("closed", () => resolve({ error: "Window Closed", details: "User closed the login window." }));
  });
});