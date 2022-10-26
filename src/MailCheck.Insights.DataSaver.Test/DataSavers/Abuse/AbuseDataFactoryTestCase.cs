using MailCheck.Insights.DataSaver.Contract;

namespace MailCheck.Insights.DataSaver.Test.DataSavers.Abuse
{
    public class AbuseDataFactoryTestCase
    {
        public AggregateReportRecordEnriched EnrichedRecord { get; set; }
        public int ExpectedFlagged { get; set; }
        public string ExpectedPolicy { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}