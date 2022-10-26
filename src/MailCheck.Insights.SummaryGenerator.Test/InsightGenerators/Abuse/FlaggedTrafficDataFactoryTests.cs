using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Domain;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Factories;
using MailCheck.Insights.SummaryGenerator.InsightGenerators.Filters;
using NUnit.Framework;

namespace MailCheck.Insights.SummaryGenerator.Test.InsightGenerators.Abuse
{
    [TestFixture]
    public class FlaggedTrafficDataFactoryTests
    {
        private IFilter _filter;
        private IFilter _filter2;
        private IFilter _mandatoryFilter;
        private IFilter _overrideFilter;
        private FlaggedTrafficDataFactory _flaggedTrafficDataFactory;

        [SetUp]
        public void SetUp()
        {
            _filter = A.Fake<IFilter>();

            _filter2 = A.Fake<IFilter>();

            _mandatoryFilter = A.Fake<IFilter>();

            _overrideFilter = A.Fake<IFilter>();

            A.CallTo(() => _filter.FilterType).Returns(FilterType.NonMandatory);
            A.CallTo(() => _filter2.FilterType).Returns(FilterType.NonMandatory);
            A.CallTo(() => _mandatoryFilter.FilterType).Returns(FilterType.Mandatory);
            A.CallTo(() => _overrideFilter.FilterType).Returns(FilterType.Override);

            _flaggedTrafficDataFactory = new FlaggedTrafficDataFactory(new List<IFilter> { _filter, _filter2, _mandatoryFilter, _overrideFilter });
        }

        [Test]
        public void CreateGroupsAndSumsResults()
        {
            A.CallTo(() => _filter.Filter(A<IFilterable>.That.Matches(x => x.HostName == "flag"))).Returns(false);
            A.CallTo(() => _filter.Filter(A<IFilterable>.That.Matches(x => x.HostName == "no flag"))).Returns(true);
            A.CallTo(() => _filter2.Filter(A<IFilterable>.That.Matches(x => x.HostName == "flag"))).Returns(false);
            A.CallTo(() => _filter2.Filter(A<IFilterable>.That.Matches(x => x.HostName == "no flag"))).Returns(true);
            A.CallTo(() => _overrideFilter.Filter(A<IFilterable>.That.Matches(x => x.HostName == "override"))).Returns(true);

            List<RawAbuseData> source = new List<RawAbuseData>
            {
                new RawAbuseData { Count = 1, Pct = 10, Disposition = "none", HostName = "flag"},
                new RawAbuseData { Count = 2, Pct = 10, Disposition = "none", HostName = "flag" },
                new RawAbuseData { Count = 3, Pct = 10, Disposition = "reject", HostName = "flag" },
                new RawAbuseData { Count = 4, Pct = 10, Disposition = "reject", HostName = "no flag" },
                new RawAbuseData { Count = 5, Pct = 20, Disposition = "reject", HostName = "no flag" },
                new RawAbuseData { Count = 6, Pct = 30, Disposition = "reject", HostName = "no flag" },
                new RawAbuseData { Count = 7, Pct = 40, Disposition = "none", HostName = "override" }
            };

            List<FlaggedTrafficData> result = _flaggedTrafficDataFactory.Create(source);

            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(10, result[0].Pct);
            Assert.AreEqual("none", result[0].Disposition);
            Assert.AreEqual(3, result[0].Alltraffic);
            Assert.AreEqual(3, result[0].FlaggedTraffic);

            Assert.AreEqual(10, result[1].Pct);
            Assert.AreEqual("reject", result[1].Disposition);
            Assert.AreEqual(7, result[1].Alltraffic);
            Assert.AreEqual(3, result[1].FlaggedTraffic);

            Assert.AreEqual(20, result[2].Pct);
            Assert.AreEqual("reject", result[2].Disposition);
            Assert.AreEqual(5, result[2].Alltraffic);
            Assert.AreEqual(0, result[2].FlaggedTraffic);

            Assert.AreEqual(30, result[3].Pct);
            Assert.AreEqual("reject", result[3].Disposition);
            Assert.AreEqual(6, result[3].Alltraffic);
            Assert.AreEqual(0, result[3].FlaggedTraffic);

            Assert.AreEqual(40, result[4].Pct);
            Assert.AreEqual("none", result[4].Disposition);
            Assert.AreEqual(7, result[4].Alltraffic);
            Assert.AreEqual(0, result[4].FlaggedTraffic);
        }

