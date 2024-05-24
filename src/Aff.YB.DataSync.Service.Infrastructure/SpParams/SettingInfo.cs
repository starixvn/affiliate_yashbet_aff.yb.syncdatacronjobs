using Aff.YB.DataSync.Service.Core.Domain;

namespace Aff.YB.DataSync.Service.Infrastructure.SpParams
{
    public class SettingInfo : IdenticalSurfixCriteriaBase, ISettingInfo
    {
        public SettingInfo(string prefixProvider) : base(prefixProvider)
        {
        }

        public virtual string SName { get; set; }

        protected override string SuffixSettingKey => "Task_Aff_GetSetting";

        public override bool IsValid() => true;
    }
}
