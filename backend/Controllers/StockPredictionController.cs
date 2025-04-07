using Microsoft.AspNetCore.Mvc;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockPredictionController : ControllerBase
    {
        private readonly ModelTrainingService _modelTrainingService;
        private readonly StockDataService _stockDataService;


        public StockPredictionController(ModelTrainingService modelTrainingService, StockDataService stockDataService)
        {
            _modelTrainingService = modelTrainingService;
            _stockDataService = stockDataService;

        }

        // GET api/StockPrediction/getPrediction?symbol=IBM
        [HttpGet("getPrediction")]
        public async Task<IActionResult> GetPrediction(string symbol)
        {
            try
            {
                var model = _modelTrainingService.GetTrainedModel();

                var stockData = await _modelTrainingService.GetStockData(symbol);
                if (stockData == null)
                {
                    return BadRequest("No stock data available.");
                }

                var prediction = _modelTrainingService.PredictStockClose(model, stockData);
                return Ok(prediction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("predict")]
        public async Task<IActionResult> PredictClose(string symbol)
        {
            var model = _modelTrainingService.GetTrainedModel();
            var latestStockData = await _modelTrainingService.GetStockData(symbol);

            var predictedClose = _modelTrainingService.PredictStockClose(model, latestStockData);

            // Get recent historical close prices for the chart
            var historicalData = await _stockDataService.FetchStockData(symbol, "5min");
            foreach (var d in historicalData)
            {
                Console.WriteLine($"Timestamp: {d.Timestamp}, Close: {d.Close}");
            }
            var historicalCloses = historicalData
                .Take(20)
                .Select(d => d.Close)
                .ToList();

            return Ok(new StockPredictionResult
            {
                PredictedClose = predictedClose,
                HistoricalClosePrices = historicalCloses
            });
        }


        // ✅ POST api/StockPrediction/train
        [HttpPost("train")]
        public IActionResult TrainModel()
        {
            try
            {
                _modelTrainingService.TrainAndSaveModelFromCsv();
                return Ok("Model trained and saved as StockModel.zip.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}