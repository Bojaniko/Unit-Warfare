using UnityEngine;
using UnityEngine.UI;

using UnitWarfare.Core.Enums;

namespace UnitWarfare.UI
{
    public class UnitCommandsHandler : MonoBehaviour, IUserInterfaceHandler
    {
        public delegate void UnitCommandsEventHandler(ActiveCommandOrder command);

        public event UnitCommandsEventHandler OnCommand;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform current = transform.GetChild(i);
                switch (current.name)
                {
                    case "ATTACK":
                        current.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { SendCommand(ActiveCommandOrder.ATTACK); });
                        break;

                    case "JOIN":
                        current.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { SendCommand(ActiveCommandOrder.JOIN); });
                        break;

                    case "MOVE":
                        current.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { SendCommand(ActiveCommandOrder.MOVE); });
                        break;

                    case "CANCEL":
                        current.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { SendCommand(ActiveCommandOrder.CANCEL); });
                        break;
                }
            }

            gameObject.SetActive(false);
        }

        public bool IsShowing => gameObject.activeSelf;

        private void SendCommand(ActiveCommandOrder command)
        {
            OnCommand?.Invoke(command);
            gameObject.SetActive(false);
        }

        public void EnableHandler(Vector2 position)
        {
            gameObject.transform.position = position;
            gameObject.SetActive(true);
        }

        public void DisableHandler()
        {
            gameObject.SetActive(false);
        }
    }
}