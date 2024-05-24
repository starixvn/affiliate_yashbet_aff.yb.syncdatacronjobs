namespace Aff.YB.DataSync.Service.Infrastructure.Configurations
{
    public class TimeTrackerSetting
    {
        public IEnumerable<string> SourcePrefix { get; set; }

        public DateTime DefaultTime { get; set; }
    }
}
