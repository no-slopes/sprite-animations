using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class FrameManagerWindow : EditorWindow
    {
        #region Static

        public static FrameManagerWindow Open(Frame frame)
        {
            var window = GetWindow<FrameManagerWindow>();
            window.titleContent = new GUIContent("Frame Manager");
            window.minSize = new Vector2(500, 300);
            window.Initialize(frame);
            window.Show();
            return window;
        }

        #endregion

        #region Fields

        private float _timeTracker;
        private Frame _frame;

        // UI Elements
        private ObjectField _spriteSelectorField;

        private Image _image;
        private VisualElement _imageContainer;

        #endregion

        #region Getters

        public AnimationsManagerWindowData Data => AnimationsManagerWindowData.instance;

        private Image Image
        {
            get
            {
                if (_image == null)
                {
                    _image = new Image();

                    _image.style.width = 300;
                    _image.style.height = 300;
                    _image.style.alignContent = Align.Center;
                    _image.style.justifyContent = Justify.Center;
                }

                return _image;
            }
        }

        #endregion

        #region GUI

        private void OnEnable()
        {
            _timeTracker = (float)EditorApplication.timeSinceStartup;

            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI Documents/FrameManagerUI");
            TemplateContainer templateContainer = visualTree.CloneTree();
            templateContainer.style.flexGrow = 1;

            _spriteSelectorField = templateContainer.Q<ObjectField>("sprite-selector-field");
            _spriteSelectorField.RegisterValueChangedCallback(OnSpriteChanged);

            _imageContainer = templateContainer.Q<VisualElement>("image-container");
            _imageContainer.Add(Image);

            if (_frame != null)
            {
                _spriteSelectorField.SetValueWithoutNotify(_frame.Sprite);
            }

            rootVisualElement.Add(templateContainer);
        }

        private void OnDisable()
        {

        }

        private void Update()
        {
            if (_frame == null) return;
        }

        public void Initialize(Frame frame)
        {
            SetFrame(frame);
            SetImage(frame.Sprite);
        }

        #endregion

        #region Frame

        public void SetFrame(Frame frame)
        {
            _frame = frame;
            _frame.SpriteChanged += OnFrameSpriteChanged;
            _spriteSelectorField?.SetValueWithoutNotify(_frame.Sprite);
        }

        #endregion

        #region Image

        private void SetImage(Sprite sprite)
        {
            Image.sprite = sprite;
        }

        #endregion

        #region Sprite

        private void OnFrameSpriteChanged(Sprite sprite)
        {
            _spriteSelectorField.SetValueWithoutNotify(sprite);
            SetImage(sprite);
        }

        private void OnSpriteChanged(ChangeEvent<Object> evt)
        {
            _frame.Sprite = evt.newValue as Sprite;
            SetImage(_frame.Sprite);
        }

        #endregion
    }
}