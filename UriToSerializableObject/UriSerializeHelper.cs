namespace UriSerializationHelper;

public static class UriSerializeHelper
{
    public static UriAddress ToSerializableObject(this Uri address)
    {
        UriAddress obj = new UriAddress
        {
            Scheme = address.Scheme,
            Host = address.Host,
            Path = address.Segments
                .Where(line => line.Length > 1)
                .Select(line => line.TrimEnd('/'))
                .ToList(),
            Query = address.Query.TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(query =>
                {
                    var elements = query.Split('=');
                    return new QueryElement { Key = elements[0], Value = elements[1] };
                })
                .ToList(),
        };

        return obj;
    }

}
