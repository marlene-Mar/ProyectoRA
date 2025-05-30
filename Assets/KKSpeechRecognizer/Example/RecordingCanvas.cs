﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using KKSpeech;

public class RecordingCanvas : MonoBehaviour
{
    public Button startRecordingButton;
    public TextMeshProUGUI resultText;  // Cambiado de Text a TextMeshProUGUI

    void Start()
    {
        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
            listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
            listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
            listener.onErrorDuringRecording.AddListener(OnError);
            listener.onErrorOnStartRecording.AddListener(OnError);
            listener.onFinalResults.AddListener(OnFinalResult);
            listener.onPartialResults.AddListener(OnPartialResult);
            listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
            SpeechRecognizer.RequestAccess();
        }
        else
        {
            resultText.text = "Sorry, but this device doesn't support speech recognition";
            startRecordingButton.enabled = false;
        }
    }

    public void OnFinalResult(string result)
    {
        // Cambiar la referencia al texto del botón para usar TextMeshProUGUI
        startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Recording";
        resultText.text = result;
        startRecordingButton.enabled = true;
    }

    public void OnPartialResult(string result)
    {
        resultText.text = result;
    }

    public void OnAvailabilityChange(bool available)
    {
        startRecordingButton.enabled = available;
        if (!available)
        {
            resultText.text = "Speech Recognition not available";
        }
        else
        {
            resultText.text = "Say something :-)";
        }
    }

    public void OnAuthorizationStatusFetched(AuthorizationStatus status)
    {
        switch (status)
        {
            case AuthorizationStatus.Authorized:
                startRecordingButton.enabled = true;
                break;
            default:
                startRecordingButton.enabled = false;
                resultText.text = "Cannot use Speech Recognition, authorization status is " + status;
                break;
        }
    }

    public void OnEndOfSpeech()
    {
        startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Recording";
    }

    public void OnError(string error)
    {
        Debug.LogError(error);
        startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Recording";
        startRecordingButton.enabled = true;
    }

    public void OnStartRecordingPressed()
    {
        if (SpeechRecognizer.IsRecording())
        {
#if UNITY_IOS && !UNITY_EDITOR
            SpeechRecognizer.StopIfRecording();
            startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stopping";
            startRecordingButton.enabled = false;
#elif UNITY_ANDROID && !UNITY_EDITOR
            SpeechRecognizer.StopIfRecording();
            startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Recording";
#endif
        }
        else
        {
            SpeechRecognizer.StartRecording(true);
            startRecordingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop Recording";
            resultText.text = "Say something :-)";
        }
    }
}
