using Fanex.Data.Repository;

namespace Aff.YB.DataSync.API.Infastructure.Criteria
{
    public class GetBonusInfoCriteria : CriteriaBase
    {
        public int Id { get; }

        public int BatchCount { get; }

        public GetBonusInfoCriteria(int id, int batchCount)
        {
            Id = id;
            BatchCount = batchCount;
        }

        public override string GetSettingKey() => "API_BonusInfo_Get";

        public override bool IsValid() => true;
    }
}
