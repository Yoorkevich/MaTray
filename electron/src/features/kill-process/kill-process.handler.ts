import { execSync } from "child_process";
import type { KillProcessRequest, KillProcessResult } from "./types";

export interface ExecProvider {
  exec(command: string): string;
}

const defaultExec: ExecProvider = {
  exec: (cmd) => execSync(cmd, { encoding: "utf-8" }),
};

export function killProcess(
  request: KillProcessRequest,
  exec: ExecProvider = defaultExec
): KillProcessResult {
  let killed = 0;
  const errors: string[] = [];

  for (const pid of request.pids) {
    try {
      exec.exec(`taskkill /PID ${pid} /F`);
      killed++;
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : String(err);
      // "not found" means it already exited — count as success
      if (msg.includes("not found")) {
        killed++;
      } else {
        errors.push(`PID ${pid}: ${msg}`);
      }
    }
  }

  if (errors.length === 0) {
    return {
      success: true,
      message: `Killed ${request.processName} (${killed} instance(s))`,
    };
  }

  return {
    success: false,
    message: `Killed ${killed}/${request.pids.length}: ${errors.join("; ")}`,
  };
}
