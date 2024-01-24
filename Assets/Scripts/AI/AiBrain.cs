using System.Collections.Generic;

using Studio28.Probability;

using UnityEngine;

using UnitWarfare.Units;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.AI
{
    public class AiBrain<FeatureHandler> where FeatureHandler : BrainFeatureHandler, new()
    {
        public record Config(float Reduction, float Increasion, float Normalization);

        private readonly WeightedProbability<AiBrainFeature> _probability;
        private readonly List<BrainFeature> _features;


        private readonly Config _config;

        private readonly FeatureHandler _featureHandler;

        public AiBrain(BrainFeature[] features, Config config)
        {
            _config = new(Mathf.Clamp(config.Reduction, 0f, 1f),
                Mathf.Clamp(config.Increasion, 0f, 10f),
                Mathf.Clamp(config.Normalization, 0f, 1f));

            _featureHandler = new();

            _features = new(features);
            Dictionary<AiBrainFeature, float> factors = new();
            foreach (BrainFeature feature in _features)
                factors.Add(feature.Feature, feature.Weight);
            _probability = new(factors);
        }

        public IUnitCommand GetOutcome(IUnit unit, IUnitCommand[] commands)
        {
            if (unit == null)
                return null;
            if (commands.Length == 0)
                return null;
            if (commands.Length == 1)
                return commands[0];

            IUnitCommand commandOutcome = null;

            BrainFeatureHandler.Outcome[] outcomes = _featureHandler.GetOutcomes(unit, commands);

            AiBrainFeature[] features = new AiBrainFeature[outcomes.Length];
            for (int i = 0; i < outcomes.Length; i++)
                features[i] = outcomes[i].Feature;

            AiBrainFeature finalOutcomeFeature = _probability.GetOutcome(features);
            List<BrainFeatureHandler.Outcome> finalOutcomes = new();
            foreach (BrainFeatureHandler.Outcome outcome in outcomes)
            {
                if (outcome.Feature.Equals(finalOutcomeFeature))
                    finalOutcomes.Add(outcome);
            }

            if (finalOutcomes.Count == 1)
            {
                commandOutcome = finalOutcomes[0].Command;
                Debug.Log($"Ai brain output feature is {finalOutcomes[0].Feature}");
            }
            else
            {
                int rand = Random.Range(0, finalOutcomes.Count);
                commandOutcome = finalOutcomes[rand].Command;
                Debug.Log($"Ai brain output feature is {finalOutcomes[rand].Feature}");
            }

            if (commandOutcome != null)
            {
                ModifyFeatureProbability(finalOutcomes[0].Feature, finalOutcomes[0].Mode);
                NormalizeFeatures();
                Debug.Log($"Command outcome is {commandOutcome.ToString()}.");
                return commandOutcome;
            }
            NormalizeFeatures();
            return commands[0];
        }

        private void NormalizeFeatures()
        {
            foreach (BrainFeature feature in _features.ToArray())
            {
                float current = _probability.GetWeightValue(feature.Feature);
                if (current < feature.Weight)
                {
                    current += _config.Normalization;
                    if (current > feature.Weight)
                        current = feature.Weight;
                }
                else if (current > feature.Weight)
                {
                    current -= _config.Normalization;
                    if (current < feature.Weight)
                        current = feature.Weight;
                }
                _probability.SetWeightFactor(feature.Feature, current);
            }
        }

        private void ModifyFeatureProbability(AiBrainFeature outcome, BrainFeatureHandler.Mode mode)
        {
            BrainFeature targetFeature = null;
            foreach (BrainFeature feature in _features)
            {
                if (feature.Feature.Equals(outcome))
                    targetFeature = feature;
            }
            if (targetFeature == null)
                return;

            float weight;
            switch (mode)
            {
                case BrainFeatureHandler.Mode.REDUCE:
                    weight = _probability.GetWeightValue(targetFeature.Feature);
                    if (weight >= targetFeature.Weight)
                    {
                        weight *= _config.Reduction;
                        _probability.SetWeightFactor(targetFeature.Feature, weight);
                    }
                    break;

                case BrainFeatureHandler.Mode.INCREASE:
                    weight = _probability.GetWeightValue(targetFeature.Feature);
                    if (weight <= targetFeature.Weight)
                    {
                        weight = Mathf.Clamp(weight + _config.Increasion, 0f, 10f);
                        _probability.SetWeightFactor(targetFeature.Feature, weight);
                    }
                    break;

                case BrainFeatureHandler.Mode.MAXIMIZE:
                    _probability.SetWeightFactor(targetFeature.Feature, 10f);
                    break;

                case BrainFeatureHandler.Mode.MINIMIZE:
                    _probability.SetWeightFactor(targetFeature.Feature, 0f);
                    break;
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