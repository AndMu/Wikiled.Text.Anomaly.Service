using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wikiled.Common.Net.Client;
using Wikiled.Server.Core.Testing.Server;
using Wikiled.Text.Anomaly.Api.Data;
using Wikiled.Text.Anomaly.Api.Service;

namespace Wikiled.Text.Anomaly.Service.Tests.Acceptance
{
    [TestFixture]
    public class ParsingTests
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
            ServiceResponse<RawResponse<string>> response = await wrapper.ApiClient.GetRequest<RawResponse<string>>("api/parsing/version", CancellationToken.None).ConfigureAwait(false);
            Assert.IsTrue(response.IsSuccess);
        }

        [Test]
        public async Task ExtractFile()
        {
            DocumentParsing analysis = new DocumentParsing(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            byte[] data = await File.ReadAllBytesAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Research.pdf")).ConfigureAwait(false);
            var result = await analysis.Extract(
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
            Assert.AreEqual(35, result.Length);
            Assert.AreEqual(12, result[0].Sentences.Count);
        }

        [Test]
        public async Task ExtractText()
        {
            DocumentParsing analysis = new DocumentParsing(new ApiClientFactory(wrapper.Client, wrapper.Client.BaseAddress));
            var result = await analysis.Extract(
                new TextRequest
                {
                    Text = "Test document",
                    Header =
                        new RequestHeader
                        {
                            Domain = "Market"
                        }
                },
                CancellationToken.None).ConfigureAwait(false);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, result[0].Sentences.Count);
        }

        [TearDown]
        public void Cleanup()
        {
            wrapper.Dispose();
        }
    }
}
