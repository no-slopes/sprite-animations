using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public delegate void AnimationCreatedEvent(SpriteAnimation animation);
    public class CreateAnimationWindow : EditorWindow
    {
        #region Static

        [MenuItem("Tools/Sprite Animations/Create Animation")]
        public static CreateAnimationWindow OpenEditorWindow()
        {
            var window = GetWindow<CreateAnimationWindow>();
            window.titleContent = new GUIContent("Create Animation");
            window.minSize = new Vector2(300, 150);
            window.Show();

            return window;
        }

        #endregion

        #region Fields

        private VisualElement _container;
        private TextField _nameField;
        private EnumField _typeField;
        private Button _createButton;

        private string _usedPath = "Assets";

        #endregion

        #region Properties

        public string UsedPath { get => _usedPath; set => _usedPath = value; }

        #endregion

        #region Behaviour

        private void OnEnable()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/CreateAnimationForm");
            TemplateContainer template = tree.CloneTree();
            _container = template.Q<VisualElement>("container");

            _nameField = template.Q<TextField>("name-field");
            _nameField.value = "New Animation";

            _typeField = template.Q<EnumField>("type-field");
            _createButton = template.Q<Button>("create-button");
            _createButton.clicked += OnCreateClicked;

            rootVisualElement.Add(template);
        }

#if !UNITY_2023_2
        void OnLostFocus()
        {
            Close();
        }
#endif

        #endregion

        #region Creating

        private void OnCreateClicked()
        {
            string name = _nameField.value;
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("Animation name cannot be empty");
                return;
            }

            string path = EditorUtility.OpenFolderPanel("Select Folder", _usedPath, "");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            string[] parts = path.Split("/Assets");
            path = "Assets" + parts[^1];

            _usedPath = path;

            SpriteAnimation animation = _typeField.value switch
            {
                AnimationType.SingleCycle => CreateSingleCycleAnimation(path, name),
                AnimationType.Windrose => CreateWindroseAnimation(path, name),
                AnimationType.Combo => CreateComboAnimation(path, name),
                AnimationType.Composite => CreateCompositeAnimation(path, name),
                _ => throw new ArgumentOutOfRangeException(nameof(_typeField.value), null, null)
            };

            animation.AnimationName = name;
            EditorUtility.SetDirty(animation);
            AssetDatabase.SaveAssetIfDirty(animation);
            AnimationCreated?.Invoke(animation);
            Close();
        }

        private SpriteAnimationSingleCycle CreateSingleCycleAnimation(string path, string name)
        {
            SpriteAnimationSingleCycle singleCycleAsset = ScriptableObject.CreateInstance<SpriteAnimationSingleCycle>();
            singleCycleAsset.GenerateCycle();
            AssetDatabase.CreateAsset(singleCycleAsset, $"{path}/{name}.asset");
            return singleCycleAsset;
        }

        private SpriteAnimationWindrose CreateWindroseAnimation(string path, string name)
        {
            SpriteAnimationWindrose windroseAsset = ScriptableObject.CreateInstance<SpriteAnimationWindrose>();
            AssetDatabase.CreateAsset(windroseAsset, $"{path}/{name}.asset");
            return windroseAsset;
        }

        private SpriteAnimationCombo CreateComboAnimation(string path, string name)
        {
            SpriteAnimationCombo comboAsset = ScriptableObject.CreateInstance<SpriteAnimationCombo>();
            comboAsset.CreateCycle();
            AssetDatabase.CreateAsset(comboAsset, $"{path}/{name}.asset");
            return comboAsset;
        }

        private SpriteAnimationComposite CreateCompositeAnimation(string path, string name)
        {
            SpriteAnimationComposite compositeAsset = ScriptableObject.CreateInstance<SpriteAnimationComposite>();
            compositeAsset.GenerateCycles();
            AssetDatabase.CreateAsset(compositeAsset, $"{path}/{name}.asset");
            return compositeAsset;
        }

        #endregion

        #region Events

        public event AnimationCreatedEvent AnimationCreated;

        #endregion

    }
}