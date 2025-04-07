namespace backend.Models
{
    public class StockPredictionResult
    {
        public float PredictedClose { get; set; }
        public List<float>? HistoricalClosePrices { get; set; }
    }
}
