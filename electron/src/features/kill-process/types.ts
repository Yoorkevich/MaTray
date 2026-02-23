export interface KillProcessRequest {
  pids: number[];
  processName: string;
}

export interface KillProcessResult {
  success: boolean;
  message: string;
}
