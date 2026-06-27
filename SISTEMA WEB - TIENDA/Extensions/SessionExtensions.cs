using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace SISTEMA_WEB___TIENDA.Extensions;

public static class SessionExtensions
{
    // Guarda cualquier objeto complejo convirtiéndolo a JSON string
    public static void SetObject(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    // Recupera el objeto complejo leyendo el JSON string
    public static T? GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}