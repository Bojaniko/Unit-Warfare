using System.Collections.Generic;

using Studio28.Probability;

using UnityEngine;

using UnitWarfare.Units;
using UnitWarfare.Core.Enums;

namespace UnitWarfare.AI
{
    public class AiBrain<FeatureHandler> where FeatureHandler : BrainFeatureHandler, new()
    {
        public record Memory(WeightedProbability<AiBrainFeature> Probability, List<IUnitCommand> CommandHistory);
        public record Config(float Reduction, float Increasion, float Normalization);

        private readonly List<BrainFeature> _features;

        private readonly Dictionary<IUnit, Memory> _memory;
        private readonly Dictionary<AiBrainFeature, float> _defaultFactors;

        private readonly Config _config;

        private readonly FeatureHandler _featureHandler;

        public AiBrain(BrainFeature[] features, Config config)
        {
            _config = new(Mathf.Clamp(config.Reduction, 0f, 1f),
                Mathf.Clamp(config.Increasion, 0f, 10f),
                Mathf.Clamp(config.Normalization, 0f, 1f));

            _featureHandler = new();

            _memory = new();

            _features = new(features);
            _defaultFactors = new();
            foreach (BrainFeature feature in _features)
                _defaultFactors.Add(feature.Feature, feature.Weight);
        }

        public IUnitCommand GetOutcome(IUnit unit, IUnitCommand[] commands)
        {
            if (unit == null)
                return null;
            if (commands.Length == 0)
                return null;
            if (commands.Length == 1)
                return commands[0];


            Memory memory;
            try
            {
                memory = RegisterUnitToMemory(unit);
            }
            catch
            {
                memory = _memory[unit];
            }

            BrainFeatureHandler.Outcome[] outcomes = _featureHandler.GetOutcomes(unit, commands);

            AiBrainFeature[] features = new AiBrainFeature[outcomes.Length];
            for (int i = 0; i < outcomes.Length; i++)
                features[i] = outcomes[i].Feature;

            AiBrainFeature finalOutcomeFeature;
            try
            {
                finalOutcomeFeature = memory.Probability.GetOutcome(features);
            }
            catch
            {
                foreach (AiBrainFeature f in features)
                    Debug.Log($"error feature is {f}");
                finalOutcomeFeature = AiBrainFeature.AGRESSIVE;
            }
            List<BrainFeatureHandler.Outcome> finalOutcomes = new();
            foreach (BrainFeatureHandler.Outcome outcome in outcomes)
            {
                if (outcome.Feature.Equals(finalOutcomeFeature))
                    finalOutcomes.Add(outcome);
            }

            BrainFeatureHandler.Outcome finalOutcome = null;
            if (finalOutcomes.Count == 1)
            {
                finalOutcome = finalOutcomes[0];
                Debug.Log($"Ai brain output feature is {finalOutcomes[0].Feature}");
            }
            else if (finalOutcomes.Count > 1)
            {
                int rand = Random.Range(0, finalOutcomes.Count);
                finalOutcome = finalOutcomes[rand];
                Debug.Log($"Ai brain output feature is {finalOutcomes[rand].Feature}");
            }

            if (finalOutcome != null)
            {
                UpdateMemory(memory, finalOutcome.Feature, finalOutcome.Mode);
                Debug.Log($"Command outcome is {finalOutcome.Command}.");
                return finalOutcome.Command;
            }
            return null;
        }

        private void RefreshMemory(Memory memory)
        {
            foreach (BrainFeature feature in _features.ToArray())
            {
                float current = memory.Probability.GetWeightValue(feature.Feature);
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
                memory.Probability.SetWeightFactor(feature.Feature, current);
            }
        }

        private void UpdateMemory(Memory memory, AiBrainFeature outcome , BrainFeatureHandler.Mode mode)
        {
            RefreshMemory(memory);

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
                    weight = memory.Probability.GetWeightValue(targetFeature.Feature);
                    if (weight >= targetFeature.Weight)
                    {
                        weight *= _config.Reduction;
                        memory.Probability.SetWeightFactor(targetFeature.Feature, weight);
                    }
                    break;

                case BrainFeatureHandler.Mode.INCREASE:
                    weight = memory.Probability.GetWeightValue(targetFeature.Feature);
                    if (weight <= targetFeature.Weight)
                    {
                        weight = Mathf.Clamp(weight + _config.Increasion, 0f, 10f);
                        memory.Probability.SetWeightFactor(targetFeature.Feature, weight);
                    }
                    break;

                case BrainFeatureHandler.Mode.MAXIMIZE:
                    memory.Probability.SetWeightFactor(targetFeature.Feature, 10f);
                    break;

                case BrainFeatureHandler.Mode.MINIMIZE:
                    memory.Probability.SetWeightFactor(targetFeature.Feature, 0f);
                    break;
            }
        }

        private Memory RegisterUnitToMemory(IUnit unit)
        {
            Memory newMemory = new(new(_defaultFactors), new());
            _memory.Add(unit, newMemory);
            unit.OnDestroy += ClearUnitFromMemory;
            return newMemory;
        }

        private void ClearUnitFromMemory(IUnit unit)
        {
            if (!_memory.ContainsKey(unit))
                return;
            _memory.Remove(unit);
            unit.OnDestroy -= ClearUnitFromMemory;
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