using System.Collections.Generic;
using System.Linq;
using SpriteAnimations.Performers;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class AnimationPreviewElement : VisualElement
    {
        #region Fields

        private ContentElement _contentElement;

        private VisualElement _imageContainer;
        private Image _image;
        private ToolbarButton _playButton;
        private ToolbarButton _stopButton;

        private List<SpriteAnimationFrame> _frames;
        private SpriteAnimationFrame _currentFrame;

        private bool _playing;
        private float _currentCycleElapsedTime = 0;

        #endregion

        #region Properties

        public List<SpriteAnimationFrame> Frames
        {
            get => _frames;
            set
            {
                _frames = value;
                if (_frames.Count > 0)
                {
                    _currentFrame = _frames[0];
                    _image.sprite = _currentFrame.Sprite;
                }
            }
        }

        #endregion

        #region Constructors

        public AnimationPreviewElement(ContentElement contentElement)
        {
            _contentElement = contentElement;

            AddToClassList("animation-cycle");

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationPreview");
            TemplateContainer template = tree.Instantiate();

            template.style.height = 231;
            template.style.width = 200;

            _imageContainer = template.Q<VisualElement>("image-container");

            _image = new Image();

            _image.style.width = 150;
            _image.style.height = 150;

            _imageContainer.Add(_image);

            _playButton = template.Q<ToolbarButton>("play-button");
            _stopButton = template.Q<ToolbarButton>("stop-button");

            _playButton.clicked += Play;
            _stopButton.clicked += Stop;

            Add(template);
        }

        #endregion

        #region Initialization

        public void Dismiss()
        {
            _currentFrame = null;
            _image.sprite = null;
        }

        #endregion

        #region Tick       

        private void OnTick(float deltaTime)
        {
            if (_frames.Count == 0)
            {
                _image.sprite = null;
                Stop();
                return;
            }

            _currentCycleElapsedTime += deltaTime;
            float frameDuration = _contentElement.CurrentAnimation != null ? 1f / _contentElement.CurrentAnimation.FPS : 0f;
            float duration = _frames.Count * frameDuration;

            int frameIndex = Mathf.FloorToInt(_currentCycleElapsedTime * _frames.Count / duration);
            SpriteAnimationFrame evaluatedFrame = _frames.ElementAtOrDefault(frameIndex);

            if (_currentCycleElapsedTime >= duration)
            {
                _currentCycleElapsedTime = 0;
            }

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return;

            _currentFrame = evaluatedFrame;
            _image.sprite = evaluatedFrame.Sprite;
        }

        #endregion

        #region Flow

        public void Play()
        {
            if (_playing) return;

            if (_frames.Count == 0)
            {
                _image.sprite = null;
                return;
            }

            _contentElement.Tick += OnTick;
            _currentCycleElapsedTime = 0;
            _playing = true;
        }

        public void Stop()
        {
            if (!_playing) return;

            _playing = false;
            _contentElement.Tick -= OnTick;
            _currentFrame = null;

            if (_frames.Count == 0)
            {
                _image.sprite = null;
            }
        }

        #endregion
    }
}