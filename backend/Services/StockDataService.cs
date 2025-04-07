using backend.Models;
using Newtonsoft.Json;

public class StockDataService
{
    private readonly string _apiKey;

    public StockDataService(string apiKey)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    public async Task<StockData[]> FetchStockData(string symbol, string resolution) // resolution is ignored now
    {
        var url = $"https://finnhub.io/api/v1/quote?symbol={symbol}&token={_apiKey}";

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var quote = JsonConvert.DeserializeObject<FinnhubQuoteResponse>(json);

            if (quote == null || quote.c == 0)
                throw new Exception("No data found in the response.");

            var data = new StockData
            {
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(quote.t).DateTime,
                Open = quote.o,
                High = quote.h,
                Low = quote.l,
                Close = quote.c,
                Volume = 0 // Volume isn't provided in this endpoint
            };

            return new[] { data };
        }
    }
}
