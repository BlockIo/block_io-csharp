using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace BlockIoLib.UnitTests
{
    public class ClientMock
    {

        private static WireMockServer stub;
        private static string baseUrl;

        [OneTimeSetUp]
        public static void PrepareClass()
        {
            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port;

            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:" + port },
                ReadStaticMappings = true
            });
        }

        [OneTimeTearDown]
        public static void CleanClass()
        {
            stub.Stop();
        }


        [Test]
        public void Test()
        {
            var bodyContent = new[] {
                                new {id = 1, description = "Book A" },
                                new {id = 2, description = "Book B" }
                            };

            stub.Given(
                Request
                .Create()
                    .WithPath("/api/products"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "aplication/json")
                        .WithBodyAsJson(bodyContent));

            var client = new RestClient(baseUrl);
            var request = new RestRequest("/api/products");

            var response = client.Execute(request);
            Console.WriteLine("resss: " + JsonConvert.DeserializeObject(response.Content));
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.AreEqual(JsonConvert.SerializeObject(bodyContent), response.Content);
        }


    }
}
