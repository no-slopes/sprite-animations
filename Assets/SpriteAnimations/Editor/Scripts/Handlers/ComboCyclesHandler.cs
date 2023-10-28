using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public class ComboCyclesHandler
    {
        #region Fields

        protected SpriteAnimationCombo _animation;
        protected ListView _cyclesList;
        protected Button _createButton;

        #endregion

        #region Constructors

        public ComboCyclesHandler(ListView cyclesList, Button createButton)
        {
            _cyclesList = cyclesList;
            _createButton = createButton;
            _createButton.clicked += OnCreateButtonClicked;
        }

        #endregion

        #region Initialization

        public void Initialize(SpriteAnimationCombo animation)
        {
            _animation = animation;

            _cyclesList.makeItem = () => new ComboCycleElement(this);

            _cyclesList.bindItem = (e, i) =>
            {
                ComboCycleElement item = e as ComboCycleElement;
                item.SetCycle(animation.GetCycle(i), i);
            };

            _cyclesList.itemsSource = _animation.Cycles;
            _cyclesList.SetSelection(0);
        }

        public void Dismiss()
        {
            _cyclesList?.Clear();
        }

        #endregion

        #region Cycles

        private void OnCreateButtonClicked()
        {
            if (_animation == null) return;

            _animation.CreateCycle();
            _cyclesList.Rebuild();
            _cyclesList.SetSelection(_cyclesList.itemsSource.Count - 1);
        }

        public void DeleteCycle(int index)
        {
            if (_animation.Cycles.Count < 2)
            {
                Logger.LogWarning("Cannot delete the last cycle of a Combo Animation");
                return;
            }

            bool needToReleselect = _cyclesList.selectedIndex == index;

            _animation.RemoveCycle(index);
            _cyclesList.Rebuild();

            if (needToReleselect)
                _cyclesList.SetSelection(0);
        }

        #endregion

        #region Classes

        private class ComboCycleElement : VisualElement
        {
            #region Fields

            private ComboCyclesHandler _handler;

            private Cycle _cycle;
            private Label _label;
            private Button _deleteButton;
            private int _index = -1;

            #endregion

            #region Properties

            public Cycle Cycle => _cycle;
            public Label Label => _label;

            #endregion

            #region Constructors

            public ComboCycleElement(ComboCyclesHandler handler)
            {
                _handler = handler;

                VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/ComboCycleListItem");
                TemplateContainer template = tree.Instantiate();

                _label = template.Q<Label>();
                _deleteButton = template.Q<Button>("delete-button");

                _deleteButton.clicked += Delete;

                AddToClassList("interactable");
                Add(template);
            }

            #endregion

            #region Cycle

            public void SetCycle(Cycle cycle, int index)
            {
                _cycle = cycle;
                _index = index;
                _label.text = $"Cycle {index}";
            }

            #endregion

            #region Deleting

            private void Delete()
            {
                if (_index < 0) return;
                _handler.DeleteCycle(_index);
            }

            #endregion
        }

        #endregion
    }
}