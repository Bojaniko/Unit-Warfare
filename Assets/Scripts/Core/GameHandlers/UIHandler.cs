using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using UnitWarfare.Core;
using UnitWarfare.Input;

namespace UnitWarfare.UI
{
    public class UIHandler : GameHandler
    {
        private readonly UIData m_data;
        public UIData Data => m_data;

        private readonly Canvas c_canvas;
        private readonly EventSystem c_eSystem;

        private List<IUserInterfaceHandler> _uiHandlers;

        private const string EVENT_SYSTEM_NAME = "EVENT_SYSTEM";
        private const string CANVAS_NAME = "USER_INTERFACE";

        public UIHandler(UIData data, IGameStateHandler game_state_handler) : base(game_state_handler)
        {
            m_data = data;

            c_eSystem = GameObject.Find(EVENT_SYSTEM_NAME).GetComponent<EventSystem>();
            if (c_eSystem == null)
                throw new UnityException($"UIHandler requires the Scene to have a '{EVENT_SYSTEM_NAME}' GameObject with a EventSystem componenet attached!");

            c_canvas = GameObject.Find(CANVAS_NAME).GetComponent<Canvas>();
            if (c_canvas == null)
                throw new UnityException($"UIHandler requires the Scene to have a '{CANVAS_NAME}' GameObject with a Canvas component attached!");

            _uiHandlers = new();
            for (int i = 0; i < c_canvas.transform.childCount; i++)
            {
                IUserInterfaceHandler handler = c_canvas.transform.GetChild(i).GetComponent<IUserInterfaceHandler>();
                if (handler != null)
                    _uiHandlers.Add(handler);
            }

            InitializeComponents();
        }

        // ##### COMPONENTS ##### \\

        private List<UIComponent> m_components;
        public UIComponent[] Components => m_components.ToArray();

        private void InitializeComponents()
        {
            m_components = new();

            UIComponent.Config config = new(c_canvas, Data.PanelSettings);

            foreach (System.Type t in typeof(UIComponent).Assembly.GetTypes())
            {
                if (t.IsAbstract)
                    continue;
                if (!t.IsSubclassOf(typeof(UIComponent)))
                    continue;
                UIComponent component = (UIComponent)System.Activator.CreateInstance(t, new object[] { config });
                m_components.Add(component);
            }
        }

        public UIComponent GetComponent(System.Type t)
        {
            if (!t.IsSubclassOf(typeof(UIComponent)))
                return null;
            foreach (UIComponent component in m_components)
            {
                if (component.GetType().Equals(t))
                    return component;
            }
            return null;
        }

        public Component GetComponent<Component>()
            where Component : UIComponent
        {
            foreach (UIComponent component in m_components)
            {
                if (component.GetType().Equals(typeof(Component)))
                    return component as Component;
            }
            return null;
        }

        protected override void Initialize()
        {
            gameStateHandler.GetHandler<InputHandler>().UI_InputTracker.OnInput += (position) =>
            {
                return c_eSystem.IsPointerOverGameObject();
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
