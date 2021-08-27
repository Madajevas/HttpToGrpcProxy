using RestSharp;
using RestSharp.Deserializers;

namespace HttpToGrpcProxy.Tests.Deserializers
{
    class TextPlainDeserializer : IDeserializer
    {
        public T? Deserialize<T>(IRestResponse response)
        {
            return (T)(dynamic)response.Content;
        }
    }
}
