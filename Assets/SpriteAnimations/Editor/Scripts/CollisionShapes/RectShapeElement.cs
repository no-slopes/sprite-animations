using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class RectShapeElement : CollisionShapeElement
    {
        private RectShape _rectShape;
        private Image _image;

        public override CollisionShape Shape => _rectShape;
        public override Image Image => _image;

        public RectShapeElement()
        {
            _rectShape = new RectShape();
            _image = new Image
            {
                sprite = Resources.Load<Sprite>("Images/Square")
            };

            Add(_image);

            style.position = Position.Absolute;

            transform.scale = new Vector3(3f, 1f, 1);
        }
    }
}