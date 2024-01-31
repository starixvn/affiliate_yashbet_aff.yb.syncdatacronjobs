namespace Aff.YB.DataSync.Service.Core.Domain
{
    public interface IDataTrackerItem
    {
        int PushTrackerId { get; }

        int StatusId { get; }

        int PushDataType { get; }

        string PushDataInfo { get; }
    }
}
