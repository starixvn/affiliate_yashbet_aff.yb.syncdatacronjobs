using Fanex.Data.Repository;
using System;

namespace Aff.YB.DataSync.API.Infastructure.Criteria
{
    public class GetDepositInfoCriteria : CriteriaBase
    {
        public GetDepositInfoCriteria(DateTime updatedTime, int depositId, int batchCount = 2000)
        {
            UpdatedTime = updatedTime;
            Id = depositId;
            BatchCount = batchCount;
        }

        public override string GetSettingKey()
            => "API_DepositInfo_Get";

        public override bool IsValid()
            => true;

        public DateTime UpdatedTime { get; set; }

        public int Id { get; set; }

        public int BatchCount { get; set; }
    }
}
