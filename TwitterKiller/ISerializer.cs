namespace TwitterKiller
{
    public interface ISerializer
    {
        string Serialize(object toSerialize);
        T Deserialize<T>(string toDeserialize);
        T DeserializeAnonymous<T>(string toDeserialize, T obj);
    }
}