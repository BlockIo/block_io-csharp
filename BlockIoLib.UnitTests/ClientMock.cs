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
        private static object sweepRequestBodyContent;
        private static object sweepResponse;
        private static dynamic signAndWithdrawalRequest;
        private static dynamic signAndSweepRequest;

        [OneTimeSetUp]
        public static void PrepareClass()
        {
            api_key = "0000-0000-0000-0000";
            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port + "/api/v2";
            
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

            var wif = "cTYLVcC17cYYoRjaBu15rEcD5WuDyowAw562q2F1ihcaomRJENu5";
            var key = new Key().FromWif(wif);
            sweepRequestBodyContent = new { to_address = "QhSWVppS12Fqv6dh3rAyoB18jXh5mB1hoC", from_address = "tltc1qpygwklc39wl9p0wvlm0p6x42sh9259xdjl059s", private_key = wif };
            using (StreamReader r = new StreamReader("./data/sweep_from_address_response.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                sweepResponse = JsonConvert.DeserializeObject(json);
            }
            stub.Given(
                Request.Create()
                  .WithPath("/api/v2/sweep_from_address")
                  )
                  .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(sweepResponse));

            using (StreamReader r = new StreamReader("./data/sign_and_finalize_sweep_request.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                signAndSweepRequest = JsonConvert.DeserializeObject(json);
                signAndSweepRequest = signAndSweepRequest.signature_data.ToString();
                signAndSweepRequest = JsonConvert.DeserializeObject(signAndSweepRequest);
            }

            stub.Given(
                Request.Create()
                  .WithPath("/api/v2/sign_and_finalize_sweep")
                  .UsingPost()
                  .WithBody(new JsonMatcher(new { signature_data = signAndSweepRequest.ToString() }))
                  )
                  .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(new
                        {
                            status = "success",
                            data = new
                            {
                                network = "random",
                                txid = "random"
                            }
                        }));
        }

        [OneTimeTearDown]
        public static void CleanClass()
        {
            stub.Stop();
        }


        [Test]
        public void Withdraw()
        {
            pin = "blockiotestpininsecure";
            blockIo = new BlockIo(api_key, pin, 2, "{api_url: '" + baseUrl + "'}");

            var response = blockIo.Withdraw(JsonConvert.SerializeObject(withdrawRequestBodyContent));
            Assert.AreEqual("success", response.Status);
            Assert.IsNotNull(response.Data);
        }
        [Test]
        public void Sweep()
        {
            blockIo = new BlockIo(api_key, null, 2, "{api_url: '" + baseUrl + "'}");
            var response = blockIo.SweepFromAddress(JsonConvert.SerializeObject(sweepRequestBodyContent));
            Assert.AreEqual("success", response.Status);
            Assert.IsNotNull(response.Data);
        }
    }
}
