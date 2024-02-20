using Fanex.Data.Repository;

namespace Aff.YB.DataSync.Service.Infrastructure.SpParams.Commands
{
    public abstract class IdenticalSuffixNonQueryCommand : IdenticalSurfixCriteriaBase, INonQueryCommand
    {
        protected IdenticalSuffixNonQueryCommand(string prefixProvider = null) : base(prefixProvider)
        {
        }
    }
}
