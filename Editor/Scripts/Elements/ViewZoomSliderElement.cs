using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public delegate void ZoomValueChangedEvent(float value);
    public class ViewZoomSliderElement : VisualElement
    {
        #region Fields

        private float _value;
        private Slider _zoomSlider;

        #endregion

        #region Properties

        public float Value => _value;

        #endregion

        #region Constructors

        public ViewZoomSliderElement()
        {
            _value = 1;

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/ViewZoomSlider");
            TemplateContainer template = tree.Instantiate();

            _zoomSlider = template.Q<Slider>("zoom-slider");
            _zoomSlider.value = _value;
            _zoomSlider.RegisterValueChangedCallback(evt => SetValue(evt.newValue));

            Add(template);
        }

        #endregion

        #region Value

        public event ZoomValueChangedEvent ValueChanged;

        private void SetValue(float value)
        {
            _value = value;
            ValueChanged?.Invoke(_value);
        }

        #endregion
    }
}