using System.Net;
using System.Threading.Tasks;
using MailCheck.Common.Api.Authorisation.Filter;
using MailCheck.Common.Api.Authorisation.Service.Domain;
using MailCheck.Insights.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MailCheck.Insights.Api.Service;

namespace MailCheck.Insights.Api.Controllers
{
    [Route("api/dmarc-insights")]
    public class InsightsController : Controller
    {
        private readonly IInsightsService _insightsService;
        private readonly ILogger<InsightsController> _log;

        public InsightsController(IInsightsService insightsService, ILogger<InsightsController> log)
        {
            _insightsService = insightsService;
            _log = log;
        }

        [HttpGet("{domain}")]
        [MailCheckAuthoriseResource(Operation.Read, ResourceType.Insights, "domain")]
        public async Task<ActionResult<InsightsResponseModel>> GetInsights(string domain)
        {
            InsightsResponseModel result = await _insightsService.GetInsights(domain);

            if (result == null)
            {
                return new NotFoundResult();
            }

            return result;
        }

        [HttpGet("{domain}/summary")]
        [MailCheckAuthoriseResource(Operation.Read, ResourceType.Insights, "domain")]
        public async Task<ActionResult<InsightsSummaryResponseModel>> GetInsightSummaries(string domain)
        {
            InsightsSummaryResponseModel result = await _insightsService.GetInsightSummaries(domain);
            if (result == null)
            {
                return new NotFoundResult();
            }

            return result;
        }
    }
}