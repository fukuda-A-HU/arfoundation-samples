using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace AiToolbox.Demo {
[RequireComponent(typeof(AudioSource))]
public class TextToSpeechDemo : MonoBehaviour {
    public TextToSpeechParameters parameters;
    public UIDocument uiDocument;

    private Button _stopButton;
    private Button _processButton;
    private Button _exportButton;
    private TextField _inputField;
    private Label _messageLabel;
    private DropdownField _voiceDropdown;

    private Action _cancelCallback;
    private AudioSource _audioSource;

    private void Start() {
        _audioSource = gameObject.GetComponent<AudioSource>();

        var root = uiDocument.rootVisualElement;

        _voiceDropdown = root.Q<DropdownField>("voice-dropdown");
        Debug.Assert(_voiceDropdown != null, "Voice dropdown not found in UI.");
        _voiceDropdown.RegisterCallback<ChangeEvent<string>>(evt => {
            var voice = (TextToSpeechParameters.Voice)Enum.Parse(typeof(TextToSpeechParameters.Voice), evt.newValue);
            parameters.voice = voice;
        });
        foreach (var voice in Enum.GetNames(typeof(TextToSpeechParameters.Voice))) {
            _voiceDropdown.choices.Add(voice);
        }

        _voiceDropdown.index = (int)parameters.voice;

        _inputField = root.Q<TextField>("input-text");
        Debug.Assert(_inputField != null, "Input text field not found in UI.");

        _stopButton = root.Q<Button>("stop-button");
        Debug.Assert(_stopButton != null, "Stop button not found in UI.");
        _stopButton.clickable.clicked += StopPlayback;
        _stopButton.SetEnabled(false);

        _processButton = root.Q<Button>("process-button");
        Debug.Assert(_processButton != null, "Process button not found in UI.");
        _processButton.clickable.clicked += SendRequest;

        _exportButton = root.Q<Button>("export-button");
        Debug.Assert(_exportButton != null, "Export button not found in UI.");
        _exportButton.clickable.clicked += () => {
            var audioClip = _audioSource.clip;
            if (audioClip == null) return;
#if UNITY_EDITOR
            var basePath = Application.dataPath;
#else
            var basePath = Application.persistentDataPath;
#endif
            var data = AudioUtility.GetWavData(audioClip);
            var path = $"{basePath}/text-to-speech_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.wav";
            File.WriteAllBytes(path, data);
            Debug.Log($"<b>Exported:</b> {path}");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.RevealInFinder(path);
#endif
        };

        _messageLabel = root.Q<Label>("message-label");
        Debug.Assert(_messageLabel != null, "Message label not found in UI.");
        _messageLabel.text = $"Type your text and press <b>{_processButton.text}</b> to begin transcription.";
    }

    private void OnDestroy() {
        _cancelCallback?.Invoke();
    }

    private void SendRequest() {
        if (_cancelCallback != null) {
            _cancelCallback();
            _cancelCallback = null;
        }

        var text = _inputField.text;
        _cancelCallback = TextToSpeech.Request(text, parameters, audioClip => {
            _messageLabel.text = "<b>Done!</b>";
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }, (errorCode, errorMessage) => {
            _messageLabel.text = $"<color=red><b>Error {errorCode}:</b></color> {errorMessage}";
        });
    }

    private void StopPlayback() {
        _audioSource.Stop();
    }

    private void Update() {
        _stopButton?.SetEnabled(_audioSource.isPlaying);
        _exportButton?.SetEnabled(_audioSource.clip != null);
    }
}
}