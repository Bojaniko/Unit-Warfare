using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnitWarfare.Test
{
    public class SceneTransitionTest : MonoBehaviour
    {
        [SerializeField] private GameObject _testObject;

        public void Play()
        {
            DontDestroyOnLoad(_testObject);
            SceneManager.LoadScene("TestLevel");
            _testObject.transform.localScale = Vector3.one * 10f;
        }
    }
}