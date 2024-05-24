namespace Aff.YB.DataSync.Service.Infrastructure.SpParams
{
    public class SiteNameInfo : SettingInfo
    {
        public SiteNameInfo(string prefixProvider) : base(prefixProvider)
        {
        }

        public override string SName => "BrandName";
    }
}