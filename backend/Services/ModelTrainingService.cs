using backend.Models;
using Microsoft.ML;

public class ModelTrainingService
{
   private readonly MLContext _mlContext;
    private readonly StockDataService _stockDataService;

    public ModelTrainingService(StockDataService stockDataService)
    {
        _mlContext = new MLContext();
        _stockDataService = stockDataService;
    }
    public ITransformer GetTrainedModel()
    {
        string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "StockModel.zip");

        if (!File.Exists(modelPath))
            throw new FileNotFoundException("Trained model not found at: " + modelPath);

        return _mlContext.Model.Load(modelPath, out var modelInputSchema);
    }

    public void TrainAndSaveModelFromCsv()
    {
        string csvPath = Path.Combine(Directory.GetCurrentDirectory(), "DownloadedCsv", "intraday_5min_IBM.csv");

        if (!File.Exists(csvPath))
            throw new FileNotFoundException("CSV file not found at: " + csvPath);

        var dataView = _mlContext.Data.LoadFromTextFile<StockData>(
            path: csvPath,
            hasHeader: true,
            separatorChar: ',');

       var pipeline = _mlContext.Transforms
    .Concatenate("Features", nameof(StockData.Open), nameof(StockData.High), nameof(StockData.Low), nameof(StockData.Volume))
    .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
    .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Close"));


        var model = pipeline.Fit(dataView);

        _mlContext.Model.Save(model, dataView.Schema, "StockModel.zip");
    }


public async Task<StockData> GetStockData(string symbol)
{
    var stockData = await _stockDataService.FetchStockData(symbol, "5min");

    if (stockData == null || stockData.Length == 0)
        throw new Exception("No stock data received for prediction.");

    // 🧪 Debugging: log the first few entries
    foreach (var entry in stockData.Take(5))
    {
        Console.WriteLine($"Timestamp: {entry.Timestamp}, Close: {entry.Close}");
    }

    return stockData[0];
}


    public float PredictStockClose(ITransformer model, StockData stockData)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<StockData, StockPrediction>(model);
        var prediction = predictionEngine.Predict(stockData);
        return prediction.PredictedClose;
    }
}
