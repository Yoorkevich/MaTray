import { useState, useEffect, useCallback } from "react";
import type { AppProcessInfo } from "../../features/list-processes/types";

export function useProcesses() {
  const [processes, setProcesses] = useState<AppProcessInfo[]>([]);
  const [loading, setLoading] = useState(true);

  const refresh = useCallback(async () => {
    setLoading(true);
    const result = await window.api.listProcesses();
    setProcesses(result);
    setLoading(false);
  }, []);

  const kill = useCallback(async (info: AppProcessInfo) => {
    const result = await window.api.killProcess(info.pids, info.name);
    if (result.success) {
      setProcesses((prev) => prev.filter((p) => p.exePath !== info.exePath));
    }
    return result;
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh]);

  return { processes, loading, refresh, kill };
}
