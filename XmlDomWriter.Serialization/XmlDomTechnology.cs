using System.Xml;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;

namespace XmlDomWriter.Serialization;

/// <summary>
/// Presents the serialization functionality of the sequence<see cref="IEnumerable{Uri}"/>
/// with using XML-DOM model.
/// </summary>
public class XmlDomTechnology : IDataSerializer<Uri>
{
    private static readonly Action<ILogger, Exception, string> LogSerializationError =
(Action<ILogger, Exception, string>)LoggerMessage.Define<string>(
    LogLevel.Error,
    new EventId(1, nameof(XmlDomTechnology)),
    "An error occurred during serialization to XML: {FilePath}");

    private readonly string? path;
    private readonly ILogger<XmlDomTechnology>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDomTechnology"/> class.
    /// </summary>
    /// <param name="path">The path to json file.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Throw if text reader is null or empty.</exception>
    public XmlDomTechnology(string? path, ILogger<XmlDomTechnology>? logger = default)
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
            var xmlDoc = new XmlDocument();

            var root = xmlDoc.CreateElement("uri-addresses");
            _ = xmlDoc.AppendChild(root);

            foreach (var uri in source)
            {
                var uriElement = xmlDoc.CreateElement("Uri");
                uriElement.InnerText = uri.ToString();
                _ = root.AppendChild(uriElement);
            }

            xmlDoc.Save(this.path);
        }
        catch (Exception ex)
        {
            LogSerializationError(this.logger, ex, this.path ?? "Unknown path");
            throw new LogerExtensionException(ex.Message);
        }
    }
}
