using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Wikiled.Common.Net.Client;
using Wikiled.Text.Anomaly.Service.Logic;

namespace Wikiled.Text.Anomaly.Service.Tests.Logic
{
    [TestFixture]
    public class DomainSentimentAnalysisFactoryTests
    {
        private readonly ILoggerFactory loggerFactory = new NullLoggerFactory();

        private Mock<IStreamApiClientFactory> mockStreamApiClientFactory;

        private DomainSentimentAnalysisFactory instance;

        [SetUp]
        public void SetUp()
        {
            mockStreamApiClientFactory = new Mock<IStreamApiClientFactory>();
            instance = CreateInstance();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new DomainSentimentAnalysisFactory(
                null,
                mockStreamApiClientFactory.Object));
            Assert.Throws<ArgumentNullException>(() => new DomainSentimentAnalysisFactory(
                loggerFactory,
                null));
        }

        private DomainSentimentAnalysisFactory CreateInstance()
        {
            return new DomainSentimentAnalysisFactory(
                loggerFactory,
                mockStreamApiClientFactory.Object);
        }
    }
}
