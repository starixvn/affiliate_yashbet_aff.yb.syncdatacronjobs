using Aff.YB.DataSync.API.Infastructure.Params;
using Aff.YB.DataSync.DataDomain.Api;
using Aff.YB.DataSync.DataDomain.Models;
using Refit;

namespace Aff.YB.DataSync.Service.Infrastructure.Apis
{
    public interface ILicenseeApi
    {
        [Get("/Home/Index")]
        Task<string> CheckConnection();

        [Get("/customer/GetAffCustomers")]
        Task<IEnumerable<AffCustomerModel>> GetAffCustomerAsync(DateTime fromDate);

        [Get("/customer/GetCustomers")]
        Task<IEnumerable<CustomerModel>> GetCustomersAsync(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize = 2000);

        [Get("/customer/GetFixedCustomers")]
        Task<IEnumerable<CustomerModel>> GetFixedCustomers();

        [Get("/BetReport/GetBetRecord")]
        Task<IEnumerable<TpBetRecordModel>> GetBetRecordAsync(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize);

        [Get("/setting/GetServerInfo")]
        Task<IEnumerable<ServerInfoModel>> GetServerInfoAsync();

        [Get("/setting/GetTpGameType")]
        Task<IEnumerable<TpGameTypeModel>> GetGameTypeAsync();

        [Get("/betreport/GetDepositInfo")]
        Task<IEnumerable<DepositInfoModel>> GetDepositInfo(DateTime fromDate, DateTime toDate, int pageIndex, int pageSize = 2000);

        [Get("/BetReport/GetBonusInfo")]
        Task<IEnumerable<BonusModel>> GetBonusInfo(int id, int batchCount);

        [Get("/BetReport/GetBonusInfoByDate")]
        Task<IEnumerable<BonusModel>> GetBonusInfoByDate(DateTime calledTime, int pageIndex, int pageSize);

        [Get("/BetReport/GetWithdrawalInfo")]
        Task<IEnumerable<WithdrawalInfoModel>> GetWithdrawalInfo(DateTime updateTime, int batchCount);

        [Get("/LogReport/GetRebateMLogs")]
        Task<IEnumerable<RebateMLogModel>> GetRebateMLogs(DateTime fromDate, DateTime toDate);

        [Post("/customer/InsertAffiliateInfo")]
        Task<ResultInfo> InsertAffiliateInfo(AffiliateParam affiliateInfo);
    }
}