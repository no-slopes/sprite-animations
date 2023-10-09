using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class AnimationPreviewEditorWindow : EditorWindow
    {
        #region Static

        [MenuItem("Tools/Handy Tools/Animation Preview (development)")]
        public static void OpenEditorWindow()
        {
            var window = GetWindow<AnimationPreviewEditorWindow>();
            window.titleContent = new GUIContent("Preview (development)");
            window.minSize = new Vector2(200, 250);
            window.maxSize = new Vector2(200, 250);
            window.Show();
        }

        #endregion

        #region Fields

        private ToolbarButton _playButton;

        #endregion

        #region Behaviour

        public void OnEnable()
        {

            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI Documents/AnimationPreviewUIDocument");
            TemplateContainer templateContainer = visualTree.Instantiate();
            rootVisualElement.Add(templateContainer);

            _playButton = rootVisualElement.Q<ToolbarButton>("play-button");

            string text = _playButton.text;
            _playButton.text = "";

            VisualElement container = rootVisualElement.Q<VisualElement>("container");


            MaterialIcon iconElement = new()
            {
                IconName = text
            };

            _playButton.Add(iconElement);
            container.Add(iconElement);
        }

        #endregion
    }
}