using System.Collections.Generic;
using System.Linq;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters;

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Factories
{
    public interface IFlaggedTrafficDataFactory
    {
        List<FlaggedTrafficData> Create<T>(List<T> source) where T: IFilterable;
    }


    public class FlaggedTrafficDataFactory : IFlaggedTrafficDataFactory
    {
        private readonly IEnumerable<IFilter> _nonMandatoryFilters;
        private readonly IEnumerable<IFilter> _mandatoryFilters;
        private readonly IEnumerable<IFilter> _overrideFilters;

        public FlaggedTrafficDataFactory(IEnumerable<IFilter> filters)
        {
            _nonMandatoryFilters = filters?.Where(x => x.FilterType == FilterType.NonMandatory);
            _mandatoryFilters = filters?.Where(x => x.FilterType == FilterType.Mandatory);
            _overrideFilters = filters?.Where(x => x.FilterType == FilterType.Override);
        }

        public List<FlaggedTrafficData> Create<T>(List<T> source) where T : IFilterable
        {
            List<FlaggedTrafficData> result = new List<FlaggedTrafficData>();

            foreach (var grouping in source.GroupBy(x => (x.Disposition, x.Pct)))
            {
                var rawData = (IGrouping<(string Disposition, int Pct), IFilterable>)grouping;
                IEnumerable<IFilterable> flaggedTraffic = rawData.Where(x => !_mandatoryFilters.All(mf => mf.Filter(x)) && _nonMandatoryFilters.Any(filter => !filter.Filter(x)) && !_overrideFilters.Any(FilterType => FilterType.Filter(x)));

                FlaggedTrafficData flaggedTrafficData = new FlaggedTrafficData
                {
                    Alltraffic = rawData.Sum(x => x.Count),
                    FlaggedTraffic = flaggedTraffic.Sum(x => x.Count),
                    Disposition = rawData.Key.Disposition,
                    Pct = rawData.Key.Pct
                };

                result.Add(flaggedTrafficData);
            }

            return result;
        }
    }
}