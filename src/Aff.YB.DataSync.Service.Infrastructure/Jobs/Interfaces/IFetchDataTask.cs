using Hangfire;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs.Interfaces
{
    public interface IFetchDataTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task FetchDataAsync();
    }
}
