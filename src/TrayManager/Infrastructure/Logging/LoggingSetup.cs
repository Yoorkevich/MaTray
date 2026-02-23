using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;

namespace TrayManager.Infrastructure.Logging;

public static class LoggingSetup
{
    public static ILoggerFactory Create()
    {
        var logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TrayManager", "logs");

        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                Path.Combine(logDir, "traymanager-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        return LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(serilogLogger, dispose: true);
        });
    }
}
