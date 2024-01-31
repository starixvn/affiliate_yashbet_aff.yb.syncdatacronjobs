using Aff.YB.DataSync.API.Infastructure.Criteria;
using Aff.YB.DataSync.DataDomain.Models;
using Fanex.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aff.YB.DataSync.API.Site.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LogReportController : ControllerBase
    {
        public IDynamicRepository _repo { get; }

        public LogReportController(IDynamicRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IEnumerable<RebateMLogModel>> GetRebateMLogs(DateTime fromDate, DateTime toDate)
            => await _repo.FetchAsync<RebateMLogModel>(new GetRebateMLogCriteria(fromDate, toDate));

    }
}
