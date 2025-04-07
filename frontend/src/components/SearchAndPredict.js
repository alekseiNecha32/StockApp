import React, { useState } from 'react';
import { Container, TextField, Button, Typography, Box } from '@mui/material';
import { Line } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  Title,
  Tooltip,
  Legend
} from 'chart.js';

ChartJS.register(LineElement, PointElement, LinearScale, CategoryScale, Title, Tooltip, Legend);

const SearchAndPredict = () => {
  const [symbol, setSymbol] = useState('');
  const [predictionResult, setPredictionResult] = useState(null);
  const [error, setError] = useState(null);

  const handlePredict = async () => {
    try {
      const response = await fetch(`http://localhost:5225/api/StockPrediction/predict?symbol=${symbol}`);
      const text = await response.text();

      if (!response.ok) {
        setPredictionResult(null);
        setError("Error: " + text);
        return;
      }

      const result = JSON.parse(text);
      setPredictionResult(result);
      setError(null);
    } catch (err) {
      setPredictionResult(null);
      setError("Error: " + err.message);
    }
  };

  const chartData = predictionResult && {
    labels: [
      ...predictionResult.historicalClosePrices.map((_, i) => `T-${predictionResult.historicalClosePrices.length - i}`),
      'Predicted'
    ],
    datasets: [
      {
        label: 'Close Prices',
        data: predictionResult.historicalClosePrices,
        borderColor: 'blue',
        backgroundColor: 'blue',
        tension: 0.4,
        fill: false,
      },
      {
        label: 'Predicted Close',
        data: [
          ...Array(predictionResult.historicalClosePrices.length).fill(null),
          predictionResult.predictedClose
        ],
        borderColor: 'green',
        backgroundColor: 'green',
        borderDash: [5, 5],
        pointRadius: 5,
        tension: 0.4,
        fill: false,
      },
    ]
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Typography variant="h4" fontWeight="bold" gutterBottom>
        Stock Prediction Web App
      </Typography>
      <Typography variant="subtitle1" gutterBottom>
        Select stock symbol
      </Typography>

      <Box display="flex" gap={2} mb={2}>
        <TextField
          fullWidth
          label="e.g. IBM"
          value={symbol}
          onChange={(e) => setSymbol(e.target.value.toUpperCase())}
          variant="outlined"
        />
        <Button variant="contained" onClick={handlePredict}>
          Predict
        </Button>
      </Box>

      {predictionResult && (
        <>
          <Typography variant="h6" color="success.main">
            Predicted Close Price: ${predictionResult.predictedClose.toFixed(2)}
          </Typography>
          <Typography variant="subtitle1" gutterBottom>
            Recent Close Prices: {predictionResult.historicalClosePrices.join(', ')}
          </Typography>
        </>
      )}

      {error && (
        <Typography color="error">
          {error}
        </Typography>
      )}

      {predictionResult && (
        <Box mt={4}>
          <Line data={chartData} />
        </Box>
      )}
    </Container>
  );
};

export default SearchAndPredict;
