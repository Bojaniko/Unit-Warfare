using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using UnitWarfare.Core;
using UnitWarfare.Input;

namespace UnitWarfare.UI
{
    public class UIHandler : GameHandler
    {
        private readonly Canvas _canvas;
        private readonly EventSystem _eSystem;

        private List<IUserInterfaceHandler> _uiHandlers;

        public UIHandler(IGameStateHandler game_state_handler) : base(game_state_handler)
        {
            _eSystem = GameObject.Find("EVENT_SYSTEM").GetComponent<EventSystem>();
            if (_eSystem == null)
                throw new UnityException("UIHandler requires the Scene to have a 'EVENT_SYSTEM' GameObject with a EventSystem componenet attached!");

            _canvas = GameObject.Find("USER_INTERFACE").GetComponent<Canvas>();
            if (_canvas == null)
                throw new UnityException("UIHandler requires the Scene to have a 'USER_INTERFACE' GameObject with a Canvas component attached!");

            _uiHandlers = new();
            for (int i = 0; i < _canvas.transform.childCount; i++)
            {
                IUserInterfaceHandler handler = _canvas.transform.GetChild(i).GetComponent<IUserInterfaceHandler>();
                if (handler != null)
                    _uiHandlers.Add(handler);
            }
        }

        protected override void Initialize()
        {
            gameStateHandler.GetHandler<InputHandler>().UI_InputTracker.OnInput += (position) =>
            {
                return _eSystem.IsPointerOverGameObject();
                /*_uiTrackerResults.Clear();
                PointerEventData ped = new(_eSystem);
                ped.position = position;
                _eSystem.RaycastAll(ped, _uiTrackerResults);

                if (_uiTrackerResults.Count > 0)
                    return true;
                return false;*/
            };
        }

        public Handler GetUIHandler<Handler>()
            where Handler : IUserInterfaceHandler
        {
            foreach (IUserInterfaceHandler uih in _uiHandlers)
            {
                if (uih.GetType().Equals(typeof(Handler)))
                    return (Handler)uih;
            }
            return default;
        }

        public Handler[] GetUIHandlers<Handler>()
            where Handler : IUserInterfaceHandler
        {
            List<Handler> handlers;
            handlers = new();

            foreach (IUserInterfaceHandler uih in _uiHandlers)
            {
                if (uih.GetType().Equals(typeof(Handler)))
                    handlers.Add((Handler)uih);
            }
            return handlers.ToArray();
        }
    }
}
