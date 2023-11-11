using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public abstract class CollisionShapeElement : VisualElement
    {
        public abstract CollisionShape Shape { get; }
        public abstract Image Image { get; }

        private ShapeDragManipulator _dragManipulator;

        public CollisionShapeElement()
        {
            _dragManipulator = new(this);
            this.AddManipulator(_dragManipulator);
        }
    }

    public class ShapeDragManipulator : PointerManipulator
    {
        private bool _enabled = false;
        private CollisionShapeElement _targetShape;
        private VisualElement Root { get; }
        private Vector2 TargetStartPosition { get; set; }
        private Vector3 PointerStartPosition { get; set; }

        // Write a constructor to set target and store a reference to the
        // root of the visual tree.
        public ShapeDragManipulator(CollisionShapeElement target)
        {
            this.target = target;
            Root = target.parent;
            _targetShape = target;
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
            TargetStartPosition = _targetShape.transform.position;
            PointerStartPosition = evt.position;
            _targetShape.CapturePointer(evt.pointerId);
            _enabled = true;
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            _targetShape.ReleasePointer(evt.pointerId);
            _enabled = false;
        }

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (!_enabled || !_targetShape.HasPointerCapture(evt.pointerId)) return;

            Vector3 pointerDelta = evt.position - PointerStartPosition;

            float minX = 0 - _targetShape.parent.worldBound.width / 2 + _targetShape.worldBound.width / 2;
            float maxX = _targetShape.parent.worldBound.width / 2 - _targetShape.worldBound.width / 2;
            float minY = 0 - _targetShape.parent.worldBound.height / 2 + _targetShape.worldBound.height / 2;
            float maxY = _targetShape.parent.worldBound.height / 2 - _targetShape.worldBound.height / 2;

            target.transform.position = new Vector2(
                Mathf.Clamp(TargetStartPosition.x + pointerDelta.x, minX, maxX),
                Mathf.Clamp(TargetStartPosition.y + pointerDelta.y, minY, maxY)
            );
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