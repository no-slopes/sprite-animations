using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class ShapeHandle : Image
    {
        public ShapeHandle()
        {
            style.position = Position.Absolute;

            style.width = 10;
            style.height = 10;

            sprite = Resources.Load<Sprite>("Images/Circle");
            tintColor = Color.red;
        }
    }
}