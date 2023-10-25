
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    // DragAndDropManipulator is a manipulator that stores pointer-related callbacks, so it inherits from
    // PointerManipulator.
    public class FramesDropManipulator : PointerManipulator
    {
        private CycleElement _cycleElement;

        // The path of the stored asset, or the empty string if there isn't one.
        string assetPath = string.Empty;

        public FramesDropManipulator(CycleElement cycleElement)
        {
            _cycleElement = cycleElement;
            target = cycleElement.ScrollView;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            // Register callbacks for various stages in the drag process.
            target.RegisterCallback<DragEnterEvent>(OnDragEnter);
            target.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            // Unregister all callbacks that you registered in RegisterCallbacksOnTarget().

            target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
        }

        // This method runs if a user brings the pointer over the target while a drag is in progress.
        void OnDragEnter(DragEnterEvent _)
        {
            _cycleElement.HighlightDropArea();
        }

        // This method runs if a user makes the pointer leave the bounds of the target while a drag is in progress.
        void OnDragLeave(DragLeaveEvent _)
        {
            _cycleElement.DismissDropArea();
        }

        // This method runs every frame while a drag is in progress.
        void OnDragUpdate(DragUpdatedEvent _)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        // This method runs when a user drops a dragged object onto the target.
        void OnDragPerform(DragPerformEvent _)
        {
            _cycleElement.DismissDropArea();

            // Set droppedObject and draggedName fields to refer to dragged object.
            Object[] droppedObjects = DragAndDrop.objectReferences;

            if (droppedObjects.Length == 0)
            {
                return;
            }

            if (droppedObjects.Length == 1)
            {
                AddSingleFrame(droppedObjects[0]);
                return;
            }

            if (_cycleElement.Size > 0)
            {
                bool confirmed = EditorUtility.DisplayDialog(
                   $"Attention", // title
                   $"This will override the current frames. Proceed?",
                   "Yes", // OK button
                   "No" // Cancel button
                );

                if (!confirmed) return;
            }

            SetMultipleFrames(droppedObjects);
        }

        private void AddSingleFrame(Object droppedObject)
        {
            Sprite sprite = droppedObject as Sprite;
            if (sprite == null)
            {
                Debug.LogError("Dropped object is not a Sprite.");
                return;
            }
            _cycleElement.AddFrame(droppedObject as Sprite);
        }

        private void SetMultipleFrames(Object[] droppedObjects)
        {
            List<Sprite> sprites = new();

            foreach (Object droppedObject in droppedObjects)
            {
                if (droppedObject is Sprite)
                {
                    sprites.Add(droppedObject as Sprite);
                }
            }

            _cycleElement.SetFrames(sprites);
        }
    }
}