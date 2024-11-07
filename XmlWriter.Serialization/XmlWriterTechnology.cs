using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;

namespace XmlWriter.Serialization;

/// <summary>
/// Presents the serialization functionality of the sequence<see cref="IEnumerable{Uri}"/>
/// with using XmlWriter class.
/// </summary>
public class XmlWriterTechnology : IDataSerializer<Uri>
{
    private static readonly Action<ILogger, Exception, string> LogSerializationError =
        (Action<ILogger, Exception, string>)LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, nameof(XmlWriterTechnology)),
            "An error occurred during serialization to XML: {FilePath}");

    private readonly string? path;
    private readonly ILogger<XmlWriterTechnology>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlWriterTechnology"/> class.
    /// </summary>
    /// <param name="path">The path to json file.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Throw if text reader is null or empty.</exception>
    public XmlWriterTechnology(string? path, ILogger<XmlWriterTechnology>? logger = default)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.logger = logger;
    }

    /// <summary>
    /// Serializes the source sequence of Uri elements in json format.
    /// </summary>
    /// <param name="source">The source sequence of Uri elements.</param>
    /// <exception cref="ArgumentNullException">Throw if the source sequence is null.</exception>
    public void Serialize(IEnumerable<Uri>? source)
    {
        ArgumentNullException.ThrowIfNull(source);

        try
        {
            using var writer = System.Xml.XmlWriter.Create(this.path, new System.Xml.XmlWriterSettings { Indent = true });
            writer.WriteStartDocument();
            writer.WriteStartElement("uriAdresses");

            foreach (var uri in source)
            {
                writer.WriteStartElement("uriAdress");

                writer.WriteStartElement("scheme");
                writer.WriteAttributeString("name", uri.Scheme);
                writer.WriteEndElement();

                writer.WriteStartElement("host");
                writer.WriteAttributeString("name", uri.Host);
                writer.WriteEndElement();

                writer.WriteStartElement("path");
                var segments = uri.AbsolutePath.Split('/')
                                  .Where(segment => !string.IsNullOrEmpty(segment));
                foreach (var segment in segments)
                {
                    writer.WriteStartElement("segment");
                    writer.WriteString(segment);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                if (!string.IsNullOrEmpty(uri.Query))
                {
                    writer.WriteStartElement("query");
                    var queryParameters = uri.Query.TrimStart('?').Split('&');
                    foreach (var param in queryParameters)
                    {
                        var parts = param.Split('=');
                        if (parts.Length == 2)
                        {
                            writer.WriteStartElement("parameter");
                            writer.WriteAttributeString("key", parts[0]);
                            writer.WriteAttributeString("value", parts[1]);
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
        catch (Exception ex)
        {
            LogSerializationError(this.logger, ex, this.path ?? "Unknown path");
            throw new LogerExtensionException(ex.Message);
        }
    }
}
