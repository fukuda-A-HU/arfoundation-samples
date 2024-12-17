using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace AiToolbox.Demo {
[RequireComponent(typeof(AudioSource))]
public class SpeechToTextDemo : MonoBehaviour {
    public SpeechToTextParameters parameters;
    public UIDocument uiDocument;

    private bool _isRecording;
    private DropdownField _micDropdown;
    private Button _startStopButton;
    private Button _playButton;
    private Button _transcribeButton;
    private Label _message;
    private Action _cancelCallback;
    private string _startButtonText;
    private AudioSource _audioSource;

    private const int RecordingLengthSeconds = 20;
    private const int RecordingFrequency = 16000;
    private const string UserMicDeviceIndex = "AiToolbox_SpeechToTextDemo_MicDeviceIndex";

    private void Start() {
        Debug.Assert(Microphone.devices.Length > 0, "No microphone devices found.");

        _audioSource = gameObject.GetComponent<AudioSource>();
        var root = uiDocument.rootVisualElement;

        _micDropdown = root.Q<DropdownField>("mic-dropdown");
        Debug.Assert(_micDropdown != null, "Mic dropdown not found in UI.");
        foreach (var device in Microphone.devices) {
            _micDropdown.choices.Add(device);
        }

        _micDropdown.RegisterCallback<ChangeEvent<string>>(evt => {
            var index = _micDropdown.choices.IndexOf(evt.newValue);
            PlayerPrefs.SetInt(UserMicDeviceIndex, index);
        });
        var micIndex = PlayerPrefs.GetInt(UserMicDeviceIndex, 0);
        _micDropdown.index = micIndex;

        _startStopButton = root.Q<Button>("start-stop-button");
        Debug.Assert(_startStopButton != null, "Start/stop button not found in UI.");
        _startStopButton.clickable.clicked += ToggleRecording;
        _startButtonText = _startStopButton.text;

        _playButton = root.Q<Button>("play-button");
        Debug.Assert(_playButton != null, "Play button not found in UI.");
        _playButton.clickable.clicked += PlayRecording;
        _playButton.SetEnabled(false);

        _transcribeButton = root.Q<Button>("transcribe-button");
        Debug.Assert(_transcribeButton != null, "Transcribe button not found in UI.");
        _transcribeButton.clickable.clicked += Transcribe;

        _message = root.Q<Label>("result-text");
        Debug.Assert(_message != null, "Result text not found in UI.");
        _message.text = $"Press <b>{_startStopButton.text}</b> to begin recording.";
    }

    private void OnDestroy() {
        PlayerPrefs.Save();
        if (_startStopButton != null) _startStopButton.clickable.clicked -= ToggleRecording;
        if (_isRecording) Microphone.End(null);
    }

    private void ToggleRecording() {
        if (_cancelCallback != null) {
            _cancelCallback();
            _cancelCallback = null;
        }

        var index = PlayerPrefs.GetInt(UserMicDeviceIndex);
        var deviceName = Microphone.devices[index];

        if (_isRecording) {
            // Stop recording.
            _isRecording = false;
            _message.text = "Press <b>Transcribe</b> to begin transcription.";
            _startStopButton.text = _startButtonText;
            _playButton.SetEnabled(true);

            Microphone.End(deviceName);
            _audioSource.clip = AudioUtility.TrimSilence(_audioSource.clip, 0.01f);
        } else {
            // Start recording.
            _isRecording = true;
            _startStopButton.text = "Stop";
            _playButton.SetEnabled(false);

            _audioSource.clip = Microphone.Start(deviceName, false, RecordingLengthSeconds, RecordingFrequency);
        }
    }

    private void PlayRecording() {
        _audioSource.Play();
    }

    private void Transcribe() {
        _cancelCallback = SpeechToText.Request(_audioSource.clip, parameters, response => {
            _message.text = response.text;
            FinishTranscription();
        }, (errorCode, errorMessage) => {
            _message.text = $"<color=red><b>Error {errorCode}:</b></color> {errorMessage}";
            FinishTranscription();
        });
    }

    private void FinishTranscription() {
        _startStopButton.text = _startButtonText;
        _cancelCallback = null;
    }
}
}