using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public abstract class CollisionShapeElement : VisualElement
    {
        public abstract CollisionShape Shape { get; }
        public abstract Image Image { get; }
        public abstract void Reposition(Vector2 delta, Vector2 origin);

        private ShapeDragManipulator _dragManipulator;

        public CollisionShapeElement()
        {
            _dragManipulator = new(this);
            Image.AddManipulator(_dragManipulator);
        }
    }

    public class ShapeDragManipulator : PointerManipulator
    {
        private bool _enabled = false;
        private CollisionShapeElement _targetShape;
        private Vector2 StartingPosition { get; set; }
        private Vector3 PointerStartPosition { get; set; }

        // Write a constructor to set target and store a reference to the
        // root of the visual tree.
        public ShapeDragManipulator(CollisionShapeElement shape)
        {
            target = shape.Image;
            _targetShape = shape;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        // This method stores the starting position of target and the pointer,
        // makes target capture the pointer, and denotes that a drag is now in progress.
        private void PointerDownHandler(PointerDownEvent evt)
        {
            StartingPosition = target.transform.position;
            PointerStartPosition = evt.position;
            target.CapturePointer(evt.pointerId);
            _enabled = true;
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            target.ReleasePointer(evt.pointerId);
            _enabled = false;
        }

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (!_enabled || !target.HasPointerCapture(evt.pointerId)) return;

            Vector3 pointerDelta = evt.position - PointerStartPosition;

            _targetShape.Reposition(pointerDelta, StartingPosition);
        }

        private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
        {
            if (_enabled)
            {
                _enabled = false;
            }
        }
    }
}