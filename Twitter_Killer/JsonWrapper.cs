using Newtonsoft.Json;

namespace Twitter_Killer
{
    public class JsonWrapper : IJsonWrapper
    {
        public string GetValueByKey(string jsonContent, string key)
        {
            dynamic deserializedObject = JsonConvert.DeserializeObject(jsonContent);
            return (string) deserializedObject[key];
        }
    }
}