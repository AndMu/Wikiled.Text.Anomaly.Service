using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.Server.Core.Testing.Server;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Api.Service;

namespace Wikiled.Text.Anomaly.Service.Tests.Acceptance
{
    [TestFixture]
    public class SupervisedTests
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
            ServiceResponse<RawResponse<string>> response = await wrapper.ApiClient.GetRequest<RawResponse<string>>("api/supervised/version", CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(response.IsSuccess);
        }

        [Test]
        public async Task AnalysisDocuments()
        {
            DocumentParsing parsing = new DocumentParsing(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            byte[] data = await File.ReadAllBytesAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Research.pdf")).ConfigureAwait(false);
            var result = await parsing.Extract(
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
                            Domain = "Market"
                        }
                },
                CancellationToken.None).ConfigureAwait(false);

            var analysis = new SupervisedAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            DocumentAnomalyData anomalyData = new DocumentAnomalyData();
            anomalyData.Name = "Test";
            anomalyData.Negative = result.Take(2).Concat(result.Skip(30)).ToArray();
            anomalyData.Positive = result.Skip(2).Take(29).ToArray();
            await analysis.Reset("Test", CancellationToken.None).ConfigureAwait(false);
            await analysis.Add(anomalyData, CancellationToken.None).ConfigureAwait(false);
            await analysis.Train("Test", CancellationToken.None).ConfigureAwait(false);

            Assert.ThrowsAsync<ServiceException>(async () => await analysis.Resolve("xxx", result, CancellationToken.None).ConfigureAwait(false));
            Assert.ThrowsAsync<ServiceException>(async () => await analysis.Resolve("xxx", result[0], CancellationToken.None).ConfigureAwait(false));
            var anomaly = await analysis.Resolve("Test", result, CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(31, anomaly.Positive.Length);
            Assert.AreEqual(4, anomaly.Negative.Length);

            var sentence = await analysis.Resolve("Test", result[3], CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(16, sentence.Positive.Length);
            Assert.AreEqual(5, sentence.Negative.Length);
        }

        [TearDown]
        public void Cleanup()
        {
            wrapper.Dispose();
        }
    }
}
