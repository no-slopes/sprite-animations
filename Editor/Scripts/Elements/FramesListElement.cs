
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class FramesListElement : VisualElement
    {
        #region Fields

        private List<SpriteAnimationFrame> _frames;

        private VisualElement _template;
        private VisualElement _scrollViewContentContainer;
        private ScrollView _scrollView;
        private Button _addButton;
        private Button _clearButton;

        private FramesDropManipulator _dropManipulator;

        #endregion

        #region Getters

        public int FramesCount => _frames.Count;

        public ScrollView ScrollView => _scrollView;

        #endregion

        #region Constructors

        public FramesListElement()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/FramesList");
            _template = tree.Instantiate();

            _scrollView = _template.Q<ScrollView>("scroll-view");
            _scrollViewContentContainer = _scrollView.Q<VisualElement>("unity-content-container");

            _addButton = _template.Q<Button>("add-button");
            _addButton.clicked += AddFrame;

            _clearButton = _template.Q<Button>("clear-button");
            _clearButton.clicked += ClearFrames;

            _dropManipulator = new FramesDropManipulator(this);

            Add(_template);
        }

        #endregion

        #region Initialization

        public void Initialize(List<SpriteAnimationFrame> frames)
        {
            _frames = frames;

            for (int i = 0; i < _frames.Count; i++)
            {
                _scrollView.Insert(i, new FrameElement(this, i, _frames[i]));
            }
        }

        public void Dismiss()
        {
            _scrollView.Clear();
            _frames = null;
        }

        #endregion

        #region Frames

        public void Swap(int a, int b)
        {

            FrameElement frameAtA = _scrollView.ElementAt(a) as FrameElement;
            FrameElement frameAtB = _scrollView.ElementAt(b) as FrameElement;

            frameAtA.Index = b;
            frameAtB.Index = a;

            SpriteAnimationFrame temp = _frames[a];
            _frames[a] = _frames[b];
            _frames[b] = temp;

            _scrollView.Insert(a, frameAtB);
            _scrollView.Insert(b, frameAtA);
        }

        public void RemoveFrame(int index)
        {
            if (index < 0 || index >= _frames.Count) return;

            _frames.RemoveAt(index);
            _scrollView.RemoveAt(index);

            if (_frames.Count == 0) return;

            for (int i = index; i < _frames.Count; i++)
            {
                FrameElement frame = _scrollView.ElementAt(i) as FrameElement;
                frame.Index = i;
            }

            EvaluateButtons();
        }

        public void AddFrame()
        {
            int index = _frames.Count;

            SpriteAnimationFrame frame = new()
            {
                Index = index
            };

            AddFrame(index, frame);
        }

        public void AddFrame(Sprite sprite)
        {
            int index = _frames.Count;

            SpriteAnimationFrame frame = new()
            {
                Index = index,
                Sprite = sprite
            };

            AddFrame(index, frame);
        }

        private void AddFrame(int index, SpriteAnimationFrame frame)
        {
            FrameElement frameElement = new(this, index, frame);

            _frames.Add(frame);

            _scrollViewContentContainer.RegisterCallback<GeometryChangedEvent>(AfterAdd);
            _scrollView.Insert(index, frameElement);
            EvaluateButtons();
        }

        public void SetFrames(List<Sprite> sprites)
        {
            _frames.Clear();
            _scrollView.Clear();


            for (int i = 0; i < sprites.Count; i++)
            {
                SpriteAnimationFrame frame = new()
                {
                    Index = i,
                    Sprite = sprites[i],
                };
                _frames.Add(frame);
                _scrollView.Insert(i, new FrameElement(this, i, frame));
            }

            EvaluateButtons();
        }

        public void ClearFrames()
        {
            _frames.Clear();
            _scrollView.Clear();
        }

        private void EvaluateButtons()
        {
            for (int i = 0; i < _frames.Count; i++)
            {
                FrameElement element = _scrollView.ElementAt(i) as FrameElement;
                element.EvaluateEnabledButtons();
            }
        }

        private void AfterAdd(GeometryChangedEvent evt)
        {
            _scrollViewContentContainer.UnregisterCallback<GeometryChangedEvent>(AfterAdd);
            _scrollView.ScrollTo(_scrollView.ElementAt(_frames.Count - 1));
        }


        #endregion

        #region Drop Area

        public void HighlightDropArea()
        {
            _scrollView.AddToClassList("dropping");
        }

        public void DismissDropArea()
        {
            _scrollView.RemoveFromClassList("dropping");
        }

        #endregion
    }
}