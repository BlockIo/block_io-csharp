using System;
using WireMock.Server;
using WireMock.Settings;

namespace BlockIoLib.Mock
{
    class MockServer
    {
        public MockServer()
        {
            var stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:5001" },
                StartAdminInterface = true
            });
            Console.WriteLine("Press any key to stop the server");
            Console.ReadLine();
            stub.Stop();
        }
    }
}