// main.js
const { app, BrowserWindow, ipcMain, dialog } = require('electron');

let mainWindow;
let tosWindow;

function createMainWindow() {
  mainWindow = new BrowserWindow({
    width: 900,
    height: 600,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
    },
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