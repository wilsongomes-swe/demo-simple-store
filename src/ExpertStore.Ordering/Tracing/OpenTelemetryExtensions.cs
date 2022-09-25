using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ExpertStore.Ordering.Tracing;

public static class OpenTelemetryExtensions
{
    static OpenTelemetryExtensions()
    {
        Local = "ExpertStore.Ordering";
        Kernel = Environment.OSVersion.VersionString;
        Framework = RuntimeInformation.FrameworkDescription;
        ServiceName = "ExpertStore.Ordering";
        ServiceVersion = typeof(OpenTelemetryExtensions).Assembly.GetName().Version!.ToString();
    }

    public static ActivitySource CreateActivitySource() => new(ServiceName, ServiceVersion);

    public static string Local { get; }
    public static string Kernel { get; }
    public static string Framework { get; }
    public static string ServiceName { get; }
    public static string ServiceVersion { get; }
}