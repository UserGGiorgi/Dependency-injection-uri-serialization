using Microsoft.Extensions.Logging;
using Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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

                var uriAddresses = source.Select(uri => new UriAddress
                {
                    Scheme = new NameElement { Name = uri.Scheme },
                    Host = new NameElement { Name = uri.Host },
                    Path = uri.AbsolutePath.Split('/').Where(segment => !string.IsNullOrEmpty(segment))
                           .Select(segment => new PathSegment { Segment = segment }).ToList(),
                    QueryParameters = string.IsNullOrEmpty(uri.Query) ? null :
                        uri.Query.TrimStart('?').Split('&').Select(q =>
                        {
                            var parts = q.Split('=');
                            return new QueryParameter { Key = parts[0], Value = parts.Length > 1 ? parts[1] : string.Empty };
                        }).ToList(),
                }).ToList();

                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(UriAddresses), new XmlRootAttribute("uriAdresses"));

                using (var writer = new StreamWriter(this.path))
                {
                    xmlSerializer.Serialize(writer, new UriAddresses { Addresses = uriAddresses });
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

    [XmlRoot("uriAdresses")]
    public class UriAddresses
    {
        [XmlElement("uriAdress")]
        public List<UriAddress> Addresses { get; set; } = new List<UriAddress>();
    }

    public class UriAddress
    {
        [XmlElement("scheme")]
        public NameElement Scheme { get; set; }

        [XmlElement("host")]
        public NameElement Host { get; set; }

        [XmlArray("path")]
        [XmlArrayItem("segment")]
        public List<PathSegment> Path { get; set; } = new List<PathSegment>();

        [XmlArray("query")]
        [XmlArrayItem("parameter")]
        public List<QueryParameter>? QueryParameters { get; set; }
    }

    public class NameElement
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class PathSegment
    {
        [XmlText]
        public string Segment { get; set; } = string.Empty;
    }

    public class QueryParameter
    {
        [XmlAttribute("key")]
        public string Key { get; set; } = string.Empty;

        [XmlAttribute("value")]
        public string Value { get; set; } = string.Empty;
    }
}
