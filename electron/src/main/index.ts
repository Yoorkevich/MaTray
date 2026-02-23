import { app, BrowserWindow, ipcMain } from "electron";
import path from "path";
import { listProcesses } from "../features/list-processes/list-processes.handler";
import { killProcess } from "../features/kill-process/kill-process.handler";
import log from "electron-log";

log.transports.file.resolvePathFn = () =>
  path.join(app.getPath("userData"), "logs", "traymanager.log");

// Single instance lock
const gotLock = app.requestSingleInstanceLock();
if (!gotLock) {
  log.info("Another instance already running, quitting");
  app.quit();
}

let mainWindow: BrowserWindow | null = null;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 420,
    height: 520,
    frame: false,
    transparent: true,
    alwaysOnTop: true,
    skipTaskbar: true,
    resizable: false,
    show: false,
    webPreferences: {
      preload: path.join(__dirname, "..", "..", "preload", "preload.js"),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  // Center on screen
  mainWindow.center();

  // Load renderer
  const rendererPath = path.join(__dirname, "..", "..", "renderer", "index.html");
  mainWindow.loadFile(rendererPath);

  mainWindow.once("ready-to-show", () => {
    mainWindow?.show();
    log.info("Window shown");
  });

  // Close on blur
  mainWindow.on("blur", () => {
    mainWindow?.close();
  });

  mainWindow.on("closed", () => {
    mainWindow = null;
  });
}

// IPC handlers
ipcMain.handle("list-processes", () => {
  try {
    return listProcesses();
  } catch (err) {
    log.error("list-processes failed:", err);
    return [];
  }
});

ipcMain.handle("kill-process", (_event, request) => {
  try {
    const result = killProcess(request);
    log.info(`Kill: ${request.processName} -> ${result.success}`);
    return result;
  } catch (err) {
    log.error("kill-process failed:", err);
    return { success: false, message: String(err) };
  }
});

app.whenReady().then(() => {
  log.info("TrayManager started");
  createWindow();
});

app.on("window-all-closed", () => {
  app.quit();
});
