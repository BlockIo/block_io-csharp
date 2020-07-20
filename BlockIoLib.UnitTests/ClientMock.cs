using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.IO;
using WireMock;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace BlockIoLib.UnitTests
{
    public class ClientMock
    {
        private static BlockIo blockIo;
        private static string api_key;
        private static string pin;

        private static WireMockServer stub;
        private static string baseUrl;

        private static object withdrawRequestBodyContent;
        private static dynamic signAndWithdrawalRequest;

        [OneTimeSetUp]
        public static void PrepareClass()
        {
            api_key = "0000-0000-0000-0000";
            pin = "blockiotestpininsecure";

            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port + "/api/v2";
            blockIo = new BlockIo(api_key, pin, 2, "{api_url: '" + baseUrl + "'}");
            using (StreamReader r = new StreamReader("./data/sign_and_finalize_withdrawal_request.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                signAndWithdrawalRequest = JsonConvert.DeserializeObject(json);
                signAndWithdrawalRequest = signAndWithdrawalRequest.signature_data.ToString();
                signAndWithdrawalRequest = JsonConvert.DeserializeObject(signAndWithdrawalRequest);
            }

            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:" + port },
                ReadStaticMappings = true
            });

            stub.Given(
                Request.Create()
                  .WithPath("/api/v2/sign_and_finalize_withdrawal")
                  .UsingPost()
                  .WithBody(new JsonMatcher(new { signature_data = signAndWithdrawalRequest.ToString() }))
                  )
                  .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson( new { 
                            status = "success", 
                            data =  new { 
                                network = "random", 
                                txid = "random" 
                            } 
                        }));
            
            withdrawRequestBodyContent = new { from_labels = "testdest", amounts = "100", to_labels = "default" };

            
        }

        [OneTimeTearDown]
        public static void CleanClass()
        {
            stub.Stop();
        }


        [Test]
        public void Test()
        {
            var response = blockIo.Withdraw(JsonConvert.SerializeObject(withdrawRequestBodyContent));
            Assert.AreEqual("success", response.Status);
            Assert.IsNotNull(response.Data);
        }
    }
}
