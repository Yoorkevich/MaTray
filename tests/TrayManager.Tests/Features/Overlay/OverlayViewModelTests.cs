using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TrayManager.Features.KillProcess;
using TrayManager.Features.ListProcesses;
using TrayManager.Features.Overlay;

namespace TrayManager.Tests.Features.Overlay;

public class OverlayViewModelTests
{
    private readonly IProcessProvider _provider = Substitute.For<IProcessProvider>();
    private readonly IProcessKiller _killer = Substitute.For<IProcessKiller>();
    private readonly OverlayViewModel _sut;

    public OverlayViewModelTests()
    {
        _sut = new OverlayViewModel(
            new ListProcessesHandler(_provider),
            new KillProcessHandler(_killer),
            NullLogger<OverlayViewModel>.Instance);
    }

    [Fact]
    public void Refresh_PopulatesProcesses()
    {
        _provider.GetUserProcesses().Returns(new List<AppProcessInfo>
        {
            new("chrome", @"C:\chrome.exe", [1], 1),
            new("Discord", @"C:\discord.exe", [2], 1),
        });

        _sut.Refresh();

        Assert.Equal(2, _sut.Processes.Count);
        Assert.Equal("chrome", _sut.Processes[0].Name);
    }

    [Fact]
    public void Refresh_ClearsPreviousProcesses()
    {
        _provider.GetUserProcesses().Returns(new List<AppProcessInfo>
        {
            new("chrome", @"C:\chrome.exe", [1], 1),
        });
        _sut.Refresh();

        _provider.GetUserProcesses().Returns(new List<AppProcessInfo>
        {
            new("Discord", @"C:\discord.exe", [2], 1),
        });
        _sut.Refresh();

        Assert.Single(_sut.Processes);
        Assert.Equal("Discord", _sut.Processes[0].Name);
    }

    [Fact]
    public void KillCommand_RemovesProcessFromList()
    {
        var info = new AppProcessInfo("chrome", @"C:\chrome.exe", [1234], 1);
        _provider.GetUserProcesses().Returns(new List<AppProcessInfo> { info });
        _killer.Kill(Arg.Any<KillProcessRequest>())
            .Returns(new KillProcessResult(true, "Killed"));

        _sut.Refresh();
        _sut.KillCommand.Execute(info);

        Assert.Empty(_sut.Processes);
    }

    [Fact]
    public void KillCommand_PassesCorrectPids()
    {
        var info = new AppProcessInfo("chrome", @"C:\chrome.exe", [100, 200], 2);
        _provider.GetUserProcesses().Returns(new List<AppProcessInfo> { info });
        _killer.Kill(Arg.Any<KillProcessRequest>())
            .Returns(new KillProcessResult(true, "Killed"));

        _sut.Refresh();
        _sut.KillCommand.Execute(info);

        _killer.Received(1).Kill(Arg.Is<KillProcessRequest>(r =>
            r.Pids.Length == 2 && r.Pids[0] == 100 && r.Pids[1] == 200));
    }

    [Fact]
    public void KillCommand_DoesNotRemove_OnFailure()
    {
        var info = new AppProcessInfo("chrome", @"C:\chrome.exe", [1234], 1);
        _provider.GetUserProcesses().Returns(new List<AppProcessInfo> { info });
        _killer.Kill(Arg.Any<KillProcessRequest>())
            .Returns(new KillProcessResult(false, "Access denied"));

        _sut.Refresh();
        _sut.KillCommand.Execute(info);

        Assert.Single(_sut.Processes);
    }
}
