using NSubstitute;
using TrayManager.Features.KillProcess;

namespace TrayManager.Tests.Features.KillProcess;

public class KillProcessHandlerTests
{
    private readonly IProcessKiller _killer = Substitute.For<IProcessKiller>();
    private readonly KillProcessHandler _sut;

    public KillProcessHandlerTests()
    {
        _sut = new KillProcessHandler(_killer);
    }

    [Fact]
    public void Handle_DelegatesToKiller()
    {
        var request = new KillProcessRequest([1234], "chrome");
        _killer.Kill(request).Returns(new KillProcessResult(true, "Killed chrome"));

        var result = _sut.Handle(request);

        Assert.True(result.Success);
        Assert.Equal("Killed chrome", result.Message);
    }

    [Fact]
    public void Handle_ReturnsFailure_WhenKillerFails()
    {
        var request = new KillProcessRequest([9999], "ghost");
        _killer.Kill(request).Returns(new KillProcessResult(false, "Process not found"));

        var result = _sut.Handle(request);

        Assert.False(result.Success);
    }

    [Fact]
    public void Handle_PassesAllPidsToKiller()
    {
        var request = new KillProcessRequest([100, 200, 300], "chrome");
        _killer.Kill(request).Returns(new KillProcessResult(true, "Killed 3 instances"));

        _sut.Handle(request);

        _killer.Received(1).Kill(Arg.Is<KillProcessRequest>(r => r.Pids.Length == 3));
    }
}
