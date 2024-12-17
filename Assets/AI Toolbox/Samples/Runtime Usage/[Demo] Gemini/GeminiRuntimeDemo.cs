using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AiToolbox.Demo {
// This script demonstrates how to use the Gemini API in a Unity project. It asks the AI to create a funny character
// description and then generates a conversation with the character. The user can ask questions to the character, and
// the AI will generate answers. The script uses the GeminiParameters class to store the API key and other settings.
public class GeminiRuntimeDemo : MonoBehaviour {
    public GeminiParameters parameters;

    [Space]
    public Text characterDescription;
    public ScrollRect scrollRect;

    [Space]
    public GameObject characterTextPrefab;
    public GameObject userTextPrefab;

    [Space]
    public Button userQuestionButton;

    private GeminiParameters _parametersWithCharacterRole;
    private readonly List<Message> _conversationSoFar = new List<Message>();

    private void Start() {
        // Check if the API Key is set in the Inspector, just in case.
        if (parameters == null || string.IsNullOrEmpty(parameters.apiKey)) {
            const string errorMessage = "Please set the <b>API Key</b> in the <b>Gemini Dialogue</b> Game Object.";
            characterDescription.text = errorMessage;
            characterDescription.color = Color.magenta;
            return;
        }

        const string prompt = "Create a one-sentence description of a funny random character for a video game. " +
                              "Answer in format: [name], [description]";

        // This request provides only `completeCallback` and `failureCallback` parameters. Since the `updateCallback`
        // is not provided, the request will be completed in one step, and the `completeCallback` will be called only
        // once, with the full text of the answer.
        Gemini.Request(prompt, parameters, completeCallback: text => {
            // We've received the full text of the answer, so we can display it in the "You're chatting with" text.
            characterDescription.text = text;

            // Create a new Parameters object with the `role` parameter set to the character description we've received.
            // Now, all the requests made with this Parameters object will be made in the context of this character.
            _parametersWithCharacterRole = new GeminiParameters(parameters) { role = text };

            // Ask AI to introduce itself. Note that the message does not contain the character description, because
            // the `role` parameter is already set in the `_parametersWithCharacterRole` object.
            _conversationSoFar.Add(new Message("Introduce yourself as the character.", Role.User));
            AddNpcAnswer();
        }, failureCallback: (errorCode, errorMessage) => {
            // If the request fails, display the error message in the "You're chatting with" text.
            var errorType = (ErrorCodes)errorCode;
            characterDescription.text = $"Error {errorCode}: {errorType} - {errorMessage}";
            characterDescription.color = Color.red;
        });

        userQuestionButton.onClick.AddListener(AddUserQuestion);
        userQuestionButton.interactable = false;
    }

    private void AddUserQuestion() {
        // Create a new text field for the user's question.
        var textField = Instantiate(userTextPrefab, scrollRect.content).GetComponent<Text>();
        textField.rectTransform.sizeDelta =
            new Vector2(scrollRect.content.rect.width - textField.rectTransform.localPosition.x, 0);
        userQuestionButton.interactable = false;

        // Create a new list of messages, which contains all the messages from the conversation so far, plus ask it
        // to create a question for the character. We can't reuse the `_conversationSoFar` list, because generating
        // questions is a separate conversation, and we don't want to mix the two.
        var conversationAndRequest = new List<Message>(_conversationSoFar) {
            new Message($"Create a question for {characterDescription.text}.", Role.User)
        };
        Gemini.Request(conversationAndRequest, parameters, completeCallback: questionFullText => {
            // The answer is complete, so we can add it to the conversation and ask the AI to answer it.
            textField.text = questionFullText;
            scrollRect.normalizedPosition = new Vector2(0, 0);
            _conversationSoFar.Add(new Message(questionFullText, Role.User));
            AddNpcAnswer();
        }, failureCallback: (errorCode, errorMessage) => {
            var errorType = (ErrorCodes)errorCode;
            textField.text = $"Error {errorCode}: {errorType} - {errorMessage}";
            textField.color = Color.red;
        });
    }

    private void AddNpcAnswer() {
        // Create a new text field for the NPC's answer.
        var textField = Instantiate(characterTextPrefab, scrollRect.content).GetComponent<Text>();
        textField.rectTransform.sizeDelta =
            new Vector2(scrollRect.content.rect.width - textField.rectTransform.localPosition.x, 0);
        scrollRect.normalizedPosition = new Vector2(0, 0);

        // Ask the AI to answer the last question in the conversation.
        Gemini.Request(_conversationSoFar, _parametersWithCharacterRole, completeCallback: fullAnswer => {
            // The answer is complete, so we can add it to the conversation and enable the "Generate question" button.
            textField.text = fullAnswer;
            scrollRect.normalizedPosition = new Vector2(0, 0);
            userQuestionButton.interactable = true;
            _conversationSoFar.Add(new Message(fullAnswer, Role.AI));
        }, failureCallback: (errorCode, errorMessage) => {
            var errorType = (ErrorCodes)errorCode;
            textField.text = $"Error {errorCode}: {errorType} - {errorMessage}";
            textField.color = Color.red;
        });
    }
}
}