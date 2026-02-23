import type { AppProcessInfo } from "../../features/list-processes/types";
import { ProcessRow } from "./ProcessRow";

interface Props {
  processes: AppProcessInfo[];
  onKill: (info: AppProcessInfo) => void;
}

export function ProcessList({ processes, onKill }: Props) {
  if (processes.length === 0) {
    return (
      <div className="text-center text-overlay text-sm py-8">
        No background processes found
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-1">
      {processes.map((p) => (
        <ProcessRow key={p.exePath} process={p} onKill={onKill} />
      ))}
    </div>
  );
}
