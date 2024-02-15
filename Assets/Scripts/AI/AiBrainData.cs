using System.Collections.Generic;

using UnityEngine;

using UnitWarfare.Core.Global;


namespace UnitWarfare.AI
{
    [CreateAssetMenu(menuName = "AI/Brain")]
    public class AiBrainData : ScriptableObject
    {
        [SerializeField] private string _displayName = "Bot";
        public string DisplayName => _displayName;

        [SerializeField] private BrainFeature[] _features;
        public BrainFeature[] BrainFeatures => _features;

        [SerializeField, Range(1f, 0f)] private float _reductionFactor = 0.5f;
        public float ReductionFactor => 1f - _reductionFactor;

        [SerializeField, Range(0f, 10f)] private float _increasionAmount = 0.5f;
        public float IncreasionAmount => _increasionAmount;

        [SerializeField, Range(0f, 5f)] private float _normalizationStep = 2.5f;
        public float NormalizationStep => _normalizationStep;

        private void Awake()
        {
            if (_features == null)
            {
                System.Array values = System.Enum.GetValues(typeof(AiBrainFeature));
                _features = new BrainFeature[values.Length];

                for(int i = 0; i < values.Length; i++)
                {
                    _features[i] = new BrainFeature((AiBrainFeature)values.GetValue(i), 5);
                }
                return;
            }
            else
            {
                System.Array values = System.Enum.GetValues(typeof(AiBrainFeature));
                if (_features.Length == values.Length)
                    return;
                List<BrainFeature> features = new(_features);
                for (int i = 0; i < values.Length; i++)
                {
                    bool hasValue = false;
                    foreach (BrainFeature feature in features)
                    {
                        if (feature.Feature.Equals((AiBrainFeature)values.GetValue(i)))
                        {
                            hasValue = true;
                            break;
                        }
                    }
                    if (!hasValue)
                    {
                        BrainFeature feature = new((AiBrainFeature)values.GetValue(i), 5);
                        features.Add(feature);
                        _features = features.ToArray();
                    }
                }
            }
        }
    }
}