import { execSync } from "child_process";
import type { AppProcessInfo } from "./types";

export interface ExecProvider {
  exec(command: string): string;
}

const defaultExec: ExecProvider = {
  exec: (cmd) => execSync(cmd, { encoding: "utf-8", maxBuffer: 10 * 1024 * 1024 }),
};

export function listProcesses(exec: ExecProvider = defaultExec): AppProcessInfo[] {
  const raw = exec.exec(
    'powershell -NoProfile -Command "Get-CimInstance Win32_Process | Select-Object ProcessId,Name,ExecutablePath | ConvertTo-Csv -NoTypeInformation"'
  );

  const grouped = new Map<string, { name: string; exePath: string; pids: number[] }>();

  for (const line of raw.split("\n")) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith('"ProcessId"')) continue;

    // CSV format: "ProcessId","Name","ExecutablePath"
    const parts = trimmed.match(/"([^"]*)"/g)?.map((s) => s.slice(1, -1)) ?? [];
    if (parts.length < 3) continue;

    const pid = parseInt(parts[0] ?? "", 10);
    const name = parts[1] ?? "";
    const exePath = parts[2] ?? "";

    if (!exePath || isNaN(pid)) continue;
    if (exePath.toLowerCase().startsWith("c:\\windows\\")) continue;

    const key = exePath.toLowerCase();
    const entry = grouped.get(key);
    if (entry) {
      entry.pids.push(pid);
    } else {
      grouped.set(key, { name, exePath, pids: [pid] });
    }
  }

  return Array.from(grouped.values())
    .map(({ name, exePath, pids }) => ({
      name,
      exePath,
      pids,
      instanceCount: pids.length,
    }))
    .sort((a, b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" }));
}
