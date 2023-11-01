using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    [CustomPropertyDrawer(typeof(SpriteAnimationManageButton), true)]
    public class SpriteAnimationManageButtonDrawer : PropertyDrawer
    {
        #region GUI

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualTreeAsset treeAsset = Resources.Load<VisualTreeAsset>("UI Documents/Inspectors/SpriteAnimationManageButtonUI");
            TemplateContainer container = treeAsset.CloneTree();

            SpriteAnimation animation = (SpriteAnimation)property.serializedObject.targetObject;

            Button openButton = container.Q<Button>("open-manager-button");
            openButton.clicked += () =>
            {
                Debug.Log($"Selected {nameof(SpriteAnimation)}: {animation.name}");
            };

            return container;
        }

        #endregion
    }
}