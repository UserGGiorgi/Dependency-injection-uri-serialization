using System.Xml.Serialization;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;
using UriSerializationHelper;

namespace XmlSerializer.Serialization
{
    public class XmlSerializerTechnology : IDataSerializer<Uri>
    {
        private static readonly Action<ILogger, Exception, string> LogSerializationError =
        (Action<ILogger, Exception, string>)LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, nameof(XmlSerializerTechnology)),
            "An error occurred during serialization to XML: {FilePath}");

        private readonly string? path;
        private readonly ILogger<XmlSerializerTechnology>? logger;

        public XmlSerializerTechnology(string? path, ILogger<XmlSerializerTechnology>? logger = default)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.logger = logger;
        }

        public void Serialize(IEnumerable<Uri>? source)
        {
            ArgumentNullException.ThrowIfNull(source);

            try
            {
                var uriAddresses = source.Select(uri => uri.ToSerializableObject()).ToList();

                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(UriContainer), new XmlRootAttribute("uriAdresses"));

                using var writer = new StreamWriter(this.path);
                var uriContainer = new UriContainer(uriAddresses);
                xmlSerializer.Serialize(writer, uriContainer);
            }
            catch (Exception ex)
            {
                LogSerializationError(this.logger, ex, this.path ?? "Unknown path");
                throw new LogerExtensionException(ex.Message);
            }
        }
    }
}
