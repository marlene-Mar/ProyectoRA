using UnityEngine;
using System.Collections;
using TMPro;

public class VoiceCommandController : MonoBehaviour
{
    [SerializeField] private RecordingCanvas recordingCanvas;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private TextMeshProUGUI feedbackText;

    //Definición de los triggers para las animaciones
    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");

    //Palabras clave para activar las animaciones(en minúsculas)
    [Header("Comandos de voz")]
    [SerializeField] private string[] comandosSentado = { "sentado", "siéntate" };
    [SerializeField] private string[] comandosPata = { "pata", "dame la pata", "la pata" };

    //Tiempo que se mostrará el texto de retroalimentación
   [SerializeField] private float tiempoMostrarFeedback = 2f;

    //Para evitar la activación múltiple de animaciones
    private bool esperandoFinAnimacion = false;
    private Coroutine feedbackCoroutine;

    void Start()
    {
        if (recordingCanvas == null)
        {
            recordingCanvas = FindObjectOfType<RecordingCanvas>();
            if (recordingCanvas == null)
            {
                Debug.LogError("No se encontró un RecordingCanvas en la escena.");
                enabled = false;
                return;
            }
        }

        //Suscribirse al evento de resultado final del reconocimiento de voz
        KKSpeech.SpeechRecognizerListener listener = FindObjectOfType<KKSpeech.SpeechRecognizerListener>();
        if (listener != null)
        {
            listener.onFinalResults.AddListener(ProcesarComandoVoz);
        }
        else
        {
            Debug.LogError("No se encontró un SpeechRecognizerListener en la escena.");
            enabled = false;
        }

        //Inicializar el texto de retroalimentación
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    public void ProcesarComandoVoz(string textoReconocido)
    {
        //Convertir a minúsculas para facilitar la comparación
        string textoLower = textoReconocido.ToLower().Trim();

        Debug.Log("Texto reconocido: " + textoLower);

        //Verificar si el comando corresponde a "Sentado"
        if (ContienePalabra(textoLower, comandosSentado) && !esperandoFinAnimacion)
        {
            EjecutarComando("sentado");
        }
        //Verificar si el comando corresponde a "Pata"
        else if (ContienePalabra(textoLower, comandosPata) && !esperandoFinAnimacion)
        {
            EjecutarComando("pata");
        }
    }

    private bool ContienePalabra(string texto, string[] palabrasClave)
    {
        foreach (string palabra in palabrasClave)
        {
            if (texto.Contains(palabra))
            {
                return true;
            }
        }
        return false;
    }

    public void EjecutarComando(string comando)
    {
        //Si ya está esperando el fin de una animación, no hacer nada
        if (esperandoFinAnimacion)
            return;

        Debug.Log("Ejecutando comando: " + comando);

        switch (comando.ToLower())
        {
            case "sentado":
                if (characterAnimator != null)
                {
                    characterAnimator.SetTrigger(triggerSentado);
                    MostrarFeedback("¡Sentado!");
                    StartCoroutine(EsperarFinAnimacion(1.5f)); // Ajusta este tiempo según la duración de tu animación
                }
                break;

            case "pata":
                if (characterAnimator != null)
                {
                    characterAnimator.SetTrigger(triggerPata);
                    MostrarFeedback("¡Dame la pata!");
                    StartCoroutine(EsperarFinAnimacion(1.5f)); // Ajusta este tiempo según la duración de tu animación
                }
                break;

            default:
                Debug.LogWarning("Comando desconocido: " + comando);
                break;
        }
    }

    //Para evitar que se activen varias animaciones simultáneamente
    private IEnumerator EsperarFinAnimacion(float tiempoEspera)
    {
        esperandoFinAnimacion = true;
        yield return new WaitForSeconds(tiempoEspera);
        esperandoFinAnimacion = false;
    }

    private void MostrarFeedback(string mensaje)
    {
        if (feedbackText != null)
        {
            //Cancelar cualquier corrutina anterior
            if (feedbackCoroutine != null)
            {
                StopCoroutine(feedbackCoroutine);
            }

            feedbackText.text = mensaje;
            feedbackCoroutine = StartCoroutine(OcultarFeedbackDespuesDeTiempo());
        }
    }

    private IEnumerator OcultarFeedbackDespuesDeTiempo()
    {
        yield return new WaitForSeconds(tiempoMostrarFeedback);
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
        feedbackCoroutine = null;
    }

    //Para pruebas desde botones en la interfaz
    public void ProbarComandoSentado()
    {
        EjecutarComando("sentado");
    }

    public void ProbarComandoPata()
    {
        EjecutarComando("pata");
    }
}
