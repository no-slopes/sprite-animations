using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class AnimationPreviewElement : VisualElement
    {
        #region Fields

        private VisualElement _imageContainer;
        private Image _image;
        private Slider _zoomSlider;
        private ToolbarButton _playButton;
        private ToolbarButton _stopButton;

        private IFPSProvider _fpsProvider;
        private ITickProvider _tickProvider;
        private CycleElement _cycle;
        private SpriteAnimationFrame _currentFrame;

        private bool _playing;
        private int _fps = 0;
        private float _currentCycleElapsedTime = 0;

        #endregion

        #region Constructors

        public AnimationPreviewElement()
        {
            AddToClassList("animation-cycle");

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationPreview");
            TemplateContainer template = tree.Instantiate();

            template.style.height = 251;
            template.style.width = 200;

            _imageContainer = template.Q<VisualElement>("image-container");
            _image = new Image();
            _image.style.width = 150;
            _image.style.height = 150;
            _imageContainer.Add(_image);

            _zoomSlider = template.Q<Slider>("zoom-slider");
            _zoomSlider.RegisterValueChangedCallback(evt => SetImageZoom(evt.newValue));

            _playButton = template.Q<ToolbarButton>("play-button");
            _stopButton = template.Q<ToolbarButton>("stop-button");

            _playButton.clicked += Play;
            _stopButton.clicked += Stop;

            Add(template);
        }

        #endregion

        #region Initialization

        public void Initialize(ITickProvider tickProvider, IFPSProvider fpsProvider, CycleElement cycle)
        {
            _fpsProvider = fpsProvider;
            _fpsProvider.FPSChanged += OnFPSChanged;
            _fps = _fpsProvider.FPS;

            _zoomSlider.value = 1;

            _tickProvider = tickProvider;
            _cycle = cycle;
            _cycle.CycleCollectionReset += OnCycleCollectionReset;
            OnCycleCollectionReset(_cycle);
        }

        public void Dismiss()
        {
            if (_cycle != null)
                _cycle.CycleCollectionReset -= OnCycleCollectionReset;

            if (_fpsProvider != null)
                _fpsProvider.FPSChanged -= OnFPSChanged;

            Stop();

            _cycle = null;
            _currentFrame = null;
            _image.sprite = null;
        }

        #endregion

        #region Tick       

        private void OnTick(float deltaTime)
        {
            if (_cycle.Frames.Count == 0)
            {
                _image.sprite = null;
                Stop();
                return;
            }

            _currentCycleElapsedTime += deltaTime;

            float frameDuration = 1f / _fps;
            float duration = _cycle.Frames.Count * frameDuration;

            SpriteAnimationFrame evaluatedFrame = EvaluateCurrentFrameIndex(_currentCycleElapsedTime, duration);

            if (_currentCycleElapsedTime >= duration)
            {
                _currentCycleElapsedTime = 0;
            }

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return;

            SetFrame(evaluatedFrame);
        }

        #endregion

        #region Frames

        private SpriteAnimationFrame EvaluateCurrentFrameIndex(float elapsedTime, float duration)
        {

            int frameIndex = Mathf.FloorToInt(elapsedTime * _cycle.Frames.Count / duration);
            return _cycle.Frames.ElementAtOrDefault(frameIndex);
        }

        private void SetFrame(SpriteAnimationFrame frame)
        {
            _currentFrame = frame;
            _image.sprite = _currentFrame.Sprite;

        }

        #endregion

        #region FPS

        private void OnFPSChanged(int fps)
        {
            _fps = fps;
        }

        #endregion

        #region Flow

        public void Play()
        {
            if (_playing) return;

            if (_cycle.Frames.Count == 0)
            {
                _image.sprite = null;
                return;
            }

            _tickProvider.Tick += OnTick;
            _currentCycleElapsedTime = 0;
            _playing = true;
        }

        public void Stop()
        {
            if (!_playing) return;

            _playing = false;
            _tickProvider.Tick -= OnTick;
            _currentFrame = null;

            if (_cycle.Frames.Count == 0)
            {
                _image.sprite = null;
            }
        }

        #endregion

        #region Cycle

        private void OnCycleCollectionReset(CycleElement cycle)
        {
            if (cycle.Frames.Count > 0)
            {
                SetFrame(cycle.Frames[0]);
            }
            else
            {
                _image.sprite = null;
            }
        }

        #endregion

        #region Image

        private void SetImageZoom(float zoom)
        {
            _image.transform.scale = new Vector3(zoom, zoom, 1);
        }

        #endregion
    }
}