using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Insights.DataSaver.Contract;
using MailCheck.Insights.DataSaver.DataSavers;
using MailCheck.Insights.DataSaver.DataSavers.Raw;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MailCheck.Insights.DataSaver.Test
{
    [TestFixture]
    public class AggregateReportRecordEnrichedHandlerTests
    {
        private AggregateReportRecordEnrichedHandler _aggregateReportRecordEnrichedHandler;
        private List<IDataSaver> _dataSavers;
        private IRawDataSaver _rawDataSaver;

        [SetUp]
        public void Setup()
        {
            _dataSavers = new List<IDataSaver>();
            _rawDataSaver = A.Fake<IRawDataSaver>();
            _aggregateReportRecordEnrichedHandler = new AggregateReportRecordEnrichedHandler(_dataSavers, _rawDataSaver, A.Fake<ILogger<AggregateReportRecordEnrichedHandler>>());
        }

        [Test]
        public async Task HandleCallsRegisteredDataSaversToSave()
        {
            IDataSaver fakeDataSaver = A.Fake<IDataSaver>();
            _dataSavers.Add(fakeDataSaver);

            AggregateReportRecordEnriched aggregateReportRecordEnriched = new AggregateReportRecordEnriched("test.com");

            await _aggregateReportRecordEnrichedHandler.Handle(aggregateReportRecordEnriched);

            A.CallTo(() => fakeDataSaver.SaveData(aggregateReportRecordEnriched)).MustHaveHappened();
        }

        [Test]
        public async Task HandleCallsRawDataSaverLast()
        {
            IDataSaver fakeDataSaver = A.Fake<IDataSaver>();
            _dataSavers.Add(fakeDataSaver);

            AggregateReportRecordEnriched aggregateReportRecordEnriched = new AggregateReportRecordEnriched("test.com");
            A.CallTo(() => fakeDataSaver.SaveData(aggregateReportRecordEnriched)).Invokes(() => { aggregateReportRecordEnriched.DomainFrom = "processed"; });

            await _aggregateReportRecordEnrichedHandler.Handle(aggregateReportRecordEnriched);

            A.CallTo(() => _rawDataSaver.SaveData(A<AggregateReportRecordEnriched>.That.Matches(x => x.DomainFrom == "processed"))).MustHaveHappened();
        }
    }
}