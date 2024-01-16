using System.Collections;

using UnityEngine;

namespace UnitWarfare.Core
{
    public delegate void EncapsulatedMBEventHandler();

    public class EncapsulatedMonoBehaviour
    {
        private readonly EMB _monoBehaviour;

        public Transform transform => _monoBehaviour.transform;

        public GameObject gameObject => _monoBehaviour.gameObject;

        public Coroutine StartCoroutine(IEnumerator routine) =>
            _monoBehaviour.StartCoroutine(routine);

        public void StopCoroutine(Coroutine coroutine) =>
            _monoBehaviour.StopCoroutine(coroutine);

        public void Destroy()
        {
            OnUpdate = null;
            GameObject.Destroy(_monoBehaviour.gameObject);
        }

        public void DestroyComponent()
        {
            OnUpdate = null;
            GameObject.Destroy(_monoBehaviour);
        }

        public EncapsulatedMonoBehaviour(GameObject game_object)
        {
            _monoBehaviour = game_object.AddComponent<EMB>();
            _monoBehaviour.SetEncapsulator(this);
        }

        public event EncapsulatedMBEventHandler OnUpdate;

        public class EMB : MonoBehaviour
        {
            private EncapsulatedMonoBehaviour _encapsulator;
            public EncapsulatedMonoBehaviour Encapsulator => _encapsulator;
            public void SetEncapsulator(EncapsulatedMonoBehaviour encapsulator)
            {
                if (_encapsulator != null)
                    return;
                _encapsulator = encapsulator;
            }

            private void Update()
            {
                if (_encapsulator == null)
                    return;
                _encapsulator.OnUpdate?.Invoke();
            }
        }
    }
}