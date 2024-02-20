using Hangfire;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs.Interfaces
{
    public interface IRetryDataTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        Task HandleAsync();
    }
}
