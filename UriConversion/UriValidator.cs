using Microsoft.Extensions.Logging;
using Validation;

namespace UriConversion;

/// <summary>
/// Uri string validator.
/// </summary>
public class UriValidator : IValidator<string>
{
    private static readonly Action<ILogger, string, Exception?> LogUri =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(UriValidator)),
            "Invalid URI: {Uri}");

    private readonly ILogger<UriValidator>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UriValidator"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public UriValidator(ILogger<UriValidator>? logger = default)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Determines if a string is valid Uri.
    /// </summary>
    /// <param name="obj">The source string.</param>
    /// <returns>true if the uri string is valid; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Throw if source string is null.</exception>
    public bool IsValid(string? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        bool isValid = Uri.TryCreate(obj, UriKind.Absolute, out Uri? validatedUri) &&
                           (validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps);

        if (this.logger != null && !isValid)
        {
            LogUri(this.logger, obj, null);
        }

        return isValid;
    }
}
