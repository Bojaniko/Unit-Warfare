using System.Collections.Generic;
using UnityEngine;

namespace Studio28.Probability
{
    public class WeightedProbability<WeightObject>
    {
        private readonly List<Weight> _weights;
        private int _weightSum;

        /// <summary>
        /// All factors should be between a range of 0 to 10,
        /// otherwise the factor will be set to a default of 5.
        /// </summary>
        public WeightedProbability(Dictionary<WeightObject, float> factors)
        {
            _weightSum = 0;
            _weights = new(factors.Count);
            foreach (WeightObject weight in factors.Keys)
            {
                float factor = factors[weight];
                if (factor > 10 || factor < 0) factor = 5;
                Weight w = new Weight(weight, factor);
                _weights.Add(new Weight(weight, factor));
                _weightSum += (int)(w.Probability);
            }
        }

        public float GetWeightValue(WeightObject weight)
        {
            foreach (Weight w in _weights)
            {
                if (w.Object.Equals(weight))
                    return w.Factor;
            }
            return -1;
        }

        /// <summary>
        /// A weight with a factor of 10 isn't promised to be returned.
        /// A weight with a factor of 0 wont be returned.
        /// </summary>
        public WeightObject GetOutcome()
        {
            int random = Random.Range(0, _weightSum - 1);

            int previousSum = 0;
            int currentSum = 0;

            foreach (Weight w in _weights)
            {
                if (w.Factor.Equals(0))
                    continue;
                currentSum += (int)w.Probability;
                if (random >= previousSum && random < currentSum)
                    return (WeightObject)w.Object;
                previousSum = currentSum;
            }
            throw new System.DataMisalignedException("This should not happen! Please contact support.");
        }

        /// <summary>
        /// A factor must be within a 0 to 10 range,
        /// otherwise the factor will be set to a default of 5.
        /// </summary>
        public void SetWeightFactor(WeightObject weight, float factor)
        {
            if (factor > 10)
                factor = 10;
            if (factor < 0)
                factor = 0;

            _weightSum = 0;

            foreach (Weight w in _weights)
            {
                if (w.Object.Equals(weight))
                {
                    w.SetFactor(factor);
                    return;
                }
                _weightSum += (int)(w.Probability);
            }
            Debug.LogError("Weight object not found.");
        }

        private class Weight
        {
            private readonly object _weight;
            public object Object => _weight;

            private float _factor;
            public float Factor => _factor;

            private float _probability;
            public float Probability => _probability;

            public Weight(object weight, float factor)
            {
                _weight = weight;
                SetFactor(factor);
            }

            public void SetFactor(float factor)
            {
                _factor = factor;
                _probability = _factor * 10;
            }
        }
    }
}