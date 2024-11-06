using System.Xml;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;
using UriSerializationHelper;

namespace XmlDomWriter.Serialization;

/// <summary>
/// Presents the serialization functionality of the sequence<see cref="IEnumerable{Uri}"/>
/// with using XML-DOM model.
/// </summary>
public class XmlDomTechnology : IDataSerializer<Uri>
{
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
            this.logger?.LogInformation("Serializing URIs to XML file.");

            var xmlDoc = new XmlDocument();

            var root = xmlDoc.CreateElement("uri-addresses");
            xmlDoc.AppendChild(root);

            foreach (var uri in source)
            {
                var uriElement = xmlDoc.CreateElement("Uri");
                uriElement.InnerText = uri.ToString();
                root.AppendChild(uriElement);
            }

            xmlDoc.Save(path);

            this.logger?.LogInformation("Serialization completed successfully.");
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "An error occurred during serialization to XML: {FilePath}", path);
            throw new Exception("An error occurred during serialization", ex);
        }
    }
}
