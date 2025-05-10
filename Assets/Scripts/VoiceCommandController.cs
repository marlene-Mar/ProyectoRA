using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VoiceCommandController : MonoBehaviour
{
    // Referencias
    [SerializeField] private VoiceController voiceController;
    [SerializeField] private Animator characterAnimator;

    // Configuraci�n de comandos de voz
    [SerializeField] private string[] voiceCommands = new string[] { "sentado", "pata" };

    // Par�metros del animator (triggers)
    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");

    // Para depuraci�n
    [SerializeField] private bool isListening = false;
    [SerializeField] private float detectionInterval = 1.0f;
    [SerializeField] private float lastVolume = 0f;
    [SerializeField] private float volumeThreshold = 0.1f;

    // Uso de micr�fono
    private string deviceName = null;
    private AudioClip microphoneClip;
    private float[] sampleData = new float[128];

    void Start()
    {
        // Obtener referencias si no est�n asignadas
        if (voiceController == null)
        {
            voiceController = FindObjectOfType<VoiceController>();
        }

        if (characterAnimator == null)
        {
            characterAnimator = GetComponent<Animator>();
        }

        // Iniciar el proceso cuando el micr�fono tenga permisos
        StartCoroutine(CheckMicrophonePermission());
    }

    IEnumerator CheckMicrophonePermission()
    {
        // Esperar hasta que el micr�fono tenga permisos
        while (voiceController == null || voiceController.GetPermissionStatus() == 0)
        {
            Debug.Log("Esperando permisos de micr�fono...");
            yield return new WaitForSeconds(1.0f);
        }

        // Una vez que tenemos permisos, podemos iniciar
        Debug.Log("Permisos de micr�fono concedidos, iniciando detecci�n...");

        // Inicializar el micr�fono para la detecci�n
        InitializeMicrophone();
    }

    void InitializeMicrophone()
    {
        // Obtener el nombre del dispositivo de micr�fono
        if (Microphone.devices.Length > 0)
        {
            deviceName = Microphone.devices[0];
            Debug.Log("Usando micr�fono: " + deviceName);

            // En un entorno real, aqu� podr�amos iniciar un sistema de reconocimiento de voz
            // Por ahora, simplemente iniciaremos una corrutina para simular detecci�n
            StartCoroutine(SimulateVoiceDetection());
        }
        else
        {
            Debug.LogError("No se detect� ning�n micr�fono en el dispositivo.");
        }
    }

    IEnumerator SimulateVoiceDetection()
    {
        isListening = true;

        while (isListening)
        {
            // Verificar que el micr�fono est� activo
            if (voiceController.GetMicStatus() == 1)
            {
                // Obtener el AudioClip actual del micr�fono
                AudioClip currentClip = voiceController.GetMicrophoneClip();

                if (currentClip != null)
                {
                    // Analizar el volumen para detectar cuando el usuario habla
                    AnalyzeVolume(currentClip);

                    // Si detectamos un volumen por encima del umbral, podr�a ser un comando
                    if (lastVolume > volumeThreshold)
                    {
                        // En un sistema real, aqu� procesar�amos el audio para reconocimiento
                        // Por ahora, simularemos reconocimiento aleatorio para demostraci�n
                        SimulateRandomVoiceCommand();
                    }
                }
            }
            else
            {
                // Si el micr�fono no est� activo, activarlo
                Debug.Log("Micr�fono inactivo. Activando...");
                voiceController.ActivarMicrofono();
            }

            yield return new WaitForSeconds(detectionInterval);
        }
    }

    // Analiza el volumen del audio para detectar cuando el usuario habla
    void AnalyzeVolume(AudioClip clip)
    {
        if (clip == null) return;

        // Obtener datos de muestra del clip
        clip.GetData(sampleData, clip.channels * Microphone.GetPosition(deviceName) - sampleData.Length);

        // Calcular el volumen (RMS)
        float sum = 0f;
        for (int i = 0; i < sampleData.Length; i++)
        {
            sum += sampleData[i] * sampleData[i];
        }

        lastVolume = Mathf.Sqrt(sum / sampleData.Length);

        // Debugging
        Debug.Log("Volumen actual: " + lastVolume);
    }

    // Simula un reconocimiento de voz aleatorio (para demostraci�n)
    void SimulateRandomVoiceCommand()
    {
        if (voiceCommands.Length == 0) return;

        // Seleccionar un comando aleatorio de la lista
        string randomCommand = voiceCommands[Random.Range(0, voiceCommands.Length)];

        Debug.Log("Comando detectado: " + randomCommand);

        // Reproducir la animaci�n correspondiente
        ExecuteVoiceCommand(randomCommand);
    }

    // Ejecuta un comando de voz espec�fico
    public void ExecuteVoiceCommand(string command)
    {
        switch (command.ToLower())
        {
            case "sentado":
                Debug.Log("Reproduciendo animaci�n: Sentado");
                characterAnimator.SetTrigger(triggerSentado);
                break;

            case "pata":
                Debug.Log("Reproduciendo animaci�n: Pata");
                characterAnimator.SetTrigger(triggerPata);
                break;

            default:
                Debug.LogWarning("Comando no reconocido: " + command);
                break;
        }
    }

    // Inicia la detecci�n de voz
    public void StartVoiceDetection()
    {
        if (!isListening)
        {
            isListening = true;
            StartCoroutine(SimulateVoiceDetection());
            Debug.Log("Detecci�n de voz iniciada.");
        }
    }

    // Detiene la detecci�n de voz
    public void StopVoiceDetection()
    {
        isListening = false;
        Debug.Log("Detecci�n de voz detenida.");
    }

    // Alternar detecci�n de voz
    public void ToggleVoiceDetection()
    {
        if (isListening)
        {
            StopVoiceDetection();
        }
        else
        {
            StartVoiceDetection();
        }
    }

    // Para pruebas desde el inspector
    [ContextMenu("Test Sentado Command")]
    void TestSentadoCommand()
    {
        ExecuteVoiceCommand("sentado");
    }

    [ContextMenu("Test Pata Command")]
    void TestPataCommand()
    {
        ExecuteVoiceCommand("pata");
    }
}