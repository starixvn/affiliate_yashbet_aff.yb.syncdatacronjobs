namespace Aff.YB.DataSync.Service.Core.Domain.DTOs
{
    public class DataTrackerItem
    {
        public string? PrefixSiteName { get; set; }

        public int PushTrackerId { get; set; }

        public int StatusId { get; set; }

        public int PushDataType { get; set; }

        public string? PushDataInfo { get; set; }
    }
}
