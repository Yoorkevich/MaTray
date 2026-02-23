import { describe, it, expect, vi } from "vitest";
import { killProcess, type ExecProvider } from "../src/features/kill-process/kill-process.handler";

describe("killProcess", () => {
  it("kills a single process successfully", () => {
    const exec: ExecProvider = { exec: vi.fn() };

    const result = killProcess({ pids: [1234], processName: "chrome" }, exec);

    expect(result.success).toBe(true);
    expect(result.message).toContain("chrome");
    expect(result.message).toContain("1 instance");
  });

  it("kills multiple PIDs", () => {
    const exec: ExecProvider = { exec: vi.fn() };

    const result = killProcess({ pids: [100, 200, 300], processName: "chrome" }, exec);

    expect(result.success).toBe(true);
    expect(result.message).toContain("3 instance");
    expect(exec.exec).toHaveBeenCalledTimes(3);
  });

  it("counts already-exited processes as success", () => {
    const exec: ExecProvider = {
      exec: vi.fn().mockImplementation(() => {
        throw new Error("not found");
      }),
    };

    const result = killProcess({ pids: [9999], processName: "ghost" }, exec);

    expect(result.success).toBe(true);
  });

  it("returns failure when kill throws non-not-found error", () => {
    const exec: ExecProvider = {
      exec: vi.fn().mockImplementation(() => {
        throw new Error("Access denied");
      }),
    };

    const result = killProcess({ pids: [1234], processName: "system" }, exec);

    expect(result.success).toBe(false);
    expect(result.message).toContain("Access denied");
  });

  it("reports partial success when some PIDs fail", () => {
    let callCount = 0;
    const exec: ExecProvider = {
      exec: vi.fn().mockImplementation(() => {
        callCount++;
        if (callCount === 2) throw new Error("Access denied");
      }),
    };

    const result = killProcess({ pids: [1, 2, 3], processName: "mixed" }, exec);

    expect(result.success).toBe(false);
    expect(result.message).toContain("2/3");
  });

  it("passes correct taskkill command for each PID", () => {
    const exec: ExecProvider = { exec: vi.fn() };

    killProcess({ pids: [42, 99], processName: "app" }, exec);

    expect(exec.exec).toHaveBeenCalledWith("taskkill /PID 42 /F");
    expect(exec.exec).toHaveBeenCalledWith("taskkill /PID 99 /F");
  });
});
