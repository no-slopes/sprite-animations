using UnityEditor;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    [CustomEditor(typeof(SpriteAnimator))]
    public class SpriteAnimatorInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return default;
        }
    }
}