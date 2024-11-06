using Conversion;
using LogerExtensionDelegate;
using Microsoft.Extensions.Logging;
using Validation;

namespace UriConversion;

/// <summary>
/// The convertor class from string to Uri.
/// </summary>
public class UriConverter : IConverter<Uri?>
{
    private readonly IValidator<string>? validator;
    private readonly ILogger<UriConverter>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UriConverter"/> class.
    /// </summary>
    /// <param name="validator">The string validator.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Throw if validator is null.</exception>
    public UriConverter(IValidator<string>? validator, ILogger<UriConverter>? logger = default)
    {
        this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        this.logger = logger;
    }

    /// <summary>
    /// Converts the source string to Uri object.
    /// </summary>
    /// <param name="obj">The source string.</param>
    /// <returns> The Uri object if source string is valid and null otherwise.</returns>
    /// <exception cref="ArgumentNullException">Throw if source string is null.</exception>
    public Uri? Convert(string? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (this.validator.IsValid(obj))
        {
            try
            {
                var uri = new Uri(obj);
                return uri;
            }
            catch (UriFormatException)
            {
                throw new Exception();
            }
        }
        else
        {
            this.logger?.LogWarning($"Invalid URI string: {obj}");
        }

        return null;
    }
}
