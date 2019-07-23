using WireMockTests.Models;

namespace WireMockTests
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using WireMock.Server;
    using WireMock.Settings;

    [TestFixture()]
    public class ApiServiceTest
    {
        private FluentMockServer _stubServer;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            this._stubServer = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://localhost:9095/" },
                StartAdminInterface = true,
                ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "http://localhost:56749/",
                    SaveMapping = true,
                    SaveMappingToFile = true
                }
            });

            this._httpClient = new HttpClient();
        }

        [TearDown]
        public void Teardown()
        {
            this._stubServer.Stop();
        }

        [Test]
        public async Task Given_A_Request_With_Expected_Contract_When_Invoking_Request_Then_Expected_Response_Is_Returned()
        {
            this._stubServer.Given(Request.Create()
                            .WithPath("/accertifyStub")
                            .WithBody(this.SerializeAccertifyPayload())
                            .UsingPost())
                            .RespondWith(Response.Create()
                            .WithStatusCode(200)
                            .WithHeader("Content-Type", "application/json")
                            .WithBody(this.SerializeAccertifyResponse()));

            var searchUrl = $"http://localhost:{this._stubServer.Ports[0]}/accertifyStub";

            var response = await this._httpClient.PostAsync(searchUrl, new StringContent(this.SerializeAccertifyPayload()));
            var responseString = await response.Content.ReadAsStringAsync();
            var accertifyResponse = JsonConvert.DeserializeObject<AccertifyResponse>(responseString);

            Assert.AreEqual(50, accertifyResponse.TotalScore);
        }

        [Test]
        public async Task Given_A_Request_To_Proxy_When_Invoking_Request_Then_Request_Is_Proxied_And_Response_Is_Returned()
        {
            var searchUrl = $"http://localhost:{this._stubServer.Ports[0]}/";

            var response = await this._httpClient.PostAsync(searchUrl, new StringContent(string.Empty));

            AccertifyResponse accertifyResponse;

            using (var stringReader = new StringReader(await response.Content.ReadAsStringAsync()))
            {
                accertifyResponse = (AccertifyResponse)new XmlSerializer(typeof(AccertifyResponse)).Deserialize(stringReader);
            }
            
            Assert.AreEqual(-5000, accertifyResponse.TotalScore);
        }

        private string SerializeAccertifyPayload()
        {
            var accertifyPayload = new AccertifyPayload()
            {
                Order = new Order()
                {
                    Number = 0,
                    Payment = new Payment()
                    {
                        Address = new Address()
                        {
                            LastName = "accept"
                        }
                    }
                }
            };

            return JsonConvert.SerializeObject(accertifyPayload);
        }

        private string SerializeAccertifyResponse()
        {
            var accertifyResponse = new AccertifyResponse()
            {
                CrossReference = "crossreference",
                Remarks = "remarks",
                Result = AccertifyResult.Accept,
                TotalScore = 50,
                TransactionId = "0"
            };

            return JsonConvert.SerializeObject(accertifyResponse);
        }
    }
}
