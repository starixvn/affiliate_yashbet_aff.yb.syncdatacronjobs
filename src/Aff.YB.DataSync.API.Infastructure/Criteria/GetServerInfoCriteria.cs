using Fanex.Data.Repository;

namespace Aff.YB.DataSync.API.Infastructure.Criteria
{
    public class GetServerInfoCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "API_Get_ServerInfo";

        public override bool IsValid() => true;
    }
}
