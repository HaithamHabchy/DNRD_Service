namespace DNRD_Service.Services.IServices
{
    public interface IUtility
    {
        T DeserializeFromJson<T>(string jsonString);
        object GetCache(string key);
        object GetKeyValue(dynamic dictionary, string path, object defaultValue = null);
        string SerializeToJson<T>(T obj);
        void SetCache(string key, object content, bool neverExpires = false);
    }
}