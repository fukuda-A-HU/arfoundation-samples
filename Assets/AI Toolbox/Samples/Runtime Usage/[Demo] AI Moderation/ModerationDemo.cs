using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AiToolbox.Demo {
public class ModerationDemo : MonoBehaviour {
    public ModerationParameters parameters;

    [Space]
    public Color safeColor = new Color32(57, 224, 107, 255);
    public Color flaggedColor = new Color32(231, 138, 57, 255);

    [Space]
    public UIDocument uiDocument;
    public VisualTreeAsset messageTemplate;

    private int _currentMessageIndex;
    private readonly string[] _examples = {
        "Everyone hates playing with you. Just delete the game and unalive yourself.",
        "Haha GG EZ, you guys should uninstall the game.", "Wow, missed that shot? KYS, lol.",
        "You're so bad at this game, just quit.",
        "Only a smurf could play like that at this rank. Go back to your main.",
        "Your connection is not good enough to play here.",
        "I used to be an adventurer like you, then I took an arrow to the knee.",
        "Lag switch much? Your connection 'mysteriously' gets bad every time you're losing.",
        "Just planting the bomb in our spawn for fun, guys.", "You call that sniping? My grandma has better aim!",
        "Enjoy that teabag?", "Get rekt", "Stop feeding the enemy, or I'll report you.", "try finger but whole",
    };

    private readonly Dictionary<VisualElement, (string displayText, float highestScore)> _messageResponses = new();
    private VisualElement _selectedMessage;
    private Action _cancelCallback;

    private ScrollView _messagesScrollView;
    private Button _sendFreeformButton;
    private Button _sendPresetButton;
    private TextField _freeformInputField;
    private Label _messageStats;
    private Button _sortBySeverityButton;

    private void Start() {
        // Check if the API Key is set in the Inspector, just in case.
        if (parameters == null || string.IsNullOrEmpty(parameters.apiKey)) {
            var errorMessage = $"Please set the <b>API Key</b> on the <b>{parameters}</b> object.";
            var textfield = uiDocument.rootVisualElement.Q<Label>("header-text");
            textfield.text = errorMessage;
            textfield.style.color = new StyleColor(flaggedColor);
            return;
        }

        var dummyMessage = uiDocument.rootVisualElement.Q<VisualElement>("message-template");
        dummyMessage.style.display = DisplayStyle.None;

        _messagesScrollView = uiDocument.rootVisualElement.Q<ScrollView>("messages-scroll-view");
        _messageStats = uiDocument.rootVisualElement.Q<Label>("message-stats");

        _sendPresetButton = uiDocument.rootVisualElement.Q<Button>("send-preset-button");
        _sendPresetButton.tooltip = "Send one of the preset messages.";
        _sendPresetButton.clickable.clicked += SendPresetMessage;

        _sendFreeformButton = uiDocument.rootVisualElement.Q<Button>("send-freeform-button");
        _sendFreeformButton.tooltip = "Send the message typed in the input field.";
        _sendFreeformButton.clickable.clicked += SendFreeformMessage;
        _sendFreeformButton.SetEnabled(false);

        _freeformInputField = uiDocument.rootVisualElement.Q<TextField>();
        _freeformInputField.tooltip = "Type a test message to check for toxicity.";
        _freeformInputField.RegisterValueChangedCallback(evt => {
            _sendFreeformButton.SetEnabled(!string.IsNullOrEmpty(evt.newValue));
        });
        _freeformInputField.RegisterCallback<KeyDownEvent>(evt => {
            // Send the message when the user presses Enter.
            if (evt.keyCode == KeyCode.Return && !string.IsNullOrEmpty(_freeformInputField.value)) {
                if (evt.modifiers == EventModifiers.Shift) {
                    _freeformInputField.value += "\n";
                } else {
                    SendFreeformMessage();
                }
            }

            // Use the up and down arrows to cycle through the preset messages.
            {
                if (evt.keyCode == KeyCode.UpArrow && !string.IsNullOrEmpty(_freeformInputField.value)) {
                    _freeformInputField.value = _examples[_currentMessageIndex];
                    _currentMessageIndex = (_currentMessageIndex + 1) % _examples.Length;
                }

                if (evt.keyCode == KeyCode.DownArrow && !string.IsNullOrEmpty(_freeformInputField.value)) {
                    _freeformInputField.value = _examples[_currentMessageIndex];
                    _currentMessageIndex = (_currentMessageIndex - 1 + _examples.Length) % _examples.Length;
                }
            }
        });

        _sortBySeverityButton = uiDocument.rootVisualElement.Q<Button>("sort-messages-button");
        _sortBySeverityButton.tooltip = "Sort all messages in order of increasing severity score.";
        _sortBySeverityButton.clickable.clicked += () => {
            var messages = _messageResponses.Keys.ToList();
            messages.Sort((a, b) => _messageResponses[a].highestScore.CompareTo(_messageResponses[b].highestScore));
            foreach (var message in messages) {
                _messagesScrollView.Add(message);
            }
        };
        _sortBySeverityButton.SetEnabled(false);
    }

    private void OnDestroy() {
        _cancelCallback?.Invoke();
        if (_sendPresetButton != null) _sendPresetButton.clickable.clicked -= SendPresetMessage;
        if (_sendFreeformButton != null) _sendFreeformButton.clickable.clicked -= SendFreeformMessage;
    }

    private void SendFreeformMessage() {
        var messageElement = messageTemplate.CloneTree();
        _messagesScrollView.Add(messageElement);
        SendRequest(_freeformInputField.text, messageElement);
        _freeformInputField.value = string.Empty;
    }

    private void SendPresetMessage() {
        var messageElement = messageTemplate.CloneTree();
        _messagesScrollView.Add(messageElement);
        SendRequest(_examples[_currentMessageIndex++ % _examples.Length], messageElement);
    }

    private void SendRequest(string input, VisualElement messageElement) {
        _sendPresetButton.SetEnabled(false);

        var messageLabel = messageElement.Q<Label>("message-text");
        messageLabel.text = input;

        messageElement.Q<VisualElement>("icon-pending").style.display = DisplayStyle.Flex;
        messageElement.Q<VisualElement>("icon-positive").style.display = DisplayStyle.None;
        messageElement.Q<VisualElement>("icon-negative").style.display = DisplayStyle.None;

        messageElement.RegisterCallback<MouseEnterEvent>(_ => {
            SelectMessage(messageElement);
        });
        _messagesScrollView.ScrollTo(messageElement);

        _cancelCallback = Moderation.Request(input, parameters, completeCallback: response => {
            var highestScore = response.GetHighestScore();
            const float scoreThreshold = 0.45f;
            var flagged = response.results.Any(result => result.flagged) || highestScore > scoreThreshold;

            // Set message icon.
            {
                messageElement.Q<VisualElement>("icon-pending").style.display = DisplayStyle.None;
                var iconName = flagged ? "icon-negative" : "icon-positive";
                messageElement.Q<VisualElement>(iconName).style.display = DisplayStyle.Flex;
            }

            string t = string.Empty;

            // Display highest score.
            {
                var color = highestScore > 0.01f ? flaggedColor : safeColor;
                var colorString = $"#{ColorUtility.ToHtmlStringRGB(color)}";
                t += $"<b>Highest severity: <color={colorString}>{highestScore:0.000}</color></b>\n\n";
            }

            // Display message stats.
            {
                t += string.Join("\n", response.results.Select(result => result.ToString(safeColor, flaggedColor)));
            }

            _messageStats.text = t;
            _messageResponses[messageElement] = (t, highestScore);
            SelectMessage(messageElement);

            _sendPresetButton.SetEnabled(true);
            _sortBySeverityButton.SetEnabled(_messageResponses.Count >= 2);
        }, failureCallback: (errorCode, errorMessage) => {
            var errorType = (ErrorCodes)errorCode;
            messageElement.Q<VisualElement>("icon-pending").style.display = DisplayStyle.None;
            messageElement.Q<VisualElement>("icon-negative").style.display = DisplayStyle.Flex;
            _messageStats.text = $"Error {errorCode}: {errorType} - {errorMessage}";

            _sendPresetButton.SetEnabled(true);
        });
    }

    private void SelectMessage(VisualElement messageElement) {
        _selectedMessage?.RemoveFromClassList("selected");
        _selectedMessage = messageElement;
        _messageStats.text = _messageResponses[messageElement].displayText;
        _selectedMessage.AddToClassList("selected");
    }
}
}