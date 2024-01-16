using System.Collections.Generic;

using Studio28.Probability;

using UnityEngine;

using UnitWarfare.Core.Enums;

namespace UnitWarfare.AI
{
    public class AiBrain
    {
        private readonly WeightedProbability<AiBrainFeature> _probability;
        private readonly List<BrainFeature> _features;
        private readonly List<BrainFeature> _reducedFeatures;

        private readonly float _reduction;
        private readonly float _normalization;

        public AiBrain(BrainFeature[] features, float reduction, float normalization)
        {
            _reduction = Mathf.Clamp(reduction, 0f, 1f);
            _normalization = Mathf.Clamp(normalization, 0f, 1f);

            _features = new(features);
            _reducedFeatures = new();
            Dictionary<AiBrainFeature, float> factors = new();
            foreach (BrainFeature feature in _features)
                factors.Add(feature.Feature, feature.Weight);
            _probability = new(factors);
        }

        public AiBrainFeature GetOutcome()
        {
            AiBrainFeature outcome = _probability.GetOutcome();

            NormalizeReducedFeatures();
            ReduceFeature(outcome);

            return outcome;
        }

        public void DisableFeature(AiBrainFeature feature)
        {
            foreach (BrainFeature f in _reducedFeatures.ToArray())
            {
                if (f.Feature.Equals(feature))
                {
                    _reducedFeatures.Remove(f);
                    break;
                }
            }
            _probability.SetWeightFactor(feature, 0);
        }

        public void EnableFeature(AiBrainFeature feature)
        {
            if (_probability.GetWeightValue(feature) != 0f)
                return;
            foreach (BrainFeature f in _features)
            {
                if (f.Feature.Equals(feature))
                {
                    _probability.SetWeightFactor(feature, f.Weight);
                    return;
                }
            }
        }

        private void NormalizeReducedFeatures()
        {
            if (_reducedFeatures.Count == 0)
                return;
            foreach (BrainFeature feature in _reducedFeatures.ToArray())
            {
                float current = _probability.GetWeightValue(feature.Feature);
                current += _normalization;
                if (current >= feature.Weight)
                {
                    current = feature.Weight;
                    _reducedFeatures.Remove(feature);
                }
                _probability.SetWeightFactor(feature.Feature, current);
            }
        }

        private void ReduceFeature(AiBrainFeature outcome)
        {
            BrainFeature targetFeature = null;
            foreach (BrainFeature feature in _features)
            {
                if (feature.Feature.Equals(outcome))
                    targetFeature = feature;
            }
            if (targetFeature != null && !_reducedFeatures.Contains(targetFeature))
            {
                _reducedFeatures.Add(targetFeature);
                _probability.SetWeightFactor(targetFeature.Feature, targetFeature.Weight * _reduction);
            }
        }
    }

    [System.Serializable]
    public class BrainFeature
    {
        [SerializeField] private AiBrainFeature _feature;
        public AiBrainFeature Feature => _feature;

        [SerializeField] private int _weight;
        public int Weight => _weight;

        public BrainFeature(AiBrainFeature feature, int weight)
        {
            _feature = feature;
            _weight = weight;
        }
    }
}