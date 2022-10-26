using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Processors.Evaluators;
using MailCheck.Common.Util;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Contracts.Subdomains;
using Microsoft.Extensions.Logging;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Subdomains
{
    public class SubdomainInsightGenerator : IInsightGenerator
    {
        private readonly IEvaluator<SubdomainsEvaluationObject> _insightGenerators;
        private readonly ISubdomainInsightGeneratorDao _dao;
        private readonly ILogger<SubdomainInsightGenerator> _log;
        private Guid Id = new Guid("F4693FE1-A063-4956-8E01-8CA387E36ABD");

        public SubdomainInsightGenerator(ISubdomainInsightGeneratorDao dao,
            ILogger<SubdomainInsightGenerator> log, IEvaluator<SubdomainsEvaluationObject> insightGenerators)
        {
            _dao = dao;
            _log = log;
            _insightGenerators = insightGenerators;
        }

        public async Task<Insight> GenerateInsights(string id, DateTime startDate, DateTime endDate)
        {
            _log.LogInformation($"Begin - GenerateInsights of type Subdomain for {id}");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            SubdomainsRawData parentData = await _dao.GetRawData(id, startDate, endDate);

            List <SubdomainsData> data = GetProcessedData(parentData);

            if (data != null && data.Count > 0)
            {
                _log.LogInformation($"Subdomains data found for {id} after {stopwatch.ElapsedMilliseconds} ms");

                SubdomainsEvaluationObject evaluationObject = new SubdomainsEvaluationObject(
                    id,
                    data
                );

                EvaluationResult<SubdomainsEvaluationObject> subdomainInsights = await _insightGenerators.Evaluate(evaluationObject);

                if(subdomainInsights.AdvisoryMessages.Count > 0)
                {
                    _log.LogInformation($"{subdomainInsights.AdvisoryMessages.Count} subdomain insights found for {id}");

                    stopwatch.Stop();
                    _log.LogInformation($"Complete - GenerateInsights of type Subdomain for {id} in {stopwatch.ElapsedMilliseconds} ms");

                    return new Insight(InsightType.Subdomains, subdomainInsights.AdvisoryMessages.Select(adv => (NamedAdvisory)adv).ToList());
                }
            }

            return new Insight(InsightType.Subdomains, new List<NamedAdvisory>
            {
                new NamedAdvisory(Id, "mailcheck.insights.noSubdomainsDetected", MessageType.info, $"Mail Check did not detect any subdomains of {id} (Note we only show subdomains with > 10 emails in the last 30 days)", null)
            });

        }

        public List<SubdomainsData> GetProcessedData(SubdomainsRawData parentData)
        {
            List<SubdomainsData> subdomains = new List<SubdomainsData>();

            foreach (var subdomainsData in parentData.Subdomains.GroupBy(x => x.Domain))
            {
                string policy = subdomainsData.OrderByDescending(x => x.EffectiveDate).Select(x => x.P).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(policy))
                {
                    if (!string.IsNullOrWhiteSpace(parentData.ParentSubPolicy))
                    {
                        policy = parentData.ParentSubPolicy;
                    }
                    else if(!string.IsNullOrWhiteSpace(parentData.ParentPolicy))
                    {
                        policy = parentData.ParentPolicy;
                    }
                }

                subdomains.Add(new SubdomainsData
                {
                    Domain = subdomainsData.Key,
                    AllTrafficCount = subdomainsData.Sum(x => x.Count),
                    Policy = policy,
                    DmarcFailCount = subdomainsData.Where(x => x.Spf != "pass" && x.Dkim != "pass").Sum(y => y.Count)

                });
            }
            
            return subdomains;
        }
    }
}