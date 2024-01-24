using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Studio28.Probability
{
    public class WeightedProbability<WeightObject>
    {
        private readonly WeightList _weights;

        /// <summary>
        /// All factors should be between a range of 0 to 10,
        /// otherwise the factor will be set to a default of 5.
        /// </summary>
        public WeightedProbability(Dictionary<WeightObject, float> factors)
        {
            _weights = new(factors.Count);
            foreach (WeightObject weight in factors.Keys)
            {
                float factor = factors[weight];
                if (factor > 10 || factor < 0) factor = 5;
                _weights.Add(new Weight(weight, factor));
            }
        }

        public float GetWeightValue(WeightObject weight)
        {
            Weight w = _weights[weight];
            if (w != null)
                return w.Factor;
            return -1f;
        }

        /// <summary>
        /// A weight object with a factor of 10 isn't promised to be returned.
        /// A weight object with a factor of 0 wont be returned.
        /// </summary>
        public WeightObject GetOutcome()
        {
            int sum = 0;
            foreach (Weight w in _weights)
                sum += (int)w.Probability;
            return GetOutcome(_weights.ToArray(), sum);
        }

        /// <summary>
        /// A weight object with a factor of 10 isn't promised to be returned.
        /// A weight object with a factor of 0 wont be returned.
        /// </summary>
        public WeightObject GetOutcome(params WeightObject[] weights)
        {
            if (weights.Length == 0)
                throw new System.ArgumentException($"Outcome requires at least one {typeof(WeightObject)} input.");
            List<Weight> list_weights = new();
            int sum = 0;
            foreach (WeightObject wo in weights)
            {
                Weight w = _weights[wo];
                if (w == null)
                    continue;
                list_weights.Add(_weights[wo]);
                sum += (int)w.Probability;
            }
            return GetOutcome(list_weights.ToArray(), sum);
        }

        private WeightObject GetOutcome(Weight[] weights, int range)
        {
            int random = Random.Range(0, range - 1);
            int previousSum = 0;
            int currentSum = 0;

            foreach (Weight w in weights)
            {
                if (w.Factor.Equals(0))
                    continue;
                currentSum += (int)w.Probability;
                if (currentSum == previousSum)
                    continue;
                if (random >= previousSum && random < currentSum)
                    return (WeightObject)w.Object;
                previousSum = currentSum;
            }
            return (WeightObject)weights[0].Object;
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

            _weights[weight].SetFactor(factor);
        }

        private sealed class WeightList : ICollection<Weight>
        {
            public WeightList()
            {
                _innerList = new();
            }

            public WeightList(int size)
            {
                _innerList = new(size);
            }

            private readonly List<Weight> _innerList;

            public int Count => _innerList.Count;
            public bool IsReadOnly => false;

            public void Add(Weight item)
            {
                if (_innerList.Contains(item))
                    return;
                foreach (Weight w in _innerList)
                {
                    if (w.Object.Equals(item.Object))
                        return;
                }
                _innerList.Add(item);
            }
            
            public bool ContainsWeightObject(object o)
            {
                foreach (Weight w in _innerList)
                {
                    if (w.Object.Equals(o))
                        return true;
                }
                return false;
            }

            public Weight this[WeightObject wo]
            {
                get
                {
                    foreach (Weight w in _innerList)
                    {
                        if (w.Object.Equals(wo))
                            return w;
                    }
                    return null;
                }
            }

            public Weight this[int index]
            {
                get
                {
                    return _innerList[index];
                }
            }

            public Weight[] ToArray() =>
                _innerList.ToArray();

            public bool Contains(Weight item) =>
                _innerList.Contains(item);

            public bool Remove(Weight item) =>
                _innerList.Remove(item);

            public void Clear() =>
                _innerList.Clear();

            public void CopyTo(Weight[] array, int arrayIndex) { }

            public IEnumerator<Weight> GetEnumerator() =>
                _innerList.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                _innerList.GetEnumerator();
        }

        private sealed class Weight : System.IComparable<Weight>
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

            public static bool operator ==(Weight a, Weight b)
            {
                if ((object)a == null && (object)b == null)
                    return true;
                if ((object)a != null && (object)b != null && a.Object == b.Object)
                    return true;
                return false;
            }
            public static bool operator !=(Weight a, Weight b) => !(a == b);

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (!obj.GetType().Equals(typeof(Weight)))
                    return false;
                return Object.Equals(((Weight)obj).Object);
            }

            public override int GetHashCode() =>
                Object.GetHashCode();

            public int CompareTo(Weight other) =>
                Factor.CompareTo(other.Factor);
        }
    }
}