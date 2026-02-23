import { contextBridge, ipcRenderer } from "electron";

contextBridge.exposeInMainWorld("api", {
  listProcesses: () => ipcRenderer.invoke("list-processes"),
  killProcess: (pids: number[], processName: string) =>
    ipcRenderer.invoke("kill-process", { pids, processName }),
});
