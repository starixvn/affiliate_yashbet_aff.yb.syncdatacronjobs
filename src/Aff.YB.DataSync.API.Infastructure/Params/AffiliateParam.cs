namespace Aff.YB.DataSync.API.Infastructure.Params
{
    public class AffiliateParam
    {
        public int CustId { get; set; }

        public int SiteId { get; set; }

        public string AffiliateName { get; set; }

        public string ParentAffiliate { get; set; } = null;

        public string AffiliateCode { get; set; }

        public string UserName { get; set; }
    }
}
