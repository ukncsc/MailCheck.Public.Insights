namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters
{
    public interface IFilter
    {
        bool Filter(IFilterable source);
        FilterType FilterType { get; }
    }

    public enum FilterType
    {
        Mandatory,
        NonMandatory,
        Override
    }
}