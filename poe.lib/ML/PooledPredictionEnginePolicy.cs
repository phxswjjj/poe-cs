using Microsoft.Extensions.ObjectPool;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace poe.lib.ML
{
    public class PooledPredictionEnginePolicy<TSrc, TDst> : IPooledObjectPolicy<PredictionEngine<TSrc, TDst>>
        where TSrc : class
        where TDst : class, new()
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        public PooledPredictionEnginePolicy(MLContext mlContext, ITransformer model)
        {
            _mlContext = mlContext;
            _model = model;
        }

        public PredictionEngine<TSrc, TDst> Create()
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<TSrc, TDst>(_model);
            return predictionEngine;
        }

        public bool Return(PredictionEngine<TSrc, TDst> obj)
        {
            if (obj == null)
                return false;

            return true;
        }
    }
}
