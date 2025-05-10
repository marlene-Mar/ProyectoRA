using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VoiceCommandController : MonoBehaviour
{
    // Referencias
    [SerializeField] private VoiceController voiceController;
    [SerializeField] private Animator characterAnimator;

    // Configuración de comandos de voz
    [SerializeField] private string[] voiceCommands = new string[] { "sentado", "pata" };

    // Parámetros del animator (triggers)
    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");

    // Para depuración
    [SerializeField] private bool isListening = false;
    [SerializeField] private float detectionInterval = 1.0f;
    [SerializeField] private float lastVolume = 0f;
    [SerializeField] private float volumeThreshold = 0.1f;

    // Uso de micrófono
    private string deviceName = null;
    private AudioClip microphoneClip;
    private float[] sampleData = new float[128];

    void Start()
    {
        // Obtener referencias si no están asignadas
        if (voiceController == null)
        {
            voiceController = FindObjectOfType<VoiceController>();
        }

        if (characterAnimator == null)
        {
            characterAnimator = GetComponent<Animator>();
        }

        // Iniciar el proceso cuando el micrófono tenga permisos
        StartCoroutine(CheckMicrophonePermission());
    }

    IEnumerator CheckMicrophonePermission()
    {
        // Esperar hasta que el micrófono tenga permisos
        while (voiceController == null || voiceController.GetPermissionStatus() == 0)
        {
            Debug.Log("Esperando permisos de micrófono...");
            yield return new WaitForSeconds(1.0f);
        }

        // Una vez que tenemos permisos, podemos iniciar
        Debug.Log("Permisos de micrófono concedidos, iniciando detección...");

        // Inicializar el micrófono para la detección
        InitializeMicrophone();
    }

    void InitializeMicrophone()
    {
        // Obtener el nombre del dispositivo de micrófono
        if (Microphone.devices.Length > 0)
        {
            deviceName = Microphone.devices[0];
            Debug.Log("Usando micrófono: " + deviceName);

            // En un entorno real, aquí podríamos iniciar un sistema de reconocimiento de voz
            // Por ahora, simplemente iniciaremos una corrutina para simular detección
            StartCoroutine(SimulateVoiceDetection());
        }
        else
        {
            Debug.LogError("No se detectó ningún micrófono en el dispositivo.");
        }
    }

    IEnumerator SimulateVoiceDetection()
    {
        isListening = true;

        while (isListening)
        {
            // Verificar que el micrófono esté activo
            if (voiceController.GetMicStatus() == 1)
            {
                // Obtener el AudioClip actual del micrófono
                AudioClip currentClip = voiceController.GetMicrophoneClip();

                if (currentClip != null)
                {
                    // Analizar el volumen para detectar cuando el usuario habla
                    AnalyzeVolume(currentClip);

                    // Si detectamos un volumen por encima del umbral, podría ser un comando
                    if (lastVolume > volumeThreshold)
                    {
                        // En un sistema real, aquí procesaríamos el audio para reconocimiento
                        // Por ahora, simularemos reconocimiento aleatorio para demostración
                        SimulateRandomVoiceCommand();
                    }
                }
            }
            else
            {
                // Si el micrófono no está activo, activarlo
                Debug.Log("Micrófono inactivo. Activando...");
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

    // Simula un reconocimiento de voz aleatorio (para demostración)
    void SimulateRandomVoiceCommand()
    {
        if (voiceCommands.Length == 0) return;

        // Seleccionar un comando aleatorio de la lista
        string randomCommand = voiceCommands[Random.Range(0, voiceCommands.Length)];

        Debug.Log("Comando detectado: " + randomCommand);

        // Reproducir la animación correspondiente
        ExecuteVoiceCommand(randomCommand);
    }

    // Ejecuta un comando de voz específico
    public void ExecuteVoiceCommand(string command)
    {
        switch (command.ToLower())
        {
            case "sentado":
                Debug.Log("Reproduciendo animación: Sentado");
                characterAnimator.SetTrigger(triggerSentado);
                break;

            case "pata":
                Debug.Log("Reproduciendo animación: Pata");
                characterAnimator.SetTrigger(triggerPata);
                break;

            default:
                Debug.LogWarning("Comando no reconocido: " + command);
                break;
        }
    }

    // Inicia la detección de voz
    public void StartVoiceDetection()
    {
        if (!isListening)
        {
            isListening = true;
            StartCoroutine(SimulateVoiceDetection());
            Debug.Log("Detección de voz iniciada.");
        }
    }

    // Detiene la detección de voz
    public void StopVoiceDetection()
    {
        isListening = false;
        Debug.Log("Detección de voz detenida.");
    }

    // Alternar detección de voz
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