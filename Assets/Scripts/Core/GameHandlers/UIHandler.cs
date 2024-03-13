using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using UnitWarfare.Core;
using UnitWarfare.Input;
using UnitWarfare.Core.Global;

namespace UnitWarfare.UI
{
    public class UIHandler : GameHandler
    {
        private readonly UIData m_data;
        public UIData Data => m_data;

        private readonly Canvas c_canvas;
        private readonly EventSystem c_eSystem;

        private List<IUserInterfaceHandler> _uiHandlers;

        public UIHandler(UIData data, IGameStateHandler game_state_handler) : base(game_state_handler)
        {
            m_data = data;

            c_eSystem = GameObject.Find(GlobalValues.GAME_EVENT_SYSTEM_NAME).GetComponent<EventSystem>();
            if (c_eSystem == null)
                throw new UnityException($"UIHandler requires the Scene to have a '{GlobalValues.GAME_EVENT_SYSTEM_NAME}' GameObject with a EventSystem componenet attached!");

            c_canvas = GameObject.Find(GlobalValues.GAME_CANVAS_NAME).GetComponent<Canvas>();
            if (c_canvas == null)
                throw new UnityException($"UIHandler requires the Scene to have a '{GlobalValues.GAME_CANVAS_NAME}' GameObject with a Canvas component attached!");

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

        protected override void Initialize() { }

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

        protected override void Enable()
        {
            throw new System.NotImplementedException();
        }

        protected override void Disable()
        {
            throw new System.NotImplementedException();
        }
    }
}
