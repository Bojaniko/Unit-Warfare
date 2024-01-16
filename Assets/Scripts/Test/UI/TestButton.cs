using UnityEngine;
using UnityEngine.UI;

namespace UnitWarfare.Test
{
    [RequireComponent(typeof(Button))]
    public class TestButton : MonoBehaviour
    {
        [SerializeField] private string _message = "Test";

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => { Debug.Log(_message); });
        }
    }
}
