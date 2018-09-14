using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wikiled.Common.Net.Client;
using Wikiled.Server.Core.Testing.Server;
using Wikiled.Text.Analysis.Structure;
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
        public async Task Analysis()
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
            TrainingData trainingData = new TrainingData();
            trainingData.Name = "Test";
            trainingData.Negative = result.Take(2).Concat(result.Skip(30)).ToArray();
            trainingData.Positive = result.Skip(2).Take(29).ToArray();
            await analysis.Add(trainingData, CancellationToken.None).ConfigureAwait(false);
            await analysis.Train("Test", CancellationToken.None).ConfigureAwait(false);

            var removed = await analysis.Resolve("Test", result, CancellationToken.None).ConfigureAwait(false);
        }

        [TearDown]
        public void Cleanup()
        {
            wrapper.Dispose();
        }
    }
}
