import type { AppProcessInfo } from "../../features/list-processes/types";

interface Props {
  process: AppProcessInfo;
  onKill: (info: AppProcessInfo) => void;
}

export function ProcessRow({ process: info, onKill }: Props) {
  return (
    <div className="flex items-center gap-3 px-3 py-2 rounded-lg bg-mantle hover:bg-surface0 transition-colors">
      {/* Name */}
      <div className="flex-1 min-w-0">
        <span className="text-sm text-text truncate block">{info.name}</span>
      </div>

      {/* Instance count badge */}
      {info.instanceCount > 1 && (
        <span className="text-xs text-subtext bg-surface0 px-2 py-0.5 rounded-full">
          ×{info.instanceCount}
        </span>
      )}

      {/* Kill button */}
      <button
        onClick={() => onKill(info)}
        className="w-7 h-7 flex items-center justify-center rounded-md text-overlay hover:bg-red hover:text-white transition-colors cursor-pointer"
        title="Kill process"
      >
        ✕
      </button>
    </div>
  );
}
