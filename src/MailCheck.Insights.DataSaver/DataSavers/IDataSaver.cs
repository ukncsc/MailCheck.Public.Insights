using System.Threading.Tasks;
using MailCheck.Insights.DataSaver.Contract;

namespace MailCheck.Insights.DataSaver.DataSavers
{
    public interface IDataSaver
    {
        Task SaveData(AggregateReportRecordEnriched aggregateReportRecordEnriched);
    }
}
