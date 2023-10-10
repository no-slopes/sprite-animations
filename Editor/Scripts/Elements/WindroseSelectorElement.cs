using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using static SpriteAnimations.SpriteAnimationWindrose;

namespace SpriteAnimations.Editor
{
    public delegate void DirectionSelectedEvent(WindroseDirection direction);
    public class WindroseSelectorElement : VisualElement
    {
        #region Fields

        private List<Button> _buttons;
        private Label _windroseLabel;

        #endregion

        #region Constructors

        public WindroseSelectorElement()
        {
            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>("UI Documents/WindroseSelector");
            TemplateContainer template = tree.Instantiate();

            _buttons = template.Query<Button>(className: "windrose-button").ToList();
            _buttons.ForEach(button => button.clicked += () => OnButtonClicked(button));
            ResetButtons();

            _windroseLabel = template.Q<Label>("windrose-label");

            Add(template);
        }

        #endregion

        #region Flow

        public void Initialize()
        {
            if (!TryGetButton("east", out Button eastButton))
            {
                Logger.LogError("Could not find east button!");
            }

            eastButton.AddToClassList("selected");
        }

        #endregion

        #region Buttons

        private void ResetButtons()
        {
            _buttons.ForEach(button => button.RemoveFromClassList("selected"));
        }

        private bool TryGetButton(string windroseString, out Button button)
        {
            button = _buttons.FirstOrDefault(button => button.viewDataKey == windroseString);
            return button != null;
        }

        #endregion

        #region Direction

        public event DirectionSelectedEvent DirectionSelected;

        private void OnButtonClicked(Button button)
        {
            if (!EvaluateWindroseDirection(button.viewDataKey, out WindroseDirection direction))
            {
                Logger.LogError("Could not evaluate windrose direction! from button: " + button.name);
                return;
            }

            ResetButtons();
            button.AddToClassList("selected");
            _windroseLabel.text = direction.ToString();
            DirectionSelected?.Invoke(direction);
        }

        private bool EvaluateWindroseDirection(string windroseString, out WindroseDirection direction)
        {
            switch (windroseString)
            {
                case "north":
                    direction = WindroseDirection.North;
                    return true;
                case "northeast":
                    direction = WindroseDirection.NorthEast;
                    return true;
                case "east":
                    direction = WindroseDirection.East;
                    return true;
                case "southeast":
                    direction = WindroseDirection.SouthEast;
                    return true;
                case "south":
                    direction = WindroseDirection.South;
                    return true;
                case "southwest":
                    direction = WindroseDirection.SouthWest;
                    return true;
                case "west":
                    direction = WindroseDirection.West;
                    return true;
                default:
                    direction = WindroseDirection.East;
                    return false;
            }
        }

        #endregion
    }
}