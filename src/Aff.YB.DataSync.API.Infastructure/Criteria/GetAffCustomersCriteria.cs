using Fanex.Data.Repository;
using System;

namespace Aff.YB.DataSync.API.Infastructure.Criteria
{
    public class GetAffCustomersCriteria : CriteriaBase
    {
        private const string SP_NAME = "Aff_CustomerSel";

        private string Prefix { get; }

        public GetAffCustomersCriteria(DateTime fromDate)
            => FromDate = fromDate;


        public override string GetSettingKey() => SP_NAME;

        public override bool IsValid() => true;

        public DateTime FromDate { get; }
    }
}