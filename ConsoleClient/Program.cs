using ExportDataService;
using JsonSerializer.Serialization;
using Microsoft.Extensions.DependencyInjection;
using TextFileReceiver;
using UriConversion;
using XmlSerializer.Serialization;

namespace ConsoleClient;

public static class Program
{
    public static int Main()
    {
        //string SourceText = @"C:\Users\Giorgi\source\repos\dependency-injection-uri-serialization\ExportDataService.Tests\uri-addresses.txt";
        //string XmlActual = @"C:\Users\Giorgi\source\repos\dependency-injection-uri-serialization\ExportDataService.Tests\bin\Debug\net8.0\uri-Addresses.xml";
        //string JsonActual = @"C:\Users\Giorgi\source\repos\dependency-injection-uri-serialization\ExportDataService.Tests\bin\Debug\net8.0\uri-addresses.json";

        var provider = Startup.CreateServiceProvider();

        var service = provider.GetService<ExportDataService<Uri>>();

        if (service != null)
        {
            service.Run();
        }

        //var validatior = new UriValidator();
        //var converter = new UriConverter(validatior);
        //var reciever = new TextStreamReceiver(SourceText);
        //var jsonSerilizer = new JsonSerializerTechnology(JsonActual);
        //var xmlSerilizer = new XmlSerializerTechnology(XmlActual);

        //var dataService = new ExportDataService<Uri>(reciever, jsonSerilizer, converter);
        //dataService.Run();

        //var dataService2 = new ExportDataService<Uri>(reciever, xmlSerilizer, converter);
        //dataService2.Run();

        return 0;
    }
}
