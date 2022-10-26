using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using MailCheck.Common.Contracts.Advisories;
using MailCheck.Common.Contracts.Findings;
using MailCheck.Common.Messaging.Abstractions;
using MailCheck.Common.Processors.Notifiers;
using MailCheck.Insights.Contracts;
using MailCheck.Insights.Entity.Config;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using LocalNotifier = MailCheck.Insights.Entity.Notifiers.FindingsChangedNotifier;

namespace MailCheck.EmailSecurity.Entity.Test.Entity.Notifiers
{
    [TestFixture]
    public class FindingsChangedNotifierTests
    {
        private IMessageDispatcher _messageDispatcher;
        private IFindingsChangedNotifier _findingsChangedNotifier;
        private IInsightEntityConfig _emailSecurityEntityConfig;
        private ILogger<LocalNotifier> _logger;
        private LocalNotifier _notifier;

        private const string Id = "test.gov.uk";
        private MessageEqualityComparer _equalityComparer;

        [SetUp]
        public void SetUp()
        {
            _messageDispatcher = A.Fake<IMessageDispatcher>();
            _findingsChangedNotifier = A.Fake<IFindingsChangedNotifier>();
            _emailSecurityEntityConfig = A.Fake<IInsightEntityConfig>();
            _logger = A.Fake<ILogger<LocalNotifier>>();
            _notifier = new LocalNotifier(_messageDispatcher, _emailSecurityEntityConfig, _findingsChangedNotifier, _logger);
            _equalityComparer = new MessageEqualityComparer();
        }

        [TestCaseSource(nameof(ExerciseFindingsChangedNotifierTestPermutations))]
        public void ExerciseFindingsChangedNotifier(FindingsChangedNotifierTestCase testCase)
        {
            A.CallTo(() => _emailSecurityEntityConfig.WebUrl).Returns("testurl.com");

            _notifier.Handle(Id, testCase.CurrentAdvisories, testCase.IncomingAdvisories);

            A.CallTo(() => _findingsChangedNotifier.Process(
                "test.gov.uk",
                "INSIGHTS",
                A<List<Finding>>.That.Matches(x => x.SequenceEqual(testCase.ExpectedCurrentFindings, _equalityComparer)),
                A<List<Finding>>.That.Matches(x => x.SequenceEqual(testCase.ExpectedIncomingFindings, _equalityComparer))
            )).MustHaveHappenedOnceExactly();
        }

        private static IEnumerable<FindingsChangedNotifierTestCase> ExerciseFindingsChangedNotifierTestPermutations()
        {
            Finding findingEvalError1 = new Finding
            {
                EntityUri = "domain:test.gov.uk",
                Name = "mailcheck.insights.testName1",
                SourceUrl = $"https://testurl.com/app/domain-security/{Id}/mta-sts",
                Severity = "Urgent",
                Title = "EvaluationError"
            };

            Finding findingEvalError2 = new Finding
            {
                EntityUri = "domain:test.gov.uk",
                Name = "mailcheck.insights.testName2",
                SourceUrl = $"https://testurl.com/app/domain-security/{Id}/mta-sts",
                Severity = "Urgent",
                Title = "EvaluationError"
            };

            Finding findingEvalWarn1 = new Finding
            {
                EntityUri = "domain:test.gov.uk",
                Name = "mailcheck.insights.testName3",
                SourceUrl = $"https://testurl.com/app/domain-security/{Id}/mta-sts",
                Severity = "Advisory",
                Title = "EvaluationWarning"
            };

            NamedAdvisory evalError1 = new NamedAdvisory(Guid.NewGuid(), "mailcheck.insights.testName1", MessageType.error, "EvaluationError", string.Empty);
            NamedAdvisory evalError2 = new NamedAdvisory(Guid.NewGuid(), "mailcheck.insights.testName2", MessageType.error, "EvaluationError", string.Empty);
            NamedAdvisory evalWarn1 = new NamedAdvisory(Guid.NewGuid(), "mailcheck.insights.testName3", MessageType.warning, "EvaluationWarning", string.Empty);


            FindingsChangedNotifierTestCase test1 = new FindingsChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory> { evalError1, evalError2, evalWarn1 },
                IncomingAdvisories = new List<NamedAdvisory>(),
                ExpectedCurrentFindings = new List<Finding> { findingEvalError1, findingEvalError2, findingEvalWarn1 },
                ExpectedIncomingFindings = new List<Finding>(),
                Description = "3 removed advisories should produce 3 current findings and no incoming findings"
            };

            FindingsChangedNotifierTestCase test2 = new FindingsChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory> { evalError1, evalError2, evalWarn1 },
                IncomingAdvisories = new List<NamedAdvisory> { evalError1, evalError2, evalWarn1 },
                ExpectedCurrentFindings = new List<Finding> { findingEvalError1, findingEvalError2, findingEvalWarn1 },
                ExpectedIncomingFindings = new List<Finding> { findingEvalError1, findingEvalError2, findingEvalWarn1 },
                Description = "3 current and 3 incoming advisories should produce 3 current findings and 3 incoming findings"
            };

            FindingsChangedNotifierTestCase test3 = new FindingsChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory>(),
                IncomingAdvisories = new List<NamedAdvisory> { evalError1, evalError2, evalWarn1 },
                ExpectedCurrentFindings = new List<Finding>(),
                ExpectedIncomingFindings = new List<Finding> { findingEvalError1, findingEvalError2, findingEvalWarn1 },
                Description = "3 incoming advisories and no current advisories should produce 3 incoming findings and no current findings"
            };

            FindingsChangedNotifierTestCase test4 = new FindingsChangedNotifierTestCase
            {
                CurrentAdvisories = new List<NamedAdvisory> { evalError1, evalError2, evalWarn1 },
                IncomingAdvisories = null,
                ExpectedCurrentFindings = new List<Finding> { findingEvalError1, findingEvalError2, findingEvalWarn1 },
                ExpectedIncomingFindings = new List<Finding>(),
                Description = "incoming advisories being null should produce incoming findings as empty"
            };

            FindingsChangedNotifierTestCase test5 = new FindingsChangedNotifierTestCase
            {
                CurrentAdvisories = null,
                IncomingAdvisories = new List<NamedAdvisory> { evalError1, evalError2, evalWarn1 },
                ExpectedCurrentFindings = new List<Finding>(),
                ExpectedIncomingFindings = new List<Finding> { findingEvalError1, findingEvalError2, findingEvalWarn1 },
                Description = "current advisories being null should produce current findings as empty"
            };


            yield return test1;
            yield return test2;
            yield return test3;
            yield return test4;
            yield return test5;
        }

        public class FindingsChangedNotifierTestCase
        {
            public List<NamedAdvisory> CurrentAdvisories { get; set; }
            public List<NamedAdvisory> IncomingAdvisories { get; set; }
            public List<Finding> ExpectedCurrentFindings { get; set; }
            public List<Finding> ExpectedIncomingFindings { get; set; }
            public string Description { get; set; }

            public override string ToString()
            {
                return Description;
            }
        }
    }
}