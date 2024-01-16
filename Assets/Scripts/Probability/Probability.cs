using System.Collections.Generic;
using UnityEngine;

namespace Studio28.Probability
{
    public static class Probability
    {
        /// <summary>
        /// A percentage based probability.
        /// </summary>
        /// <param name="probability">The probability percentage from 0 to 100.</param>
        /// <returns>true if probable.</returns>
        public static bool Percentage(int probability)
        {
            if (probability >= 100)
                return true;

            if (probability <= 0)
                return false;

            int number = Random.Range(1, 100);
            return (probability <= number);
        }

        /// <summary>
        /// A percentage based probability where the probability is normalized
        /// between a float value of 0 and 1.
        /// </summary>
        /// <param name="probability">The probability percentage from 0f to 1f.</param>
        /// <returns>true if probable.</returns>
        public static bool PercentageNormalized(float probability)
        {
            if (probability >= 1.0f)
                return true;

            if (probability <= 0.0f)
                return false;

            int number = Random.Range(1, 100);
            return (probability <= number / 100f);
        }
    }
}