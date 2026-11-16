using Newtonsoft.Json;

public static class JsonSettings
{
    public static readonly JsonSerializerSettings Settings =
        new()
        {
            TypeNameHandling = TypeNameHandling.All
        };
}