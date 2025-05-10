using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_ANDROID || UNITY_EDITOR
// Importamos esta biblioteca para reconocimiento de voz nativo en Unity
using UnityEngine.Windows.Speech;
#endif

// Evento personalizado para el reconocimiento de voz
[System.Serializable]
public class WordRecognizedEvent : UnityEvent<string> { }

public class SpeechController : MonoBehaviour
{
    // Referencia al controlador de voz y al animator
    [SerializeField] private VoiceController voiceController;
    [SerializeField] private Animator characterAnimator;

    // Lista de palabras clave a reconocer
    [SerializeField] private string[] keywords = new string[] { "sentado", "pata" };

    // Evento que se dispara cuando se reconoce una palabra
    public WordRecognizedEvent onWordRecognized = new WordRecognizedEvent();

    // Palabra reconocida actual (para depuración)
    [SerializeField] private string currentRecognizedWord = "";

    // Estado de reconocimiento
    [SerializeField] private bool isListening = false;

#if UNITY_ANDROID || UNITY_EDITOR
    // KeywordRecognizer para el reconocimiento de voz
    private KeywordRecognizer keywordRecognizer;
    private ConfidenceLevel confidenceLevel = ConfidenceLevel.Medium;
#endif

    // Parámetros del animator
    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");

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

        // Inicializar el reconocedor de voz
        InitializeVoiceRecognition();
    }

    void InitializeVoiceRecognition()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        // Verificar que el micrófono tenga permiso
        if (voiceController != null && voiceController.GetPermissionStatus() == 0)
        {
            Debug.LogWarning("El micrófono no tiene permisos. Solicitando permisos...");
            voiceController.RequestMicrophonePermission();
            return;
        }
        
        // Crear el reconocedor de palabras clave con las palabras que queremos detectar
        if (keywords != null && keywords.Length > 0)
        {
            keywordRecognizer = new KeywordRecognizer(keywords, confidenceLevel);
            keywordRecognizer.OnPhraseRecognized += OnKeywordRecognized;
            
            Debug.Log("Reconocedor de voz inicializado con " + keywords.Length + " palabras clave.");
        }
        else
        {
            Debug.LogError("No se especificaron palabras clave para el reconocimiento.");
        }
#else
        Debug.LogWarning("El reconocimiento de voz solo está disponible en Android o en el Editor.");
#endif
    }

    // Iniciar el reconocimiento de voz
    public void StartListening()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        if (keywordRecognizer == null)
        {
            InitializeVoiceRecognition();
        }
        
        // Verificar que el micrófono esté activo
        if (voiceController != null && voiceController.GetMicStatus() == 0)
        {
            Debug.Log("Activando micrófono antes de iniciar reconocimiento...");
            voiceController.ActivarMicrofono();
        }
        
        // Iniciar el reconocedor si no está activo
        if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Start();
            isListening = true;
            Debug.Log("Reconocimiento de voz iniciado.");
        }
#endif
    }

    // Detener el reconocimiento de voz
    public void StopListening()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.Stop();
            isListening = false;
            Debug.Log("Reconocimiento de voz detenido.");
        }
#endif
    }

#if UNITY_ANDROID || UNITY_EDITOR
    // Método llamado cuando se reconoce una palabra clave
    private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        currentRecognizedWord = args.text;
        Debug.Log("Palabra reconocida: " + currentRecognizedWord);
        
        // Ejecutar el evento de palabra reconocida
        onWordRecognized.Invoke(currentRecognizedWord);
        
        // Activar la animación correspondiente
        PlayAnimationForWord(currentRecognizedWord);
    }
#endif

    // Reproducir la animación correspondiente a la palabra reconocida
    private void PlayAnimationForWord(string word)
    {
        if (characterAnimator == null)
        {
            Debug.LogError("No hay Animator asignado.");
            return;
        }

        switch (word.ToLower())
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
                Debug.LogWarning("Palabra no reconocida para animación: " + word);
                break;
        }
    }

    // Activar o desactivar el reconocimiento de voz
    public void ToggleVoiceRecognition()
    {
        if (isListening)
        {
            StopListening();
        }
        else
        {
            StartListening();
        }
    }

    // Detener todo al destruir el objeto
    void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        if (keywordRecognizer != null)
        {
            if (keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
            keywordRecognizer.OnPhraseRecognized -= OnKeywordRecognized;
            keywordRecognizer.Dispose();
        }
#endif
    }

    // Para pruebas desde el inspector
    [ContextMenu("Test Sentado Animation")]
    void TestSentadoAnimation()
    {
        PlayAnimationForWord("sentado");
    }

    [ContextMenu("Test Pata Animation")]
    void TestPataAnimation()
    {
        PlayAnimationForWord("pata");
    }
}