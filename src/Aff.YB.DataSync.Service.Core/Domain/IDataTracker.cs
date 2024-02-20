namespace Aff.YB.DataSync.Service.Core.Domain
{
    public interface IDataTracker
    {
        int PushDataType { get; }

        int StatusId { get; }

        int PushTrackerId { get; }
    }
}
