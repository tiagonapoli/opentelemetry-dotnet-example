using System.Diagnostics;
using Grpc.Core;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;

namespace app2.Telemetry
{
    public static class TracerSingleton
    {
        private const string AppName = "app2";
        private const string AppVersion = "0.1.0";
        public static readonly ActivitySource Tracer = new ActivitySource(TracerSingleton.AppName);

        public static void ConfigureTracer(IServiceCollection services)
        {
            services.AddOpenTelemetryTracing(
                (builder) =>
                {
                    builder.AddSource(TracerSingleton.AppName);
                    builder.AddAspNetCoreInstrumentation();
                    builder.AddHttpClientInstrumentation();
                    builder.SetResource(Resources.CreateServiceResource(TracerSingleton.AppName,
                        serviceVersion: TracerSingleton.AppVersion));
                    builder.AddConsoleExporter();
                    builder.AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = "ingest.lightstep.com:443";
                        opt.Headers = new Metadata
                        {
                            {
                                "lightstep-access-token",
                                "ZsaaD6QDwXWceDwFE3itVZd2UiXiuFx7QZO1kTH2HQ+Vu6GfpwBn3eRqEM9k/nrS1PeM9KwQCLVfMo5ejnNED1773JIS5Aco8GozpHRD"
                            }
                        };
                        opt.Credentials = new SslCredentials();
                    });
                    
                    builder.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.5)));
                });
        }
    }
}