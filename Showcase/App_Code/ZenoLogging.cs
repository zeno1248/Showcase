using Microsoft.Extensions.Logging;
using System.IO;
using System.Web.Hosting;

namespace Showcase.Util
{
    public static class ZenoLogging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory().AddFile(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Logs/zeno-{Date}.txt"));
        public static ILogger CreateLogger<T>() =>
          LoggerFactory.CreateLogger<T>();
    }
}