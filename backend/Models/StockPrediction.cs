using Microsoft.ML.Data;

namespace backend.Models
{
    public class StockPrediction
    {
        [ColumnName("Score")]
        public float PredictedClose { get; set; }
    }
}
