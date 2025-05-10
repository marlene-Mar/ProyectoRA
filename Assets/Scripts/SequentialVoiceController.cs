using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SequentialVoiceController : MonoBehaviour
{
    // Referencias
    [SerializeField] private VoiceController voiceController;
    [SerializeField] private Animator characterAnimator;

    // Configuraci�n de comandos de voz
    [SerializeField] private string[] voiceCommands = new string[] { "sentado", "pata" };

    // Par�metros del animator (triggers)
    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");

    // Estado actual de animaci�n
    [SerializeField] private string currentAnimationState = "Inicial";

    // Para depuraci�n
    [SerializeField] private bool isListening = false;
    [SerializeField] private float detectionInterval = 1.0f;
    [SerializeField] private string lastRecognizedCommand = "";

    // Uso de micr�fono
    private AudioClip microphoneClip;

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
            if (characterAnimator == null)
            {
                characterAnimator = GetComponentInChildren<Animator>();
                if (characterAnimator == null)
                {
                    Debug.LogError("No se encontr� un componente Animator.");
                }
            }
        }

        // Registrarse para eventos de animaci�n para rastrear el estado actual
        RegisterAnimationCallbacks();

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

            // Solicitar permisos si no est�n concedidos
            if (voiceController != null && voiceController.GetPermissionStatus() == 0)
            {
                voiceController.RequestMicrophonePermission();
            }
        }

        // Una vez que tenemos permisos, podemos iniciar
        Debug.Log("Permisos de micr�fono concedidos, iniciando detecci�n...");

        // Iniciar la detecci�n de voz simulada
        StartVoiceDetection();
    }

    // Registrar callbacks para rastrear cambios de estado de animaci�n
    void RegisterAnimationCallbacks()
    {
        // Esta es una forma de rastrear el estado actual usando AnimatorStateInfo
        // En un proyecto real, podr�as implementar AnimationBehaviours para una detecci�n m�s precisa
    }

    // Obtener el estado actual de animaci�n
    string GetCurrentAnimatorState()
    {
        if (characterAnimator == null) return "Unknown";

        // Obtener informaci�n del estado actual (capa 0 - Base Layer)
        AnimatorStateInfo stateInfo = characterAnimator.GetCurrentAnimatorStateInfo(0);

        // Comprobar qu� estado est� activo
        if (stateInfo.IsName("Inicial")) return "Inicial";
        if (stateInfo.IsName("Sentado")) return "Sentado";
        if (stateInfo.IsName("Pata")) return "Pata";

        return "Unknown";
    }

    // Inicia la detecci�n de voz simulada
    public void StartVoiceDetection()
    {
        if (!isListening)
        {
            isListening = true;
            StartCoroutine(SimulateVoiceDetection());
            Debug.Log("Detecci�n de voz iniciada.");
        }
    }

    IEnumerator SimulateVoiceDetection()
    {
        while (isListening)
        {
            // Verificar que el micr�fono est� activo
            if (voiceController.GetMicStatus() == 0)
            {
                Debug.Log("Micr�fono inactivo. Activando...");
                voiceController.ActivarMicrofono();
                yield return new WaitForSeconds(0.5f);
            }

            // Aqu� ir�a la l�gica real de detecci�n de voz
            // Para fines de demostraci�n, puedes presionar teclas para simular comandos

            if (Input.GetKeyDown(KeyCode.S))
            {
                // Simular detecci�n de "sentado"
                ProcessVoiceCommand("sentado");
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                // Simular detecci�n de "pata"
                ProcessVoiceCommand("pata");
            }

            // Actualizar el estado actual (para depuraci�n)
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

    // Reproduce la animaci�n "Sentado"
    public void PlaySentadoAnimation()
    {
        Debug.Log("Reproduciendo animaci�n: Sentado");
        characterAnimator.SetTrigger(triggerSentado);
    }

    // Reproduce la animaci�n "Pata", considerando la secuencia
    public void PlayPataAnimation()
    {
        // Actualizar el estado actual para saber en qu� estado estamos
        currentAnimationState = GetCurrentAnimatorState();

        // Si no estamos en "Sentado", primero activamos esa animaci�n
        if (currentAnimationState != "Sentado")
        {
            Debug.Log("El perro debe estar sentado primero. Sentando...");
            StartCoroutine(PlayPataAfterSentado());
        }
        else
        {
            // Ya estamos sentados, podemos dar la pata directamente
            Debug.Log("Reproduciendo animaci�n: Pata");
            characterAnimator.SetTrigger(triggerPata);
        }
    }

    // Corrutina para esperar a que el perro se siente antes de dar la pata
    IEnumerator PlayPataAfterSentado()
    {
        // Activar la animaci�n de sentado
        characterAnimator.SetTrigger(triggerSentado);

        // Esperar a que la transici�n se complete (tiempo aproximado)
        yield return new WaitForSeconds(1.0f);

        // Ahora el perro deber�a estar sentado, activar la pata
        Debug.Log("Reproduciendo animaci�n: Pata");
        characterAnimator.SetTrigger(triggerPata);
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
        GUILayout.Label("�ltimo comando: " + lastRecognizedCommand);

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