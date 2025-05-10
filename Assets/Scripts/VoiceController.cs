using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android; //Permisos en Android
using UnityEngine.UI; // Para referencia a componentes UI

public class VoiceController : MonoBehaviour
{
    // Variables de control
    [SerializeField] private int micStatus = 0; // 0 = mic apagado, 1 = mic encendido
    [SerializeField] private int permissionStatus = 0; // 0=sin permiso, 1=con permiso

    private bool isMicrophoneOn = false; //indica si el microfono esta activo
    private bool hasMicPermission = false; //indica si el microfono tiene permiso
    private AudioClip microphoneClip; //almacena el audio grabado 
    private string selectedMicrophone; //nombre del microfono usado

    //Panel de solicitud de permisos de microfono 
    [SerializeField] private GameObject PermisosUsuario;

    // Referencias a los botones
    [SerializeField] private Button btnActivarMic;
    [SerializeField] private Button btnDesactivarMic;

    // Referencias opcionales para retroalimentaci�n visual
    [SerializeField] private GameObject indicadorMicActivo;
    [SerializeField] private GameObject indicadorMicInactivo;

    public void ShowOnlyPanel(GameObject panelToShow)
    {
        PermisosUsuario.SetActive(panelToShow == PermisosUsuario);
    }

    void Start()
    {
        // Verifica si la plataforma es Android
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.LogWarning("Este script est� dise�ado para dispositivos Android.");
        }

        // Asignar listeners a los botones si est�n referenciados
        if (btnActivarMic != null)
        {
            btnActivarMic.onClick.AddListener(ActivarMicrofono);
        }

        if (btnDesactivarMic != null)
        {
            btnDesactivarMic.onClick.AddListener(DesactivarMicrofono);
        }

        // Solicita permisos de micr�fono al inicio
        RequestMicrophonePermission();

        // Actualiza la UI inicial
        UpdateButtonUI();
    }

    // M�todo para activar el micr�fono (para el bot�n de activar)
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

    // M�todo para desactivar el micr�fono (para el bot�n de desactivar)
    public void DesactivarMicrofono()
    {
        if (isMicrophoneOn)
        {
            isMicrophoneOn = false;
            StopMicrophone();
            UpdateButtonUI();
        }
    }

    public void RequestMicrophonePermission()
    {
        if (PermisosUsuario != null)
        {
            PermisosUsuario.SetActive(true);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            StartCoroutine(CheckPermissionAfterRequest());
        }
        else
        {
            hasMicPermission = true;
            permissionStatus = 1; // Permiso concedido

            if (PermisosUsuario != null)
            {
                PermisosUsuario.SetActive(false);
            }

            InitializeMicrophone();
        }
    }

    IEnumerator CheckPermissionAfterRequest()
    {
        // Espera un momento para que el usuario responda a la solicitud de permiso
        yield return new WaitForSeconds(0.5f);

        // Verifica si el permiso fue concedido
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            hasMicPermission = true;
            permissionStatus = 1; // Permiso concedido

            if (PermisosUsuario != null)
            {
                PermisosUsuario.SetActive(false);
            }

            InitializeMicrophone();
        }
        else
        {
            Debug.LogWarning("El usuario no concedi� permiso para usar el micr�fono.");
            hasMicPermission = false;
            permissionStatus = 0; // Permiso denegado
        }

        UpdateButtonUI();
    }

    void InitializeMicrophone()
    {
        // Obtiene la lista de micr�fonos disponibles
        if (Microphone.devices.Length > 0)
        {
            selectedMicrophone = Microphone.devices[0]; // Usa el primer micr�fono disponible
            Debug.Log("Micr�fono seleccionado: " + selectedMicrophone);
        }
        else
        {
            Debug.LogError("No se detectaron micr�fonos en el dispositivo.");
        }
    }

    public void ToggleMicrophone()
    {
        if (!hasMicPermission)
        {
            RequestMicrophonePermission();
            return;
        }

        isMicrophoneOn = !isMicrophoneOn;

        if (isMicrophoneOn)
        {
            StartMicrophone();
        }
        else
        {
            StopMicrophone();
        }

        UpdateButtonUI();
    }

    void StartMicrophone()
    {
        try
        {
            if (selectedMicrophone != null)
            {
                // Iniciar grabaci�n con el micr�fono (longitud de 10 segundos con loop)
                microphoneClip = Microphone.Start(selectedMicrophone, true, 10, 44100);
                Debug.Log("Micr�fono activado");
                micStatus = 1;

                // Actualizar indicadores visuales si existen
                if (indicadorMicActivo != null) indicadorMicActivo.SetActive(true);
                if (indicadorMicInactivo != null) indicadorMicInactivo.SetActive(false);
            }
            else
            {
                Debug.LogError("No hay micr�fono seleccionado.");
                isMicrophoneOn = false;
                micStatus = 0;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al iniciar el micr�fono: " + e.Message);
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
                Debug.Log("Micr�fono desactivado");
                micStatus = 0;

                // Actualizar indicadores visuales si existen
                if (indicadorMicActivo != null) indicadorMicActivo.SetActive(false);
                if (indicadorMicInactivo != null) indicadorMicInactivo.SetActive(true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al detener el micr�fono: " + e.Message);
        }
    }

    private void UpdateButtonUI()
    {
        // Actualiza las variables int seg�n el estado actual
        permissionStatus = hasMicPermission ? 1 : 0;
        micStatus = isMicrophoneOn ? 1 : 0;

        // Actualiza estados de los botones
        if (btnActivarMic != null)
        {
            btnActivarMic.interactable = hasMicPermission && !isMicrophoneOn;
        }

        if (btnDesactivarMic != null)
        {
            btnDesactivarMic.interactable = hasMicPermission && isMicrophoneOn;
        }

        // Actualizar indicadores visuales
        if (indicadorMicActivo != null)
        {
            indicadorMicActivo.SetActive(isMicrophoneOn);
        }

        if (indicadorMicInactivo != null)
        {
            indicadorMicInactivo.SetActive(!isMicrophoneOn);
        }

        // Para depuraci�n
        Debug.Log($"Estado actual: Permiso={permissionStatus}, Micr�fono={micStatus}");
    }

    public int GetMicStatus()
    {
        return micStatus; // 0=apagado, 1=encendido
    }

    public int GetPermissionStatus()
    {
        return permissionStatus; // 0=sin permiso, 1=con permiso
    }

    // Los m�todos GetAceptaStatus y GetRechazaStatus se han eliminado
    // ya que son redundantes con GetPermissionStatus

    void OnDestroy()
    {
        // Aseg�rate de que el micr�fono se detenga cuando se destruya este objeto
        if (selectedMicrophone != null && Microphone.IsRecording(selectedMicrophone))
        {
            Microphone.End(selectedMicrophone);
        }
    }

    // M�todo opcional para acceder al estado del micr�fono desde otros scripts
    public bool IsMicrophoneActive()
    {
        return isMicrophoneOn && hasMicPermission;
    }

    // M�todo opcional para obtener el AudioClip del micr�fono
    public AudioClip GetMicrophoneClip()
    {
        return microphoneClip;
    }
}