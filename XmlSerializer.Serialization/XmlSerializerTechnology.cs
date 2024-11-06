using Microsoft.Extensions.Logging;
using Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UriSerializationHelper;
using static UriSerializationHelper.UriAddress;

namespace XmlSerializer.Serialization
{
    public class XmlSerializerTechnology : IDataSerializer<Uri>
    {
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
                this.logger?.LogInformation("Serializing URIs to XML file.");

                // Using the extension method to convert Uri to UriAddress
                var uriAddresses = source.Select(uri => uri.ToSerializableObject()).ToList();

                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(UriContainer), new XmlRootAttribute("uriAdresses"));

                using (var writer = new StreamWriter(this.path))
                {
                    var uriContainer = new UriContainer(uriAddresses);
                    xmlSerializer.Serialize(writer, uriContainer);
                }

                this.logger?.LogInformation("Serialization completed successfully.");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "An error occurred during serialization to XML: {FilePath}", path);
                throw new Exception("An error occurred during serialization", ex);
            }
        }
    }
}
