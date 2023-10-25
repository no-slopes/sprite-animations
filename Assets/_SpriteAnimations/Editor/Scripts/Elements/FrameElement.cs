using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class FrameElement : VisualElement
    {
        #region Fields

        private CycleElement _owner;
        private int _index;
        private Frame _frame;
        private ViewZoomSliderElement _zoomSlider;

        private Label _indexLabel;
        private VisualElement _imagePreviewContainer;
        private ObjectField _spriteField;

        // Id Field
        private TextField _idField;
        private Button _idEditButton;
        private Button _idSaveButton;

        private Button _upButton;
        private Button _downButton;
        private Button _deleteButton;

        private Image _previewImage;

        #endregion

        #region Properties

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                _indexLabel.text = _index.ToString();
                EvaluateEnabledButtons();
            }
        }

        #endregion

        #region Constructors

        public FrameElement(CycleElement owner, int index, Frame frame, ViewZoomSliderElement zoomSlider)
        {
            _owner = owner;
            _frame = frame;

            _zoomSlider = zoomSlider;
            _zoomSlider.ValueChanged += SetImageZoom;

            AddToClassList("animation-cycle");

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/Frame");
            TemplateContainer template = tree.Instantiate();

            template.style.height = 60;
            template.style.flexGrow = 1;

            _indexLabel = template.Q<Label>("index-label");

            _imagePreviewContainer = template.Q<VisualElement>("image-preview-container");

            _previewImage = new Image();
            _previewImage.style.width = 40;
            _previewImage.style.height = 40;
            _previewImage.style.alignContent = Align.Center;
            _previewImage.style.justifyContent = Justify.Center;

            _imagePreviewContainer.Add(_previewImage);

            _spriteField = template.Q<ObjectField>("sprite-field");
            _spriteField.RegisterValueChangedCallback(OnSpriteObjectChanged);

            _idField = template.Q<TextField>("id-field");
            _idField.SetEnabled(_owner.Cycle.Identifiable);
            _idField.RegisterValueChangedCallback(OnIdTextChanged);

            _upButton = template.Q<Button>("up-button");
            _upButton.clicked += OnUpRequested;

            _downButton = template.Q<Button>("down-button");
            _downButton.clicked += OnDownRequested;

            _deleteButton = template.Q<Button>("delete-button");
            _deleteButton.clicked += OnDeleteRequested;

            InitializeValues(index, frame);

            Add(template);
        }

        #endregion

        #region Flow

        public void Dismiss()
        {
            if (_zoomSlider != null)
                _zoomSlider.ValueChanged -= SetImageZoom;
        }

        #endregion

        #region Frame

        private void InitializeValues(int index, Frame frame)
        {
            _index = index;
            _indexLabel.text = index.ToString();
            _previewImage.sprite = frame.Sprite;
            SetImageZoom(_zoomSlider.Value);
            _spriteField.value = frame.Sprite;
            _idField.value = frame.Id;

            EvaluateEnabledButtons();
        }

        private void OnUpRequested()
        {
            if (_index == 0) return;
            _owner.Swap(_index, _index - 1);
        }

        private void OnDownRequested()
        {
            if (_index == _owner.Size - 1) return;
            _owner.Swap(_index, _index + 1);
        }

        private void OnDeleteRequested()
        {
            _owner.RemoveFrame(_index);
        }

        #endregion

        #region Sprite

        private void OnSpriteObjectChanged(ChangeEvent<Object> changeEvent)
        {
            _frame.Sprite = changeEvent.newValue as Sprite;
            _previewImage.sprite = _frame.Sprite;
            _owner.DismissDropArea();
        }

        #endregion

        #region ID

        private void OnIdTextChanged(ChangeEvent<string> changeEvent)
        {
            _frame.Id = changeEvent.newValue;
        }

        #endregion

        #region Buttons

        public void EvaluateEnabledButtons()
        {
            _downButton.SetEnabled(Index < _owner.Size - 1);
            _upButton.SetEnabled(Index > 0);
        }

        #endregion

        #region Zoom

        private void SetImageZoom(float zoomValue)
        {
            _previewImage.transform.scale = new Vector3(zoomValue, zoomValue, 1);
        }

        #endregion
    }
}