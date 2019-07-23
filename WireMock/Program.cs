using System;
namespace WireMock
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using RestEase;
    using WireMock.Client;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using WireMock.Server;
    using WireMock.Settings;

    class Program
    {
        static void Main(string[] args)
        {
            var server = FluentMockServer.Start(new FluentMockServerSettings
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

            Console.WriteLine("Press any key to stop the wire mock fake server");
            Console.ReadLine();

            server.Stop();


        }
    }
}

