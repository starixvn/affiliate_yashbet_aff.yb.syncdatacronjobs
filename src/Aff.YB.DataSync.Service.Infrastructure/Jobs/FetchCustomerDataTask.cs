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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            , ILogger logger
            , ITrackerService timeTrackerService
            , IOptionsSnapshot<FetchServiceSetting> setting)
            : base(thirdPartyImportRepository, setting.Get(ServiceConfigName.PULL_CUSTOMER_SETTING))
        {
            _timeTrackerService = timeTrackerService ?? throw new ArgumentNullException(nameof(timeTrackerService));
            _liceseeApi = licenseeApi ?? throw new ArgumentNullException(nameof(licenseeApi));
        }

        public override async Task FetchDataAsync()
        {
            var tracker = await _timeTrackerService.GetTrackerAsync(TableName, t => t.CalledTime);
            var fromDate = GetAdjustmentTime(DateTime.Parse(tracker.KeyValue));
            await FetchDataAsync(fromDate);
        }

        public async Task FetchDataAsync(DateTime fromDate)
        {
            var reqFromDate = fromDate;
            while (true)
            {
                var data = await _liceseeApi.GetCustomersAsync(reqFromDate, BATCH_COUNT);
                var toDate = data.Max(_ => _.RegTime);
                reqFromDate = toDate.Value;
                await ImportDataToMultipleRepoAsync(data);
                var tracker = new TrackerInfo { CalledTime = DateTime.Now, KeyValue = toDate.Value.ToString(Format.DateTimeFormat) };
                await _timeTrackerService.SaveTrackerAsync(TableName, tracker);
                if (data.Count() < BATCH_COUNT)
                {
                    break;
                }
            }
        }

        public async Task FetchDataDailyTask()
        {
            var adjustmentDay = DateTime.Now.AddDays(-1);
            var fromDate = new DateTime(adjustmentDay.Year, adjustmentDay.Month, adjustmentDay.Day, 0, 0, 0);
            while (true)
            {
                var data = await _liceseeApi.GetCustomersAsync(fromDate, BATCH_COUNT);
                var toDate = data.Max(_ => _.RegTime);
                fromDate = toDate.Value;
                await ImportDataToMultipleRepoAsync(data);
                if (data.Count() < BATCH_COUNT)
                {
                    break;
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
    }
}