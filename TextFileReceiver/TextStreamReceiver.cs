using DataReceiving;
using Microsoft.Extensions.Logging;

namespace TextFileReceiver;

/// <summary>
/// The data receiver from text file.
/// </summary>
public class TextStreamReceiver : IDataReceiver
{
    private readonly string? path;
    private readonly ILogger<TextStreamReceiver>? logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextStreamReceiver"/> class.
    /// </summary>
    /// <param name="path">The path to text file.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentException">Throw if text reader is null or empty.</exception>
    public TextStreamReceiver(string? path, ILogger<TextStreamReceiver>? logger = default)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));
        this.logger = logger;
    }

    /// <summary>
    /// Receives lines from text reader.
    /// </summary>
    /// <returns>Strings.</returns>
    public IEnumerable<string> Receive()
    {
        if (!File.Exists(this.path))
        {
            this.logger?.LogError("File not found: {FilePath}", this.path);
            throw new FileNotFoundException("The specified file was not found.", this.path);
        }

        var lines = new List<string>();

        this.logger?.LogInformation("Reading lines from file: {FilePath}", this.path);
        using (var reader = new StreamReader(this.path))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
        }

        return lines;
    }
}
