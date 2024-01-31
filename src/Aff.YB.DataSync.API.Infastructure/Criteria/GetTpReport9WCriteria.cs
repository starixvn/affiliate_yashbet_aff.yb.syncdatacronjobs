using Fanex.Data.Repository;
using System;


namespace Aff.YB.DataSync.API.Infastructure.Criteria
{
    public class GetTpReport9WCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public override string GetSettingKey()
            => "API_TpReport9W_Get";

        public override bool IsValid()
            => true;
    }
}
