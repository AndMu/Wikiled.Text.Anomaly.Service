using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.Server.Core.Testing.Server;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Api.Service;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Service.Tests.Acceptance
{
    [TestFixture]
    public class UnusupervisedTests
    {
        private ServerWrapper wrapper;

        [SetUp]
        public void SetUp()
        {
            wrapper = ServerWrapper.Create<Startup>(TestContext.CurrentContext.TestDirectory, services => { });
        }

        [Test]
        public async Task Version()
        {
            ServiceResponse<RawResponse<string>> response = await wrapper.ApiClient.GetRequest<RawResponse<string>>("api/anomaly/version", CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(response.IsSuccess);
        }

        [Test]
        public async Task Measure()
        {
            AnomalyAnalysis analysis = new AnomalyAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            byte[] data = await File.ReadAllBytesAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Research.pdf")).ConfigureAwait(false);
            AnomalyResult result = await analysis.RemoveAnomaly(
                                       new FileRequest
                                       {
                                           FileData = new FileData
                                                      {
                                                          Data = data,
                                                          Name = "Market.pdf",
                                                      },
                                           Header =
                                               new RequestHeader
                                               {
                                                   Domain = "Market",

                                                   AnomalyFilters = new[] {FilterTypes.Svm}
                                               }
                                       },
                                       CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(-0.49, Math.Round(result.Sentiment.Value, 2));
            Assert.AreEqual(13617, result.Document.TotalWords);
            Assert.AreEqual(614, result.Document.Sentences.Count);
        }

        [TearDown]
        public void Cleanup()
        {
            wrapper.Dispose();
        }
    }
}
