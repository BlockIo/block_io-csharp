using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.IO;
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
        private static object successResponseBody;

        [OneTimeSetUp]
        public static void PrepareClass()
        {
            api_key = "0000-0000-0000-0000";
            pin = "blockiotestpininsecure";
            
            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port + "/api/v2";
            blockIo = new BlockIo(api_key, pin, 2, "{api_url: '" + baseUrl +"'}");

            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:" + port },
                ReadStaticMappings = true
            });

            withdrawRequestBodyContent = new { from_labels = "testdest", amounts = "100", to_labels = "default" };
            successResponseBody = new {status= "success", data = new[] { new { network = "random", txid = "random" } }};

            //using (StreamReader r = new StreamReader("file.json"))
            //{
            //    string json = r.ReadToEnd();
            //    sweepRequestBodyContent = JsonConvert.DeserializeObject(json);
            //}
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
