using Newtonsoft.Json;

namespace backend.Models
{
    public class FinnhubCandleResponse
    {
        [JsonProperty("c")]
        public float[] C { get; set; }  // Close prices

        [JsonProperty("h")]
        public float[] H { get; set; }  // High prices

        [JsonProperty("l")]
        public float[] L { get; set; }  // Low prices

        [JsonProperty("o")]
        public float[] O { get; set; }  // Open prices

        [JsonProperty("v")]
        public float[] V { get; set; }  // Volume

        [JsonProperty("t")]
        public long[] T { get; set; }   // Timestamps (Unix time)

        [JsonProperty("s")]
        public string S { get; set; }   // Status (should be "ok")
    }
}
