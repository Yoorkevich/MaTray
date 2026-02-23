import { useProcesses } from "./hooks/useProcesses";
import { ProcessList } from "./components/ProcessList";
import { useEffect } from "react";

export default function App() {
  const { processes, loading, kill } = useProcesses();

  // Close on Escape
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (e.key === "Escape") window.close();
    };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, []);

  return (
    <div className="bg-base border border-border rounded-xl p-4 mx-2 my-2 shadow-2xl max-h-[500px] flex flex-col">
      {/* Header */}
      <div className="mb-3">
        <h1 className="text-lg font-semibold text-text">TrayManager</h1>
        <p className="text-xs text-overlay">Background processes</p>
      </div>

      {/* Process list */}
      <div className="flex-1 overflow-y-auto">
        {loading ? (
          <div className="text-center text-overlay text-sm py-8">Loading...</div>
        ) : (
          <ProcessList processes={processes} onKill={kill} />
        )}
      </div>
    </div>
  );
}
