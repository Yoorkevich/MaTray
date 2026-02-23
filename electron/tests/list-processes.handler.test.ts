import { describe, it, expect } from "vitest";
import { listProcesses, type ExecProvider } from "../src/features/list-processes/list-processes.handler";

function mockExec(output: string): ExecProvider {
  return { exec: () => output };
}

const CSV_HEADER = '"ProcessId","Name","ExecutablePath"\r\n';

describe("listProcesses", () => {
  it("parses PowerShell CSV output into AppProcessInfo list", () => {
    const csv =
      CSV_HEADER +
      '"1234","chrome.exe","C:\\Program Files\\Google\\Chrome\\chrome.exe"\r\n' +
      '"5678","Discord.exe","C:\\Program Files\\Discord\\Discord.exe"\r\n';

    const result = listProcesses(mockExec(csv));

    expect(result).toHaveLength(2);
    expect(result[0].name).toBe("chrome.exe");
    expect(result[0].pids).toEqual([1234]);
    expect(result[1].name).toBe("Discord.exe");
  });

  it("groups multiple instances by exe path", () => {
    const csv =
      CSV_HEADER +
      '"100","chrome.exe","C:\\Program Files\\Google\\Chrome\\chrome.exe"\r\n' +
      '"200","chrome.exe","C:\\Program Files\\Google\\Chrome\\chrome.exe"\r\n' +
      '"300","chrome.exe","C:\\Program Files\\Google\\Chrome\\chrome.exe"\r\n';

    const result = listProcesses(mockExec(csv));

    expect(result).toHaveLength(1);
    expect(result[0].pids).toEqual([100, 200, 300]);
    expect(result[0].instanceCount).toBe(3);
  });

  it("filters out C:\\WINDOWS processes", () => {
    const csv =
      CSV_HEADER +
      '"999","svchost.exe","C:\\WINDOWS\\System32\\svchost.exe"\r\n' +
      '"1000","app.exe","C:\\Program Files\\App\\app.exe"\r\n';

    const result = listProcesses(mockExec(csv));

    expect(result).toHaveLength(1);
    expect(result[0].name).toBe("app.exe");
  });

  it("returns empty list when no user processes", () => {
    const csv =
      CSV_HEADER +
      '"999","svchost.exe","C:\\WINDOWS\\System32\\svchost.exe"\r\n';

    const result = listProcesses(mockExec(csv));

    expect(result).toHaveLength(0);
  });

  it("skips lines with missing exe path", () => {
    const csv =
      CSV_HEADER +
      '"0","idle",""\r\n' +
      '"1000","app.exe","C:\\Program Files\\App\\app.exe"\r\n';

    const result = listProcesses(mockExec(csv));

    expect(result).toHaveLength(1);
  });

  it("sorts results alphabetically by name", () => {
    const csv =
      CSV_HEADER +
      '"1","Zebra.exe","C:\\Zebra\\z.exe"\r\n' +
      '"2","Alpha.exe","C:\\Alpha\\a.exe"\r\n' +
      '"3","Mid.exe","C:\\Mid\\m.exe"\r\n';

    const result = listProcesses(mockExec(csv));

    expect(result.map((p) => p.name)).toEqual(["Alpha.exe", "Mid.exe", "Zebra.exe"]);
  });
});
