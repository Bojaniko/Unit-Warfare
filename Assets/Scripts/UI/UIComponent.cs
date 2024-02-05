using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnitWarfare.Core;

namespace UnitWarfare.UI
{
    public abstract class ShowableUIComponent : UIComponent
    {
        private bool m_showing;
        public override bool Showing => m_showing;

        protected ShowableUIComponent(Config config) : base(config)
        {
            m_showing = false;
            root.SetEnabled(false);
        }

        protected virtual void OnShow()
        {

        }
        protected virtual void OnHide()
        {

        }

        public void Show()
        {
            OnShow();
            m_showing = false;
            root.SetEnabled(true);
        }

        public void Hide()
        {
            OnHide();
            m_showing = false;
            root.SetEnabled(false);
        }
    }

    public abstract class DataBindedUIComponent<Binding> : UIComponent
        where Binding : notnull
    {
        private bool m_showing;
        public override bool Showing => m_showing;

        protected DataBindedUIComponent(Config config) : base(config)
        {
            m_showing = false;
            root.SetEnabled(false);
        }

        protected virtual void OnShow(Binding data)
        {

        }
        protected virtual void OnHide()
        {

        }

        public void Show(Binding data)
        {
            OnShow(data);
            m_showing = true;
            root.SetEnabled(true);
        }

        public void Hide()
        {
            OnHide();
            m_showing = false;
            root.SetEnabled(false);
        }
    }

    public abstract class UIComponent
    {
        public record Config(Canvas Canvas, PanelSettings PanelSettings);

        private readonly Config config;
        public Config Data => config;

        private readonly UIDocument document;
        protected VisualElement root => document.rootVisualElement;

        protected abstract string documentPath { get; }

        protected readonly EncapsulatedMonoBehaviour emb;

        public abstract bool Showing { get; }

        protected UIComponent(Config config)
        {
            this.config = config;

            GameObject go = new($"UI_COMPONENT: {GetType().Name.ToUpper()}");
            go.transform.parent = this.config.Canvas.transform;
            emb = new(go);

            VisualTreeAsset vta = (VisualTreeAsset)Resources.Load(documentPath);
            document = emb.gameObject.AddComponent<UIDocument>();
            document.panelSettings = this.config.PanelSettings;
            document.visualTreeAsset = vta;
        }
    }
}