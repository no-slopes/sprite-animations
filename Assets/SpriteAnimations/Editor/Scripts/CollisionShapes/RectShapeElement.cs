using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class RectShapeElement : CollisionShapeElement
    {
        private RectShape _rectShape;
        private Image _image;

        public override CollisionShape Shape => _rectShape;
        public override Image Image
        {
            get
            {
                _image ??= new Image
                {
                    name = "RectImage",
                    sprite = Resources.Load<Sprite>("Images/Square")
                };
                return _image;
            }
        }

        private ShapeHandle _handleDownLeft;
        private ShapeHandle _handleUpLeft;
        private Rect _rect;

        public RectShapeElement()
        {
            _rectShape = new RectShape();

            _handleDownLeft = new ShapeHandle();
            _handleUpLeft = new ShapeHandle();

            _rect = new Rect
            {
                x = 200,
                y = 200,
                width = 100,
                height = 100
            };
        }

        public void Initialize()
        {
            parent.Add(_handleDownLeft);
            // parent.Add(_handleUpLeft);
            parent.Add(Image);

            Image.style.position = Position.Absolute;

            style.position = Position.Absolute;
            style.width = _rect.width;
            style.height = _rect.height;
        }

        public override void Reposition(Vector2 delta, Vector2 origin)
        {
            float minX = 0 - parent.worldBound.width / 2 + Image.worldBound.width / 2;
            float maxX = parent.worldBound.width / 2 - Image.worldBound.width / 2;
            float minY = 0 - parent.worldBound.height / 2 + Image.worldBound.height / 2;
            float maxY = parent.worldBound.height / 2 - Image.worldBound.height / 2;

            Image.transform.position = new Vector2(
                Mathf.Clamp(origin.x + delta.x, minX, maxX),
                Mathf.Clamp(origin.y + delta.y, minY, maxY)
            );

            transform.position = Image.transform.position;

            _handleDownLeft.transform.position = Image.worldBound.min;
            // _handleUpLeft.transform.position = _image.worldBound.max;
        }
    }
}