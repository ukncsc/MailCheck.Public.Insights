namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain
{
    public class FlaggedTrafficData
    {
        public int FlaggedTraffic { get; set; }
        public int Alltraffic { get; set; }
        public string Disposition { get; set; }
        public int? Pct { get; set; }
    }
}
