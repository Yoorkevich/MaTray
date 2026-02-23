import type { AppProcessInfo } from "../features/list-processes/types";
import type { KillProcessResult } from "../features/kill-process/types";

export interface ElectronAPI {
  listProcesses(): Promise<AppProcessInfo[]>;
  killProcess(pids: number[], processName: string): Promise<KillProcessResult>;
}

declare global {
  interface Window {
    api: ElectronAPI;
  }
}
