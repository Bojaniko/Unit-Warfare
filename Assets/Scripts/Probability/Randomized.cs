using System.Collections.Generic;
using UnityEngine;

namespace Studio28.Probability
{
    /// <summary>
    /// A more randomzied probability. Since computers don't actually generate random numbers
    /// this class generates a list of random numbers in an amount equal to the probability percentage,
    /// then when returning an outcome generates a new number and checks if it equals to any
    /// of the previously generated numbers in the list, returning true if it does.
    /// </summary>
    public class Randomized
    {
        private int _previousValue;
        private readonly List<int> _probabilityValues;

        /// <summary>
        /// This generates a probability amount of random numbers from 1 to 99.
        /// </summary>
        /// <param name="probability">A value between 1 and 99.</param>
        public Randomized(int probability)
        {
            probability = Mathf.Clamp(probability, 1, 99);
            _previousValue = 0;
            _probabilityValues = new(probability);
            _probabilityValues.AddRange(GenerateProbabilityValues(probability));
        }

        /// <summary>
        /// The outcome of the probability.
        /// </summary>
        /// <returns>true if the array of random numbers contains
        /// a generated random number from 1 to 99.</returns>
        public bool Outcome()
        {
            int probability = GenerateUniqueProbability(_previousValue);
            _previousValue = probability;
            return (_probabilityValues.Contains(probability));
        }

        private int GenerateUniqueProbability(int previous_probability)
        {
            int probability = Random.Range(1, 100);
            if (probability == previous_probability)
                probability = GenerateUniqueProbability(previous_probability);
            return probability;
        }

        private int[] GenerateProbabilityValues(int probability)
        {
            List<int> probabilities = new(probability);
            for (int i = 0; i < probability; i++)
            {
                probabilities.Add(GenerateUniqueNumber(probabilities));
            }
            return probabilities.ToArray();
        }

        private int GenerateUniqueNumber(List<int> numbers)
        {
            int number = Random.Range(1, 100);
            if (numbers.Contains(number))
                number = GenerateUniqueNumber(numbers);
            return number;
        }
    }
}
