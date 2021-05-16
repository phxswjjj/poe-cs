using Microsoft.Extensions.ObjectPool;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace poe.lib.ML
{
    public class MLModelEngine<TSrc, TDst>
        where TSrc : class
        where TDst : class, new()
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _mlModel;
        private readonly ObjectPool<PredictionEngine<TSrc, TDst>> _predictionEnginePool;
        private readonly int _maxObjectsRetained;

        public ITransformer MLModel => _mlModel;
        public PredictionEngine<TSrc, TDst> PredictionEngine => _predictionEnginePool.Get();

        public MLModelEngine(string modelFilePathName, int maxObjectsRetained = -1)
        {
            //Create the MLContext object to use under the scope of this class 
            _mlContext = new MLContext();

            //Load the ProductSalesForecast model from the .ZIP file
            using (var fileStream = File.OpenRead(modelFilePathName))
            {
                _mlModel = _mlContext.Model.Load(fileStream, out _);
            }

            _maxObjectsRetained = maxObjectsRetained;

            //Create PredictionEngine Object Pool
            _predictionEnginePool = CreatePredictionEngineObjectPool();
        }

        private ObjectPool<PredictionEngine<TSrc, TDst>> CreatePredictionEngineObjectPool()
        {
            var predEnginePolicy = new PooledPredictionEnginePolicy<TSrc, TDst>(_mlContext, _mlModel);

            DefaultObjectPool<PredictionEngine<TSrc, TDst>> pool;

            if (_maxObjectsRetained != -1)
            {
                pool = new DefaultObjectPool<PredictionEngine<TSrc, TDst>>(predEnginePolicy, _maxObjectsRetained);
            }
            else
            {
                //default maximumRetained is Environment.ProcessorCount * 2, if not explicitly provided
                pool = new DefaultObjectPool<PredictionEngine<TSrc, TDst>>(predEnginePolicy);
            }

            return pool;
        }
        public TDst Predict(TSrc dataSample)
        {
            //Get PredictionEngine object from the Object Pool
            PredictionEngine<TSrc, TDst> predictionEngine = _predictionEnginePool.Get();

            try
            {
                //Predict
                TDst prediction = predictionEngine.Predict(dataSample);
                return prediction;
            }
            finally
            {
                //Release used PredictionEngine object into the Object Pool
                _predictionEnginePool.Return(predictionEngine);
            }
        }
    }
}
