using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SequentialVoiceController : MonoBehaviour
{
    // Referencias
    [SerializeField] private VoiceController voiceController;
    [SerializeField] private Animator characterAnimator;

    // Configuración de comandos de voz
    [SerializeField] private string[] voiceCommands = new string[] { "sentado", "pata" };

    // Parámetros del animator (triggers)
    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");

    // Estado actual de animación
    [SerializeField] private string currentAnimationState = "Inicial";

    // Para depuración
    [SerializeField] private bool isListening = false;
    [SerializeField] private float detectionInterval = 1.0f;
    [SerializeField] private string lastRecognizedCommand = "";

    // Uso de micrófono
    private AudioClip microphoneClip;

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
            if (characterAnimator == null)
            {
                characterAnimator = GetComponentInChildren<Animator>();
                if (characterAnimator == null)
                {
                    Debug.LogError("No se encontró un componente Animator.");
                }
            }
        }

        // Registrarse para eventos de animación para rastrear el estado actual
        RegisterAnimationCallbacks();

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

            // Solicitar permisos si no están concedidos
            if (voiceController != null && voiceController.GetPermissionStatus() == 0)
            {
                voiceController.RequestMicrophonePermission();
            }
        }

        // Una vez que tenemos permisos, podemos iniciar
        Debug.Log("Permisos de micrófono concedidos, iniciando detección...");

        // Iniciar la detección de voz simulada
        StartVoiceDetection();
    }

    // Registrar callbacks para rastrear cambios de estado de animación
    void RegisterAnimationCallbacks()
    {
        // Esta es una forma de rastrear el estado actual usando AnimatorStateInfo
        // En un proyecto real, podrías implementar AnimationBehaviours para una detección más precisa
    }

    // Obtener el estado actual de animación
    string GetCurrentAnimatorState()
    {
        if (characterAnimator == null) return "Unknown";

        // Obtener información del estado actual (capa 0 - Base Layer)
        AnimatorStateInfo stateInfo = characterAnimator.GetCurrentAnimatorStateInfo(0);

        // Comprobar qué estado está activo
        if (stateInfo.IsName("Inicial")) return "Inicial";
        if (stateInfo.IsName("Sentado")) return "Sentado";
        if (stateInfo.IsName("Pata")) return "Pata";

        return "Unknown";
    }

    // Inicia la detección de voz simulada
    public void StartVoiceDetection()
    {
        if (!isListening)
        {
            isListening = true;
            StartCoroutine(SimulateVoiceDetection());
            Debug.Log("Detección de voz iniciada.");
        }
    }

    IEnumerator SimulateVoiceDetection()
    {
        while (isListening)
        {
            // Verificar que el micrófono esté activo
            if (voiceController.GetMicStatus() == 0)
            {
                Debug.Log("Micrófono inactivo. Activando...");
                voiceController.ActivarMicrofono();
                yield return new WaitForSeconds(0.5f);
            }

            // Aquí iría la lógica real de detección de voz
            // Para fines de demostración, puedes presionar teclas para simular comandos

            if (Input.GetKeyDown(KeyCode.S))
            {
                // Simular detección de "sentado"
                ProcessVoiceCommand("sentado");
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                // Simular detección de "pata"
                ProcessVoiceCommand("pata");
            }

            // Actualizar el estado actual (para depuración)
            currentAnimationState = GetCurrentAnimatorState();

            yield return null; // Esperar al siguiente frame
        }
    }

    // Procesa un comando de voz detectado
    public void ProcessVoiceCommand(string command)
    {
        lastRecognizedCommand = command.ToLower();
        Debug.Log("Comando reconocido: " + lastRecognizedCommand);

        // Ejecutar el comando apropiado
        switch (lastRecognizedCommand)
        {
            case "sentado":
                PlaySentadoAnimation();
                break;

            case "pata":
                PlayPataAnimation();
                break;

            default:
                Debug.LogWarning("Comando no reconocido: " + lastRecognizedCommand);
                break;
        }
    }

    // Reproduce la animación "Sentado"
    public void PlaySentadoAnimation()
    {
        Debug.Log("Reproduciendo animación: Sentado");
        characterAnimator.SetTrigger(triggerSentado);
    }

    // Reproduce la animación "Pata", considerando la secuencia
    public void PlayPataAnimation()
    {
        // Actualizar el estado actual para saber en qué estado estamos
        currentAnimationState = GetCurrentAnimatorState();

        // Si no estamos en "Sentado", primero activamos esa animación
        if (currentAnimationState != "Sentado")
        {
            Debug.Log("El perro debe estar sentado primero. Sentando...");
            StartCoroutine(PlayPataAfterSentado());
        }
        else
        {
            // Ya estamos sentados, podemos dar la pata directamente
            Debug.Log("Reproduciendo animación: Pata");
            characterAnimator.SetTrigger(triggerPata);
        }
    }

    // Corrutina para esperar a que el perro se siente antes de dar la pata
    IEnumerator PlayPataAfterSentado()
    {
        // Activar la animación de sentado
        characterAnimator.SetTrigger(triggerSentado);

        // Esperar a que la transición se complete (tiempo aproximado)
        yield return new WaitForSeconds(1.0f);

        // Ahora el perro debería estar sentado, activar la pata
        Debug.Log("Reproduciendo animación: Pata");
        characterAnimator.SetTrigger(triggerPata);
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

    // Para pruebas desde el Inspector
    [ContextMenu("Test Sentado Command")]
    void TestSentadoCommand()
    {
        ProcessVoiceCommand("sentado");
    }

    [ContextMenu("Test Pata Command")]
    void TestPataCommand()
    {
        ProcessVoiceCommand("pata");
    }

    void OnGUI()
    {
        // Interfaz simple para pruebas
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));

        GUILayout.Label("Estado actual: " + currentAnimationState);
        GUILayout.Label("Último comando: " + lastRecognizedCommand);

        if (GUILayout.Button("Sentar"))
        {
            ProcessVoiceCommand("sentado");
        }

        if (GUILayout.Button("Pata"))
        {
            ProcessVoiceCommand("pata");
        }

        GUILayout.EndArea();
    }
}