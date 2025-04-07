namespace backend.Models
{
    public class FinnhubQuoteResponse
    {
        public float c { get; set; }  // Current price
        public float h { get; set; }  // High price
        public float l { get; set; }  // Low price
        public float o { get; set; }  // Open price
        public float pc { get; set; } // Previous close price
        public long t { get; set; }   // Timestamp (Unix)
    }
}
