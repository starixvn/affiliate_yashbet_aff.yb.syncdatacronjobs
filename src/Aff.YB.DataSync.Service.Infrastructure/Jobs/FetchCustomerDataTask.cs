using Aff.YB.DataSync.DataDomain.Models;
using Aff.YB.DataSync.Service.Core.Domain.DTOs;
using Aff.YB.DataSync.Service.Core.Repository;
using Aff.YB.DataSync.Service.Core.Services;
using Aff.YB.DataSync.Service.Infrastructure.Apis;
using Aff.YB.DataSync.Service.Infrastructure.Configurations;
using Aff.YB.DataSync.Service.Infrastructure.Constants;
using Aff.YB.DataSync.Service.Infrastructure.Helpers;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Base;
using Aff.YB.DataSync.Service.Infrastructure.Jobs.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Aff.YB.DataSync.Service.Infrastructure.Jobs
{
    public interface IFetchCustomerDataTask : IFetchDataTask
    {
        Task FetchDataDailyTask();

        Task FetchDataAsync(DateTime fromDate);

        Task FetchFixedDataTask();
    }

    public sealed class FetchCustomerDataTask : FetchDataForMultipleRepositoryTaskBase<CustomerModel>, IFetchCustomerDataTask
    {
        private const int BATCH_COUNT = 2000;

        private readonly ITrackerService _timeTrackerService;
        private readonly ILicenseeApi _liceseeApi;

        public FetchCustomerDataTask(IThirdPartyImportRepository thirdPartyImportRepository
            , ILicenseeApi licenseeApi
            , ITrackerService timeTrackerService
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, setting.Get(ServiceConfigName.PULL_CUSTOMER_SETTING))
        {
            _timeTrackerService = timeTrackerService ?? throw new ArgumentNullException(nameof(timeTrackerService));
            _liceseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public override async Task FetchDataAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var fromDate = GetAdjustmentTime(DateTime.ParseExact(tracker.KeyValue, Format.DateTimeFormat, null));
            await FetchDataAsync(fromDate);
        }

        public async Task FetchDataAsync(DateTime fromDate)
        {
            var timeRange = GetApiMaxTimeRange();
            var apiServerNow = DateTimeHelper.GetApiServerDateTime();
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, apiServerNow, timeRange);
            foreach (var range in dateRanges)
            {
                int pageIndex = 0;
                while (true)
                {
                    var data = await _liceseeApi.GetCustomersAsync(range.FromDate, range.ToDate, pageIndex, BATCH_COUNT);
                    if (data.Count() == 0)
                    {
                        break;
                    }
                    await ImportDataToMultipleRepoAsync(data);
                    pageIndex++;
                    if (data.Count() < BATCH_COUNT)
                    {
                        break;
                    }
                }
                var tracker = new TrackerInfo { CalledTime = DateTime.Now, KeyValue = range.ToDate.ToString(Format.DateTimeFormat) };
                await _timeTrackerService.SaveTrackerAsync(TableName, tracker);
            }
        }

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task FetchDataDailyTask()
        {
            var now = DateTimeHelper.GetApiServerDateTime();
            var adjustmentDay = now.AddDays(-1);
            var fromDate = new DateTime(adjustmentDay.Year, adjustmentDay.Month, adjustmentDay.Day, 0, 0, 0);
            var maxTimeRange = GetApiMaxTimeRange();
            var dateRanges = DateTimeHelper.GetProportionTimeRange(fromDate, now, maxTimeRange);
            foreach (var range in dateRanges)
            {
                var pageIndex = 0;
                while (true)
                {
                    var data = await _liceseeApi.GetCustomersAsync(range.FromDate, range.ToDate, pageIndex, BATCH_COUNT);
                    await ImportDataToMultipleRepoAsync(data);
                    pageIndex++;
                    if (data.Count() < BATCH_COUNT)
                    {
                        break;
                    }
                }
            }

        }

        public async Task FetchFixedDataTask()
        {
            var data = await _liceseeApi.GetFixedCustomers();
            await ImportDataToMultipleRepoAsync(data);
        }

        private DateTime GetAdjustmentTime(DateTime fromDate)
            => fromDate.AddMinutes(Settings.AdjustmentValue);


        protected override string BuildXml(IEnumerable<CustomerModel> data)
            => XmlHelper.BuildXml(data, "root", "row");

        protected override IEnumerable<string> GetSupportedRepoPrefix()
            => Settings.SupportedPrefix;

        private TimeSpan GetApiMaxTimeRange()
            => TimeSpan.FromMinutes(Settings.MaxTimeRange);
    }
}