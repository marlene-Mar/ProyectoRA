using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System;
using System.Collections;

public class VoiceController : MonoBehaviour
{
    [SerializeField] private int micStatus = 0;
    [SerializeField] private int permissionStatus = 0;

    private bool isMicrophoneOn = false;
    private bool hasMicPermission = false;
    private AudioClip microphoneClip;
    private string selectedMicrophone;

    [Header("UI y animaciones")]
    [SerializeField] private GameObject PermisosUsuario;
    [SerializeField] private Button btnActivarMic;
    [SerializeField] private Button btnDesactivarMic;
    [SerializeField] private Button btnSentado;
    [SerializeField] private Button btnPata;
    [SerializeField] private GameObject indicadorMicActivo;
    [SerializeField] private GameObject indicadorMicInactivo;
    [SerializeField] private Animator characterAnimator;

    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");

    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
            Debug.LogWarning("Este script está diseñado para Android.");

        // Asignar botones
        if (btnActivarMic != null) btnActivarMic.onClick.AddListener(ActivarMicrofono);
        if (btnDesactivarMic != null) btnDesactivarMic.onClick.AddListener(DesactivarMicrofono);
        if (btnSentado != null) btnSentado.onClick.AddListener(() => ExecuteVoiceCommand("sentado"));
        if (btnPata != null) btnPata.onClick.AddListener(() => ExecuteVoiceCommand("pata"));

        RequestMicrophonePermission();
        UpdateButtonUI();
    }

    public void RequestMicrophonePermission()
    {
        if (PermisosUsuario != null)
            PermisosUsuario.SetActive(true);

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            StartCoroutine(CheckPermissionAfterRequest());
        }
        else
        {
            hasMicPermission = true;
            permissionStatus = 1;
            if (PermisosUsuario != null) PermisosUsuario.SetActive(false);
            InitializeMicrophone();
        }
    }

    IEnumerator CheckPermissionAfterRequest()
    {
        yield return new WaitForSeconds(0.5f);

        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            hasMicPermission = true;
            permissionStatus = 1;
            if (PermisosUsuario != null) PermisosUsuario.SetActive(false);
            InitializeMicrophone();
        }
        else
        {
            Debug.LogWarning("Permiso de micrófono no concedido.");
            hasMicPermission = false;
            permissionStatus = 0;
        }

        UpdateButtonUI();
    }

    void InitializeMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            selectedMicrophone = Microphone.devices[0];
            Debug.Log("Micrófono detectado: " + selectedMicrophone);
        }
        else
        {
            Debug.LogError("No se encontraron micrófonos.");
        }
    }

    public void ActivarMicrofono()
    {
        if (!hasMicPermission)
        {
            RequestMicrophonePermission();
            return;
        }

        if (!isMicrophoneOn)
        {
            isMicrophoneOn = true;
            StartMicrophone();
            UpdateButtonUI();
        }
    }

    public void DesactivarMicrofono()
    {
        if (isMicrophoneOn)
        {
            isMicrophoneOn = false;
            StopMicrophone();
            UpdateButtonUI();
        }
    }

    void StartMicrophone()
    {
        try
        {
            if (selectedMicrophone != null)
            {
                microphoneClip = Microphone.Start(selectedMicrophone, true, 10, 44100);
                Debug.Log("Micrófono activado.");
                micStatus = 1;

                if (indicadorMicActivo != null) indicadorMicActivo.SetActive(true);
                if (indicadorMicInactivo != null) indicadorMicInactivo.SetActive(false);
            }
            else
            {
                Debug.LogError("No hay micrófono disponible.");
                isMicrophoneOn = false;
                micStatus = 0;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al activar micrófono: " + e.Message);
            isMicrophoneOn = false;
            micStatus = 0;
        }
    }

    void StopMicrophone()
    {
        try
        {
            if (selectedMicrophone != null && Microphone.IsRecording(selectedMicrophone))
            {
                Microphone.End(selectedMicrophone);
                Debug.Log("Micrófono detenido.");
                micStatus = 0;

                if (indicadorMicActivo != null) indicadorMicActivo.SetActive(false);
                if (indicadorMicInactivo != null) indicadorMicInactivo.SetActive(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al detener micrófono: " + e.Message);
        }
    }

    void UpdateButtonUI()
    {
        permissionStatus = hasMicPermission ? 1 : 0;
        micStatus = isMicrophoneOn ? 1 : 0;

        if (btnActivarMic != null)
            btnActivarMic.interactable = hasMicPermission && !isMicrophoneOn;

        if (btnDesactivarMic != null)
            btnDesactivarMic.interactable = hasMicPermission && isMicrophoneOn;

        if (indicadorMicActivo != null)
            indicadorMicActivo.SetActive(isMicrophoneOn);

        if (indicadorMicInactivo != null)
            indicadorMicInactivo.SetActive(!isMicrophoneOn);

        Debug.Log($"Estado micrófono: Permiso={permissionStatus}, Activo={micStatus}");
    }

    public void ExecuteVoiceCommand(string command)
    {
        switch (command.ToLower())
        {
            case "sentado":
                characterAnimator?.SetTrigger(triggerSentado);
                break;
            case "pata":
                characterAnimator?.SetTrigger(triggerPata);
                break;
            default:
                Debug.LogWarning("Comando desconocido: " + command);
                break;
        }
    }
}
