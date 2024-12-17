using UnityEngine;
using AiToolbox;
using System;

[RequireComponent(typeof(AudioSource))]
public class SpeechToText : MonoBehaviour
{
    public SpeechToTextParameters parameters;
    private AudioSource _audioSource;

    private Action _cancelCallback;

    private void Transcribe() {
        _cancelCallback = AiToolbox.SpeechToText.Request(_audioSource.clip, parameters, response => {
            Debug.Log(response.text);
            FinishTranscription();
        }, (errorCode, errorMessage) => {
            Debug.Log($"<color=red><b>Error {errorCode}:</b></color> {errorMessage}");
            FinishTranscription();
        });
    }

    private void FinishTranscription() {
        _cancelCallback = null;
    }
}
