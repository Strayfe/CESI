using Newtonsoft.Json;

namespace CESI.ResponseModels
{
    public class CesiResponse<T>
    {
        public string? JsonString { get; set; }
        public T Model => JsonConvert.DeserializeObject<T>(JsonString ?? "");
    }
}