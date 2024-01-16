using System.Collections.Generic;
using UnityEngine;

namespace Studio28.Probability
{
    /// <summary>
    /// A Weighted Probability is a simple probability.
    /// </summary>
    /// <typeparam name="WeightObject">The key object type for pairing weights.</typeparam>
    public class Weighted<WeightObject>
    {
        private readonly int _probabilitySum;
        private readonly Dictionary<WeightObject, int> _probabilities;

        /// <summary>
        /// Creates a Simple Probability that generates a successul probability
        /// based on WeightObject key-pairing with a whole number from 1 to 10.
        /// </summary>
        /// <param name="probabilities">A key which is any object to be probable,
        /// and an integer from 1 to 10 as a probability weight.</param>
        public Weighted(Dictionary<WeightObject, int> probabilities)
        {
            if (probabilities.Count <= 1)
                throw new System.ArgumentException("There should not be less than 2 probability weights.");

            _probabilities = probabilities;

            _probabilitySum = GetProbabilitySum(_probabilities);
        }

        /// <summary>
        /// The outcome of the weighted probability.
        /// </summary>
        /// <returns>a random object based on it's paired weight.</returns>
        public WeightObject Outcome()
        {
            int probability = Random.Range(0, _probabilitySum);

            int count = 0;
            foreach (WeightObject weight in _probabilities.Keys)
            {
                count += _probabilities[weight];
                if (probability < count)
                    return weight;
            }
            throw new System.DataMisalignedException("This should not happen! Please contact support.");
        }

        private int GetProbabilitySum(Dictionary<WeightObject, int> probabilities)
        {
            int sum = 0;
            foreach (int count in probabilities.Values)
            {
                sum += count;
            }
            return sum;
        }
    }
}