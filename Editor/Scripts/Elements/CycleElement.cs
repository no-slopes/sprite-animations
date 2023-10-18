using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public delegate void CycleCollectionResetEvent(CycleElement cycle);
    public class CycleElement : VisualElement
    {
        #region Fields

        private Cycle _cycle;
        private SpriteAnimationView _animationView;

        private ScrollView _scrollView;
        private VisualElement _scrollViewContentContainer;
        private Button _addButton;
        private Button _clearButton;

        private Label _sizeText;
        private FramesDropManipulator _dropManipulator;

        #endregion

        #region Properties

        public Cycle Cycle
        {
            get => _cycle;
            set
            {
                _cycle = value;
            }
        }


        #endregion

        #region Getters

        public int Size => _cycle.Size;
        public List<Frame> Frames => _cycle.Frames;
        public ScrollView ScrollView => _scrollView;

        #endregion

        #region Constructors

        public CycleElement()
        {
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1;

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/Cycle");
            TemplateContainer template = tree.Instantiate();
            template.style.flexGrow = 1;

            _scrollView = template.Q<ScrollView>("scroll-view");
            _scrollViewContentContainer = _scrollView.Q<VisualElement>("unity-content-container");

            _sizeText = template.Q<Label>("size-text");

            _addButton = template.Q<Button>("add-button");
            _addButton.clicked += AddFrame;

            _clearButton = template.Q<Button>("clear-button");
            _clearButton.clicked += ClearFrames;

            _dropManipulator = new FramesDropManipulator(this);

            Add(template);
        }

        #endregion

        #region Flow

        public void Initialize(Cycle cycle, SpriteAnimationView view)
        {
            _cycle = cycle;
            _animationView = view;

            for (int i = 0; i < _cycle.Size; i++)
            {
                _scrollView.Insert(i, new FrameElement(this, i, _cycle.Frames[i], view.ViewZoomSlider));
            }

            _sizeText.text = _cycle.Size.ToString();
        }

        public void Dismiss()
        {
            _cycle = null;
            _animationView = null;

            if (_cycle != null)
            {
                for (int i = 0; i < _cycle.Size; i++)
                {
                    FrameElement frameElement = _scrollView.ElementAt(i) as FrameElement;
                    frameElement.Dismiss();
                }
            }

            _scrollView.Clear();
        }

        #endregion

        #region Frames

        public void Swap(int a, int b)
        {
            FrameElement frameAtA = _scrollView.ElementAt(a) as FrameElement;
            FrameElement frameAtB = _scrollView.ElementAt(b) as FrameElement;

            frameAtA.Index = b;
            frameAtB.Index = a;

            // Tuple Swap
            (_cycle.Frames[b], _cycle.Frames[a]) = (_cycle.Frames[a], _cycle.Frames[b]);

            _scrollView.Insert(a, frameAtB);
            _scrollView.Insert(b, frameAtA);
        }

        public void RemoveFrame(int index)
        {
            if (index < 0 || index >= _cycle.Size) return;

            _cycle.Frames.RemoveAt(index);
            _scrollView.RemoveAt(index);

            if (_cycle.Size == 0) return;

            for (int i = index; i < _cycle.Size; i++)
            {
                FrameElement frame = _scrollView.ElementAt(i) as FrameElement;
                frame.Index = i;
            }

            UpdateSizeText(_cycle.Size);
            EvaluateButtons();

            if (_cycle.Frames.Count == 0)
            {
                CycleCollectionReset?.Invoke(this);
            }
        }

        public void AddFrame()
        {
            int index = _cycle.Size;

            Frame frame = new();

            AddFrame(index, frame);
        }

        public void AddFrame(Sprite sprite)
        {
            int index = _cycle.Size;

            Frame frame = new()
            {
                Sprite = sprite
            };

            AddFrame(index, frame);
        }

        private void AddFrame(int index, Frame frame)
        {
            FrameElement frameElement = new(this, index, frame, _animationView.ViewZoomSlider);

            int previousCount = _cycle.Frames.Count;
            _cycle.Frames.Add(frame);

            _scrollViewContentContainer.RegisterCallback<GeometryChangedEvent>(AfterAdd);
            _scrollView.Insert(index, frameElement);

            UpdateSizeText(_cycle.Size);
            EvaluateButtons();

            if (previousCount <= 0)
            {
                CycleCollectionReset?.Invoke(this);
            }
        }

        public void SetFrames(List<Sprite> sprites)
        {
            _cycle.Frames.Clear();
            _scrollView.Clear();

            for (int i = 0; i < sprites.Count; i++)
            {
                Frame frame = new()
                {
                    Sprite = sprites[i],
                };

                _cycle.Frames.Add(frame);
                _scrollView.Insert(i, new FrameElement(this, i, frame, _animationView.ViewZoomSlider));
            }

            UpdateSizeText(_cycle.Size);
            EvaluateButtons();
            CycleCollectionReset?.Invoke(this);
        }

        public event CycleCollectionResetEvent CycleCollectionReset;

        public void ClearFrames()
        {
            _cycle.Frames.Clear();
            _scrollView.Clear();
            UpdateSizeText(0);
        }

        private void EvaluateButtons()
        {
            for (int i = 0; i < _cycle.Size; i++)
            {
                FrameElement element = _scrollView.ElementAt(i) as FrameElement;
                element.EvaluateEnabledButtons();
            }
        }

        private void AfterAdd(GeometryChangedEvent evt)
        {
            _scrollViewContentContainer.UnregisterCallback<GeometryChangedEvent>(AfterAdd);
            _scrollView.ScrollTo(_scrollView.ElementAt(_cycle.Size - 1));
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

        #region Size

        private void UpdateSizeText(int size)
        {
            _sizeText.text = size.ToString();
        }

        #endregion
    }
}