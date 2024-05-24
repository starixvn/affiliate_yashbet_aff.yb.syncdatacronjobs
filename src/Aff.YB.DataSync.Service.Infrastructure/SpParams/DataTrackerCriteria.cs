using Aff.YB.DataSync.Service.Core.Domain;
using Fanex.Data;

namespace Aff.YB.DataSync.Service.Infrastructure.SpParams
{
    public class DataTrackerCriteria : IdenticalSurfixCriteriaBase, IDataTracker
    {

        public DataTrackerCriteria(string prefixProvider) : base(prefixProvider)
        {
        }

        protected override string SuffixSettingKey => "Task_API_PushDataTracker_Sel";

        public int PushDataType { get; set; }

        public int StatusId { get; set; }

        [SpParam(Ignored = true)]
        public int PushTrackerId => default;

        public override bool IsValid()
            => true;
    }
}
