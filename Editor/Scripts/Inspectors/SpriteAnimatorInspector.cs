using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    [CustomEditor(typeof(SpriteAnimator))]
    public class SpriteAnimatorInspector : UnityEditor.Editor
    {
        #region Inspector

        [SerializeField]
        private MonoScript _scriptAsset;

        #endregion

        #region Fields

        private SpriteAnimator _spriteAnimator;
        private VisualElement _defaultAnimationFieldContainer;
        private Toggle _playerOnStartToggle;

        #endregion

        #region GUI

        public override VisualElement CreateInspectorGUI()
        {
            _spriteAnimator = target as SpriteAnimator;

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/SpriteAnimatorInspector");
            TemplateContainer inspector = tree.Instantiate();

            ObjectField scriptField = inspector.Query<ObjectField>("script-field");
            scriptField.SetEnabled(false);
            scriptField.value = _scriptAsset;

            Button openManagerButton = inspector.Query<Button>("open-manager-button");
            openManagerButton.clicked += () => AnimationsManagerWindow.OpenEditorWindow(_spriteAnimator);

            Button openDocsButton = inspector.Query<Button>("open-docs-button");
            openDocsButton.clicked += OpenDocs;

            _playerOnStartToggle = inspector.Query<Toggle>("play-on-start-field");
            _playerOnStartToggle.RegisterValueChangedCallback(evt => DisplayDefaultAnimationContainer(evt.newValue));

            _defaultAnimationFieldContainer = inspector.Query<VisualElement>("default-animation-field-row");
            DisplayDefaultAnimationContainer(_playerOnStartToggle.value);

            PropertyField animationsListField = inspector.Query<PropertyField>("animations-list-field");
            animationsListField.SetEnabled(false);

            return inspector;
        }

        #endregion

        #region Events

        private void DisplayDefaultAnimationContainer(bool shouldDisplay)
        {
            _defaultAnimationFieldContainer.style.display = shouldDisplay ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OpenDocs()
        {
            Application.OpenURL("https://no-slopes.github.io/sprite-animations/documentation/sprite-animator.html");
        }

        #endregion
    }
}