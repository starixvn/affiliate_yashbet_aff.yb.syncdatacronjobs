

namespace Aff.YB.DataSync.Service.Core.Domain
{
    public interface IImportTimeTracker
    {
        string DataSource { get; }

        DateTime CalledTime { get; }

        bool IsReset { get; }

        string KeyValue { get; }
    }
}
