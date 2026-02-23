using NSubstitute;
using TrayManager.Features.ListProcesses;

namespace TrayManager.Tests.Features.ListProcesses;

public class ListProcessesHandlerTests
{
    private readonly IProcessProvider _provider = Substitute.For<IProcessProvider>();
    private readonly ListProcessesHandler _sut;

    public ListProcessesHandlerTests()
    {
        _sut = new ListProcessesHandler(_provider);
    }

    [Fact]
    public void Handle_ReturnsProcessesFromProvider()
    {
        var expected = new List<AppProcessInfo>
        {
            new("chrome", @"C:\Program Files\Google\Chrome\Application\chrome.exe", [1234, 5678], 2),
            new("Discord", @"C:\Users\max_c\AppData\Local\Discord\app-1.0\Discord.exe", [9999], 1),
        };
        _provider.GetUserProcesses().Returns(expected);

        var result = _sut.Handle();

        Assert.Equal(2, result.Count);
        Assert.Equal("chrome", result[0].Name);
        Assert.Equal(2, result[0].InstanceCount);
    }

    [Fact]
    public void Handle_ReturnsEmptyList_WhenNoProcesses()
    {
        _provider.GetUserProcesses().Returns(new List<AppProcessInfo>());

        var result = _sut.Handle();

        Assert.Empty(result);
    }

    [Fact]
    public void Handle_CallsProviderExactlyOnce()
    {
        _provider.GetUserProcesses().Returns(new List<AppProcessInfo>());

        _sut.Handle();

        _provider.Received(1).GetUserProcesses();
    }
}
