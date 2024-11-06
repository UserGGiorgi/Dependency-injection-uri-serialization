using Conversion;
using ExportDataService;
using JsonSerializer.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Serialization;
using UriConversion;
using Validation;
using XDomWriter.Serialization;
using XmlDomWriter.Serialization;
using XmlSerializer.Serialization;
using XmlWriter.Serialization;

namespace ConsoleClient;

public static class Startup
{
    public static IServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        _ = LogManager.Setup()
            .SetupExtensions(s => s.RegisterConfigSettings(configuration))
            .GetCurrentClassLogger();

        return new ServiceCollection()
            .AddTransient<IValidator<string>, UriValidator>()
            .AddTransient<IConverter<Uri?>, UriConverter>()
            .AddTransient<ExportDataService<Uri>>()
            .AddSingleton<IDataSerializer<Uri>, XmlWriterTechnology>()
            .AddSingleton<IDataSerializer<Uri>, XmlSerializerTechnology>()
            .AddSingleton<IDataSerializer<Uri>, XmlDomTechnology>()
            .AddSingleton<IDataSerializer<Uri>, XDomTechnology>()
            .AddSingleton<IDataSerializer<Uri>, JsonSerializerTechnology>().UseExportDataServices(configuration, configuration["format"], configuration["mode"])
            .AddLogging(loggingBuilder =>
            {
                _ = loggingBuilder.ClearProviders();
                _ = loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                _ = loggingBuilder.AddNLog(configuration);
            })
            .BuildServiceProvider();
    }
}
