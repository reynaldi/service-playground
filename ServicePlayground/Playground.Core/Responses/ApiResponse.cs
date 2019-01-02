using Newtonsoft.Json;

namespace Playground.Core.Responses
{
    public class ApiResponse<T> where T : class
    {
        [JsonProperty(PropertyName = "isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty(PropertyName = "errorMessages")]
        public object ErrorMessages { get; set; }

        [JsonProperty(PropertyName = "data")]
        public T Data { get; set; }
    }
}
