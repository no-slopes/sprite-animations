using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public abstract class CollisionShapeElement : VisualElement
    {
        public abstract CollisionShape Shape { get; }

        protected Image _image;

        public Image Image => _image;

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
        private VisualElement Root { get; }
        private Vector2 TargetStartPosition { get; set; }
        private Vector3 PointerStartPosition { get; set; }

        // Write a constructor to set target and store a reference to the
        // root of the visual tree.
        public ShapeDragManipulator(VisualElement target)
        {
            this.target = target;
            Root = target.parent;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        // This method stores the starting position of target and the pointer,
        // makes target capture the pointer, and denotes that a drag is now in progress.
        private void PointerDownHandler(PointerDownEvent evt)
        {
            TargetStartPosition = target.transform.position;
            PointerStartPosition = evt.position;
            target.CapturePointer(evt.pointerId);
            _enabled = true;
        }

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (!_enabled || !target.HasPointerCapture(evt.pointerId)) return;

            Vector3 pointerDelta = evt.position - PointerStartPosition;

            target.transform.position = new Vector2(
                Mathf.Clamp(TargetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                Mathf.Clamp(TargetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height)
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