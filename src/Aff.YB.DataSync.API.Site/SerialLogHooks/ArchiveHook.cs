using Serilog.Sinks.File.Archive;
using System.IO.Compression;

namespace Aff.YB.DataSync.API.Site.SerialLogHooks
{
    public class ArchiveHook
    {
        public static ArchiveHooks MyArchiveHooks => new ArchiveHooks(CompressionLevel.Fastest);
    }
}
