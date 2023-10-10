
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpriteAnimations.Editor
{
    public delegate void AnimationSelectedEvent(SpriteAnimation animation);

    public class SidebarElement : VisualElement
    {
        #region Fields

        private List<SpriteAnimation> _animations;
        private SpriteAnimation _currentAnimation;

        private TemplateContainer _root;
        private ListView _animationsListView;
        private Button _createAnimationButton;

        private CreateAnimationWindow _createAnimationWindow;

        #endregion

        #region Constructors

        public SidebarElement()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/Sidebar");
            _root = tree.Instantiate();

            _animationsListView = _root.Q<ListView>("animations-list");

            _animationsListView.makeItem = () => new AnimationListItemElement();
            _animationsListView.bindItem = (e, i) =>
            {
                AnimationListItemElement item = e as AnimationListItemElement;
                item.Animation = _animations[i];
            };

            _animationsListView.selectedIndicesChanged += OnListItemsSelected;

            _animationsListView.fixedItemHeight = 50;
            _animationsListView.reorderable = true;

            _createAnimationButton = _root.Q<Button>("create-animation-button");
            _createAnimationButton.clicked += OnAnimationButtonClicked;

            Add(_root);
        }

        #endregion

        #region Initialization

        public void Initialize(List<SpriteAnimation> animations)
        {
            _animations = animations;
            _animationsListView.itemsSource = _animations;
            Reload();
        }

        public void Dismiss()
        {
            _animations.Clear();
            _animationsListView.Clear();
        }

        public void Reload()
        {
            _animationsListView.Rebuild();

            if (_animations.Count > 0)
            {
                SelectAnimation(_animations[0]);
                _animationsListView.SetSelection(0);
            }
        }

        #endregion

        #region Animations

        private void SelectAnimation(SpriteAnimation animation)
        {
            _currentAnimation = animation;
            AnimationSelected?.Invoke(animation);
        }

        private void OnAnimationCreated(SpriteAnimation animation)
        {
            _createAnimationWindow?.Close();
            _createAnimationWindow = null;

            _animations.Add(animation);
            _animationsListView.Rebuild();
            SelectAnimation(animation);
            _animationsListView.SetSelection(_animations.Count - 1);
        }

        private void OnListItemsSelected(IEnumerable<int> enumerable)
        {
            int[] indexes = enumerable.ToArray();

            if (indexes.Length <= 0) return;

            SelectAnimation(_animations[indexes[0]]);
        }


        #endregion

        #region Events

        public event AnimationSelectedEvent AnimationSelected;

        private void OnAnimationButtonClicked()
        {
            if (_createAnimationWindow != null) return;
            _createAnimationWindow = CreateAnimationWindow.OpenEditorWindow();
            _createAnimationWindow.AnimationCreated += OnAnimationCreated;
        }

        #endregion
    }
}