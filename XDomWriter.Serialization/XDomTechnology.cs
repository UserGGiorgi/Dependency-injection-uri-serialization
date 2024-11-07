using System.Xml.Linq;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;

namespace XDomWriter.Serialization;

/// <summary>
/// Presents the serialization functionality of the sequence <see cref="IEnumerable{Uri}"/>
/// using the X-DOM model.
/// </summary>
public class XDomTechnology : IDataSerializer<Uri>
{
    private static readonly Action<ILogger, Exception, string> LogSerializationError =
(Action<ILogger, Exception, string>)LoggerMessage.Define<string>(
    LogLevel.Error,
    new EventId(1, nameof(XDomTechnology)),
    "An error occurred during serialization to XML: {FilePath}");

    private readonly string? path;
    private readonly ILogger<XDomTechnology>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="XDomTechnology"/> class.
    /// </summary>
    /// <param name="path">The path to the XML file.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown if path is null.</exception>
    public XDomTechnology(string? path, ILogger<XDomTechnology>? logger = default)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.logger = logger;
    }

    /// <summary>
    /// Serializes the source sequence of Uri elements in XML format.
    /// </summary>
    /// <param name="source">The source sequence of Uri elements.</param>
    /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
    public void Serialize(IEnumerable<Uri>? source)
    {
        ArgumentNullException.ThrowIfNull(source);

        try
        {
            var root = new XElement("uri-addresses");

            foreach (var uri in source)
            {
                var uriElement = new XElement(
                    "uri-address",
                    new XElement("scheme", new XAttribute("name", uri.Scheme)),
                    new XElement("host", new XAttribute("name", uri.Host)),
                    new XElement(
                        "path",
                        uri.AbsolutePath.Split('/')
                            .Where(segment => !string.IsNullOrEmpty(segment))
                            .Select(segment => new XElement("segment", segment))));

                if (!string.IsNullOrEmpty(uri.Query))
                {
                    var queryElement = new XElement(
                        "query",
                        uri.Query.TrimStart('?').Split('&')
                            .Select(param =>
                            {
                                var parts = param.Split('=');
                                return parts.Length == 2
                                    ? new XElement(
                                        "parameter",
                                        new XAttribute("key", parts[0]),
                                        new XAttribute("value", parts[1]))
                                    : null;
                            })
                            .Where(x => x != null));

                    uriElement.Add(queryElement);
                }

                root.Add(uriElement);
            }

            var xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            xDoc.Save(this.path);
        }
        catch (Exception ex)
        {
            LogSerializationError(this.logger, ex, this.path ?? "Unknown path");
            throw new LogerExtensionException(ex.Message);
        }
    }
}
