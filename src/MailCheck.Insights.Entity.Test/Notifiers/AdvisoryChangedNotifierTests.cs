using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.EmailSecurity.Entity.Entity.Notifiers;
using MailCheck.Insights.Entity.Config;
using NUnit.Framework;
using AdvisoryMessage = MailCheck.Common.Contracts.Advisories.AdvisoryMessage;
using MailCheck.Insights.Entity.Notifications;
using System.Linq;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Insights.Contracts;

namespace MailCheck.Insights.Entity.Test.Notifiers
{
    [TestFixture]
    public class AdvisoryChangedNotifierTests
    {
        private IMessageDispatcher _messageDispatcher;
        private IInsightEntityConfig _insightsEntityConfig;
        private IEqualityComparer<AdvisoryMessage> _messageEqualityComparer;
        private ILogger<AdvisoryChangedNotifier> _logger;

        private AdvisoryChangedNotifier _notifier;
        private const string Id = "test.gov.uk";
        private const string SnsTopicArn = "arn::aws:sns:test";

        [SetUp]
        public void SetUp()
        {
            _messageDispatcher = A.Fake<IMessageDispatcher>();
            _insightsEntityConfig = A.Fake<IInsightEntityConfig>();
            _messageEqualityComparer = new MessageEqualityComparer();
            _logger = A.Fake<ILogger<AdvisoryChangedNotifier>>();

            _notifier = new AdvisoryChangedNotifier(
                _messageDispatcher,
                _insightsEntityConfig,
                _messageEqualityComparer,
                _logger);

            A.CallTo(() => _insightsEntityConfig.WebUrl).Returns("testurl.com");
            A.CallTo(() => _insightsEntityConfig.SnsTopicArn).Returns(SnsTopicArn);
        }

        [TestCaseSource(nameof(ExerciseAdvisoryChangedNotifierTestAdvisories))]
        public void ExerciseAdvisoryChangedNotifier(AdvisoryChangedNotifierTestCase testCase)
        {
            _notifier.Handle(Id, testCase.CurrentAdvisories, testCase.IncomingAdvisories);

            AssessDispatch<InsightAdvisoryAdded>(testCase.ExpectedAdvisoriesAdded);
            AssessDispatch<InsightAdvisorySustained>(testCase.ExpectedAdvisoriesSustained);
            AssessDispatch<InsightAdvisoryRemoved>(testCase.ExpectedAdvisoriesRemoved);
        }

        private void AssessDispatch<T>(List<NamedAdvisory> expected) where T : InsightAdvisoryMessage
        {
            if (expected.Count > 0)
            {
                A.CallTo(() => _messageDispatcher.Dispatch(
                    A<T>.That.Matches(
                        x => x.Id.Equals(Id) && x.Messages.SequenceEqual(expected, _messageEqualityComparer)
                    ),
                    SnsTopicArn
                )).MustHaveHappenedOnceExactly();
            } else
            {
                A.CallTo(() => _messageDispatcher.Dispatch(A<T>._, SnsTopicArn)).MustNotHaveHappened();
            }
        }

        private static IEnumerable<AdvisoryChangedNotifierTestCase> ExerciseAdvisoryChangedNotifierTestAdvisories()
        {
            NamedAdvisory advisory1 = new NamedAdvisory
            (
                Guid.NewGuid(),
                "mailcheck.insights.testAdvisory1",
                MessageType.success,
                "Some text for advisory 1.",
                "Some markdown for advisory 1."
            );

            NamedAdvisory advisory2 = new NamedAdvisory
            (
                Guid.NewGuid(),
                "mailcheck.insights.testAdvisory2",
                MessageType.success,
                "Some text for advisory 2.",
                "Some markdown for advisory 2."
            );

            NamedAdvisory advisory3 = new NamedAdvisory
            (
                Guid.NewGuid(),
                "mailcheck.insights.testAdvisory3",
                MessageType.success,
                "Some text for advisory 3.",
                "Some markdown for advisory 3."
            );


            AdvisoryChangedNotifierTestCase test1 = new AdvisoryChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory>(),
                IncomingAdvisories = new List<NamedAdvisory>() { advisory1 },
                ExpectedAdvisoriesAdded = new List<NamedAdvisory> { advisory1 },
                ExpectedAdvisoriesRemoved = new List<NamedAdvisory>(),
                ExpectedAdvisoriesSustained = new List<NamedAdvisory>(),
                Description = "1 new advisory added with none sustained or removed"
            };

            AdvisoryChangedNotifierTestCase test2 = new AdvisoryChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory> { advisory1 },
                IncomingAdvisories = new List<NamedAdvisory>() { advisory1, advisory2 },
                ExpectedAdvisoriesAdded = new List<NamedAdvisory> { advisory2 },
                ExpectedAdvisoriesRemoved = new List<NamedAdvisory>(),
                ExpectedAdvisoriesSustained = new List<NamedAdvisory> { advisory1 },
                Description = "1 new advisory, 1 sustained, and none removed"
            };

            AdvisoryChangedNotifierTestCase test3 = new AdvisoryChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory> { advisory1, advisory3 },
                IncomingAdvisories = new List<NamedAdvisory>() { advisory1, advisory2 },
                ExpectedAdvisoriesAdded = new List<NamedAdvisory> { advisory2 },
                ExpectedAdvisoriesRemoved = new List<NamedAdvisory> { advisory3 },
                ExpectedAdvisoriesSustained = new List<NamedAdvisory> { advisory1 },
                Description = "1 new advisory, 1 sustained, and 1 removed"
            };

            AdvisoryChangedNotifierTestCase test4 = new AdvisoryChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory> { advisory1, advisory3 },
                IncomingAdvisories = new List<NamedAdvisory>() { advisory1 },
                ExpectedAdvisoriesAdded = new List<NamedAdvisory>(),
                ExpectedAdvisoriesRemoved = new List<NamedAdvisory> { advisory3 },
                ExpectedAdvisoriesSustained = new List<NamedAdvisory> { advisory1 },
                Description = "no new advisories, 1 sustained, and 1 removed"
            };

            AdvisoryChangedNotifierTestCase test5 = new AdvisoryChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory> { advisory3 },
                IncomingAdvisories = new List<NamedAdvisory>() { advisory1 },
                ExpectedAdvisoriesAdded = new List<NamedAdvisory> { advisory1 },
                ExpectedAdvisoriesRemoved = new List<NamedAdvisory> { advisory3 },
                ExpectedAdvisoriesSustained = new List<NamedAdvisory>(),
                Description = "1 new advisory, no sustained, and 1 removed"
            };


            yield return test1;
            yield return test2;
            yield return test3;
            yield return test4;
            yield return test5;
        }

        public class AdvisoryChangedNotifierTestCase
        {
            public List<NamedAdvisory> CurrentAdvisories { get; set; }
            public List<NamedAdvisory> IncomingAdvisories { get; set; }
            public List<NamedAdvisory> ExpectedAdvisoriesAdded { get; set; }
            public List<NamedAdvisory> ExpectedAdvisoriesRemoved { get; set; }
            public List<NamedAdvisory> ExpectedAdvisoriesSustained { get; set; }
            public string Description { get; set; }

            public override string ToString()
            {
                return Description;
            }
        }
    }
}
