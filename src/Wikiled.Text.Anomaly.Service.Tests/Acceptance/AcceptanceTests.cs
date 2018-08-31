using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.Common.Net.Client;
using Wikiled.Server.Core.Testing.Server;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Api.Service;
using Wikiled.Text.Anomaly.Processing.Filters;

namespace Wikiled.Text.Anomaly.Service.Tests.Acceptance
{
    [TestFixture]
    public class AcceptanceTests
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
            var response = await wrapper.ApiClient.GetRequest<RawResponse<string>>("api/parser/version", CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(response.IsSuccess);
        }

        [Test]
        public async Task Measure()
        {
            AnomalyAnalysis analysis = new AnomalyAnalysis(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            var data = await File.ReadAllBytesAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Research.pdf")).ConfigureAwait(false);
            var result = await analysis.Measure(
                new FileAnomalyRequest
                {
                    Data = data,
                    Name = "Market.pdf",
                    Header =
                        new AnomalyRequestHeader
                        {
                            Domain = "Market",

                            Filters = new[] {FilterTypes.Svm}
                        }
                },
                CancellationToken.None).ConfigureAwait(false);
        }

        [TearDown]
        public void Cleanup()
        {
            wrapper.Dispose();
        }
    }
}
