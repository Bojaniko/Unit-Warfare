using System.Collections.Generic;

using Studio28.Probability;

using UnityEngine;

using UnitWarfare.Units;
using UnitWarfare.Core.Global;

namespace UnitWarfare.AI
{
    public class AiBrain
    {
        public record Memory(WeightedProbability<AiBrainFeature> Probability);
        public record Config(float Reduction, float Increasion, float Normalization);

        private readonly BrainFeature[] _defaultFeatures;
        private readonly Dictionary<AiBrainFeature, float> _defaultFactors;
        private readonly Dictionary<IUnit, Memory> _memory;

        private readonly Config _config;

        public AiBrain(BrainFeature[] features, Config config)
        {
            _config = new(Mathf.Clamp(config.Reduction, 0f, 1f),
                Mathf.Clamp(config.Increasion, 0f, 10f),
                Mathf.Clamp(config.Normalization, 0f, 1f));

            GenerateFeatureHandler();

            _memory = new();
            _defaultFeatures = features;
            _defaultFactors = new(_defaultFeatures.Length);
            foreach (BrainFeature f in _defaultFeatures)
                _defaultFactors.Add(f.Feature, f.Weight);
        }

        // ##### HANDLERS ##### \\

        private BrainFeatureHandler[] _featureHandlers;

        private void GenerateFeatureHandler()
        {
            List<BrainFeatureHandler> handlers = new();

            foreach (System.Type handlerType in typeof(BrainFeatureHandler).Assembly.GetTypes())
            {
                if (handlerType.IsSubclassOf(typeof(BrainFeatureHandler)))
                    handlers.Add((BrainFeatureHandler)System.Activator.CreateInstance(handlerType));
            }
            _featureHandlers = handlers.ToArray();
        }

        private BrainFeatureHandler.Outcome[] GetOutcomesForUnit(IUnit unit, IUnitCommand[] commands)
        {
            foreach (BrainFeatureHandler handler in _featureHandlers)
            {
                BrainFeatureHandler.Outcome[] outcomes = handler.GetOutcomes(unit, commands);
                if (outcomes.Length != 0)
                    return outcomes;
            }
            return new BrainFeatureHandler.Outcome[0];
        }

        // ##### OUTCOME ##### \\

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

            BrainFeatureHandler.Outcome[] outcomes = GetOutcomesForUnit(unit, commands);
            if (outcomes.Length == 0)
                return null;

            AiBrainFeature[] features = new AiBrainFeature[outcomes.Length];
            for (int i = 0; i < outcomes.Length; i++)
                features[i] = outcomes[i].Feature;

            AiBrainFeature finalOutcomeFeature = memory.Probability.GetOutcome(features);

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
            }
            else if (finalOutcomes.Count > 1)
            {
                int rand = Random.Range(0, finalOutcomes.Count);
                finalOutcome = finalOutcomes[rand];
            }

            if (finalOutcome != null)
            {
                UpdateMemory(memory, finalOutcome);
                Debug.Log($"Ai brain output feature is {finalOutcome.Feature} for {unit}.");
                Debug.Log($"Command outcome is {finalOutcome.Command}.");
                return finalOutcome.Command;
            }
            return null;
        }

        private void RefreshMemory(Memory memory)
        {
            foreach (BrainFeature feature in _defaultFeatures)
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

        private void UpdateMemory(Memory memory, BrainFeatureHandler.Outcome outcome)
        {
            RefreshMemory(memory);

            BrainFeature targetFeature = null;
            foreach (BrainFeature feature in _defaultFeatures)
            {
                if (feature.Feature.Equals(outcome))
                    targetFeature = feature;
            }
            if (targetFeature == null)
                return;

            float weight;
            switch (outcome.Mode)
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
            Memory newMemory = new(new(_defaultFactors));
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