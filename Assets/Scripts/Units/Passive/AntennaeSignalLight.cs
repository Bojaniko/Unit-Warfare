using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitWarfare.Units
{
    public class AntennaeSignalLight : MonoBehaviour
    {
        private Camera c_camera;
        private SpriteRenderer c_sprite;

        [SerializeField] private Color _activeColor = Color.green;
        [SerializeField] private Color _inactiveColor = Color.red;

        private void Awake()
        {
            c_camera = Camera.main;
            c_sprite = GetComponent<SpriteRenderer>();
            c_sprite.color = _inactiveColor;
        }

        private void Update()
        {
            transform.LookAt(c_camera.transform);
        }

        public void EnableSignal()
        {
            c_sprite.color = _activeColor;
        }

        public void DisableSignal()
        {
            c_sprite.color = _inactiveColor;
        }
    }
}