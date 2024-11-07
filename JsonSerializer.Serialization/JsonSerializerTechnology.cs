using System.Text.Json;
using System.Web;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Serialization;

namespace JsonSerializer.Serialization
{
    /// <summary>
    /// Presents the serialization functionality of the sequence<see cref="IEnumerable{Uri}"/>
    /// with using JsonSerialization class.
    /// </summary>
    public class JsonSerializerTechnology : IDataSerializer<Uri>
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        private readonly string? path;
        private readonly ILogger<JsonSerializerTechnology>? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializerTechnology"/> class.
        /// </summary>
        /// <param name="path">The path to json file.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentException">Thrown if the path is null or empty.</exception>
        public JsonSerializerTechnology(string? path, ILogger<JsonSerializerTechnology>? logger = default)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.logger = logger;
        }

        /// <summary>
        /// Serializes the source sequence of Uri elements in json format.
        /// </summary>
        /// <param name="source">The source sequence of Uri elements.</param>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public void Serialize(IEnumerable<Uri>? source)
        {
            ArgumentNullException.ThrowIfNull(source);
            try
            {
                this.logger?.LogInformation("Serializing URIs to JSON format");

                var transformedUris = source.Select(uri =>
                {
                    var queryCollection = HttpUtility.ParseQueryString(uri.Query)
                                                     .Cast<string>()
                                                     .Select(key => new { key, value = HttpUtility.ParseQueryString(uri.Query)[key] })
                                                     .ToList();

                    return new
                    {
                        scheme = uri.Scheme.ToLower(System.Globalization.CultureInfo.CurrentCulture),
                        host = uri.Host.Trim(),
                        path = uri.AbsolutePath.Split('/')
                                              .Where(segment => !string.IsNullOrWhiteSpace(segment))
                                              .ToArray(),
                        query = queryCollection.Count != 0 ? queryCollection : null,
                    };
                }).ToList();

                var json = System.Text.Json.JsonSerializer.Serialize(transformedUris, JsonOptions);

                File.WriteAllText(this.path, json);

                this.logger?.LogInformation("Serialization completed successfully.");
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "An error occurred during serialization");
                throw new LogerExtensionException(ex.Message);
            }
        }
    }
}
