const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('electronAPI', {
  acceptTOS: () => ipcRenderer.send('accept-tos'),
  declineTOS: () => ipcRenderer.send('decline-tos'),
  openTOS: () => ipcRenderer.send('open-tos'),
});
