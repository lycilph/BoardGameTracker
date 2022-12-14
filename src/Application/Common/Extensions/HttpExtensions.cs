using Polly;
using System.Text;
using System.Xml.Serialization;

namespace BoardGameTracker.Application.Common.Extensions;

public static class HttpExtensions
{
    public static async Task<T?> GetFromXmlAsync<T>(this HttpClient client, string str)
    {
        var request = GetRequestWithContext(str);
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
        {
            var serializer = new XmlSerializer(typeof(T));
            if (serializer.Deserialize(stream) is T dto)
            {
                return dto;
            }
        }

        return default;
    }

    public static HttpRequestMessage GetRequestWithContext(string str)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, str);
        request.SetPolicyExecutionContext(new Context(str));
        return request;
    }
}
