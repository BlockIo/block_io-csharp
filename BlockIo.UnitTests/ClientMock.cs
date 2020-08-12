using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
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

        private static dynamic withdrawRequestBodyContent;
        private static dynamic sweepRequestBodyContent;
        private static dynamic dTrustRequestBodyContent;
        private static object sweepResponse;
        private static object withdrawResponse;
        private static dynamic signAndWithdrawalRequest;
        private static dynamic signAndSweepRequest;
        private static dynamic signAndDtrustRequest;
        private static dynamic withdrawFromDtrustAddressResponse;

        [OneTimeSetUp]
        public static void PrepareClass()
        {
            api_key = "0000-0000-0000-0000";
            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port + "/api/v2";


            stub = WireMockServer.Start(new WireMockServerSettings
            {
                Urls = new[] { "http://+:" + port }
            });

            using (StreamReader r = new StreamReader("./data/withdraw_response.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                withdrawResponse = JsonConvert.DeserializeObject(json);
            }

            using (StreamReader r = new StreamReader("./data/sign_and_finalize_withdrawal_request.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                signAndWithdrawalRequest = JsonConvert.DeserializeObject(json);
                signAndWithdrawalRequest = signAndWithdrawalRequest.signature_data.ToString();
                signAndWithdrawalRequest = JsonConvert.DeserializeObject(signAndWithdrawalRequest);
            }

            stub.Given(
                Request.Create()
                  .WithPath("/api/v2/withdraw")
                  )
                  .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(withdrawResponse));

            stub.Given(
                Request.Create()
                  .WithPath("/api/v2/sign_and_finalize_withdrawal")
                  .UsingPost()
                  .WithBody(new JsonMatcher(new { signature_data = JsonConvert.SerializeObject(signAndWithdrawalRequest) }))
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

            withdrawRequestBodyContent = new { from_labels = "testdest", amounts = "100", to_labels = "default" };

            var wif = "cTYLVcC17cYYoRjaBu15rEcD5WuDyowAw562q2F1ihcaomRJENu5";
            var key = new Key().FromWif(wif);
            sweepRequestBodyContent = new { to_address = "QhSWVppS12Fqv6dh3rAyoB18jXh5mB1hoC", from_address = "tltc1qpygwklc39wl9p0wvlm0p6x42sh9259xdjl059s", private_key = wif };

            using (StreamReader r = new StreamReader("./data/sweep_from_address_response.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                sweepResponse = JsonConvert.DeserializeObject(json);
            }
            using (StreamReader r = new StreamReader("./data/sign_and_finalize_sweep_request.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                signAndSweepRequest = JsonConvert.DeserializeObject(json);
                signAndSweepRequest = signAndSweepRequest.signature_data.ToString();
                signAndSweepRequest = JsonConvert.DeserializeObject(signAndSweepRequest);
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

            stub.Given(
                Request.Create()
                  .WithPath("/api/v2/sign_and_finalize_sweep")
                  .UsingPost()
                  .WithBody(new JsonMatcher(new { signature_data = JsonConvert.SerializeObject(signAndSweepRequest) }))
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

            dTrustRequestBodyContent = new { to_addresses = "QhSWVppS12Fqv6dh3rAyoB18jXh5mB1hoC", from_address = "tltc1q8y9naxlsw7xay4jesqshnpeuc0ap8fg9ejm2j2memwq4ng87dk3s88nr5j", amounts = 0.09 };

            using (StreamReader r = new StreamReader("./data/withdraw_from_dtrust_address_response.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                withdrawFromDtrustAddressResponse = JsonConvert.DeserializeObject(json);
            }

            using (StreamReader r = new StreamReader("./data/sign_and_finalize_sweep_request.json"))
            {
                string json = r.ReadToEnd().Replace(" ", "");
                signAndDtrustRequest = JsonConvert.DeserializeObject(json);
                signAndDtrustRequest = signAndDtrustRequest.signature_data.ToString();
                signAndDtrustRequest = JsonConvert.DeserializeObject(signAndDtrustRequest);
            }

            stub.Given(
                Request.Create()
                  .WithPath("/api/v2/withdraw_from_dtrust_address")
                  )
                  .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBodyAsJson(withdrawFromDtrustAddressResponse));

            stub.Given(
                Request.Create()
                  .WithPath("/api/v2/sign_and_finalize_withdrawal")
                  .UsingPost()
                  .WithBody(new JsonMatcher(new { signature_data = signAndDtrustRequest.ToString() }))
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
            blockIo = new BlockIo(api_key, pin, 2, new Options(baseUrl));

            var response = blockIo.Withdraw(withdrawRequestBodyContent);
            Assert.AreEqual("success", response.Status);
            Assert.IsNotNull(response.Data);
        }
        [Test]
        public void Sweep()
        {
            blockIo = new BlockIo(api_key, null, 2, new Options(baseUrl));
            var response = blockIo.SweepFromAddress(sweepRequestBodyContent);
            Assert.AreEqual("success", response.Status);
            Assert.IsNotNull(response.Data);
        }

        [Test]
        public void Dtrust()
        {
            pin = "blockiotestpininsecure";
            blockIo = new BlockIo(api_key, pin, 2, new Options(baseUrl));
            var response = blockIo.WithdrawFromDtrustAddress(dTrustRequestBodyContent);
            Assert.AreEqual("success", response.Status);
            Assert.IsNotNull(response.Data);
        }

    }
}
