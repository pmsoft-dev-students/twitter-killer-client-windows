using Newtonsoft.Json;

namespace TwitterKiller
{
    public class JsonNetSerializer : ISerializer
    {
        public string Serialize(object toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }

        public T Deserialize<T>(string toDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(toDeserialize);
        }

        public T DeserializeAnonymous<T>(string toDeserialize, T obj)
        {
            return JsonConvert.DeserializeAnonymousType(toDeserialize, obj);
        }
    }
}