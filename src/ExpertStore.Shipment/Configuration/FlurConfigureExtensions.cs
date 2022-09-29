using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace ExpertStore.Shipment.Configuration;

public class JsonNetSerializer : ISerializer
{
    public T Deserialize<T>(string s) => JsonConvert.DeserializeObject<T>(s, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

    public T Deserialize<T>(Stream stream) => throw new Exception("N/A");

    public string Serialize(object obj) => JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
}

public static class FlurConfigureExtensions
{
    public static void ConfigureFlur(this IServiceCollection services)
        => FlurlHttp.Configure(settings => { 
            settings.JsonSerializer = new JsonNetSerializer(); });
}