        [TestCaseSource(nameof(ExerciseGenerateTestPermutations))]
        public void ExerciseFactoryPermutations(FlaggedTrafficDataFactoryTestCase testCase)
        {
            A.CallTo(() => _mandatoryFilter.Filter(A<IFilterable>._)).Returns(testCase.MandatoryFilterPass);
            A.CallTo(() => _filter.Filter(A<IFilterable>._)).Returns(testCase.NonMandatoryFilter1Pass);
            A.CallTo(() => _filter2.Filter(A<IFilterable>._)).Returns(testCase.NonMandatoryFilter2Pass);

            List<RawAbuseData> source = new List<RawAbuseData>
            {
                new RawAbuseData {Count = 1, Pct = 10,  Disposition = "none", HostName = "flag"},
                new RawAbuseData { Count = 2, Pct = 10, Disposition = "none", HostName = "flag" },
            };

            List<FlaggedTrafficData> result = _flaggedTrafficDataFactory.Create(source);

            Assert.AreEqual(testCase.ExpectedCount, result.Count);
            Assert.AreEqual(testCase.FlaggedTraffic, result[0].FlaggedTraffic);
        }

        private static IEnumerable<FlaggedTrafficDataFactoryTestCase> ExerciseGenerateTestPermutations()
        {
            FlaggedTrafficDataFactoryTestCase test1 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory Fail + Non-mandatory fail + Non-mandatory fail",
                MandatoryFilterPass = false,
                NonMandatoryFilter1Pass = false,
                NonMandatoryFilter2Pass = false,
                ExpectedCount = 1,
                FlaggedTraffic = 3
            };

            FlaggedTrafficDataFactoryTestCase test2 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "/Mandatory Fail +Non - mandatory fail + Non - mandatory pass",
                MandatoryFilterPass = false,
                NonMandatoryFilter2Pass = false,
                NonMandatoryFilter1Pass = true,
                ExpectedCount = 1,
                FlaggedTraffic = 3
            };

            FlaggedTrafficDataFactoryTestCase test3 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory Fail +Non - mandatory pass + Non - mandatory pass",
                MandatoryFilterPass = false,
                NonMandatoryFilter1Pass = true,
                NonMandatoryFilter2Pass = true,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            FlaggedTrafficDataFactoryTestCase test4 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory Pass+Non - mandatory fail + Non - mandatory fail",
                MandatoryFilterPass = true,
                NonMandatoryFilter1Pass = false,
                NonMandatoryFilter2Pass = false,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            FlaggedTrafficDataFactoryTestCase test5 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory Pass+Non-mandatory pass + Non - mandatory fail",
                MandatoryFilterPass = true,
                NonMandatoryFilter1Pass = true,
                NonMandatoryFilter2Pass = false,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            FlaggedTrafficDataFactoryTestCase test6 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory Pass+Non - mandatory pass + Non - mandatory pass",
                MandatoryFilterPass = true,
                NonMandatoryFilter1Pass = true,
                NonMandatoryFilter2Pass = true,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            FlaggedTrafficDataFactoryTestCase test7 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory pass+Non - mandatory pass + Non - mandatory pass + override",
                MandatoryFilterPass = true,
                NonMandatoryFilter1Pass = true,
                NonMandatoryFilter2Pass = true,
                OverrideFilterPass = true,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            FlaggedTrafficDataFactoryTestCase test8 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory Fail+Non - mandatory pass + Non - mandatory pass + override",
                MandatoryFilterPass = true,
                NonMandatoryFilter1Pass = true,
                NonMandatoryFilter2Pass = true,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            FlaggedTrafficDataFactoryTestCase test9 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory pass +Non - mandatory fail + Non - mandatory pass + override",
                MandatoryFilterPass = true,
                NonMandatoryFilter1Pass = true,
                NonMandatoryFilter2Pass = true,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            FlaggedTrafficDataFactoryTestCase test10 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory pass + Non - mandatory pass + Non - mandatory fail + override",
                MandatoryFilterPass = true,
                NonMandatoryFilter1Pass = true,
                NonMandatoryFilter2Pass = true,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            FlaggedTrafficDataFactoryTestCase test11 = new FlaggedTrafficDataFactoryTestCase
            {
                Description = "Mandatory fail + Non - mandatory fail + Non - mandatory fail + override",
                MandatoryFilterPass = true,
                NonMandatoryFilter1Pass = true,
                NonMandatoryFilter2Pass = true,
                ExpectedCount = 1,
                FlaggedTraffic = 0
            };

            yield return test1;
            yield return test2;
            yield return test3;
            yield return test4;
            yield return test5;
            yield return test6;
            yield return test7;
            yield return test8;
            yield return test9;
            yield return test10;
            yield return test11;
        }

        public class FlaggedTrafficDataFactoryTestCase
        {
            public string Description { get; set; }
            public bool MandatoryFilterPass { get; set; }
            public bool NonMandatoryFilter2Pass { get; set; }
            public bool NonMandatoryFilter1Pass { get; set; }
            public bool OverrideFilterPass { get; set; }
            public int ExpectedCount { get; set; }
            public int FlaggedTraffic { get; set; }
        }
    }
}
