using UnityEngine;
using System.Collections;
using TMPro;

public class VoiceCommandController : MonoBehaviour
{
    [SerializeField] private RecordingCanvas recordingCanvas;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI levelText;

    // Definición de los triggers para las animaciones
    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");

    // Palabras clave para activar las animaciones (en minúsculas)
    [Header("Comandos de voz")]
    [SerializeField] private string[] comandosSentado = { "sentado", "siéntate" };
    [SerializeField] private string[] comandosPata = { "pata", "dame la pata", "la pata" };

    // Tiempo que se mostrará el texto de retroalimentación
    [SerializeField] private float tiempoMostrarFeedback = 2f;
    [SerializeField] private float tiempoMostrarInstruccion = 3f;

    // Objetos 3D para feedback visual
    [Header("Objetos de feedback visual")]
    [SerializeField] private GameObject modeloInterrogacion;
    [SerializeField] private GameObject modeloAdmiracion;
    [SerializeField] private GameObject modeloCorazon;
    [SerializeField] private GameObject botonGalleta;

    // Para el sistema de nivel de entrenamiento
    private int nivelActual = 1;
    private int pasoActual = 0;
    private bool esperandoComando = false;
    private bool esperandoGalleta = false;
    private bool comandoComprendido = false;

    // Para evitar la activación múltiple de animaciones
    private bool esperandoFinAnimacion = false;
    private Coroutine feedbackCoroutine;
    private Coroutine instructionCoroutine;

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

        // Suscribirse al evento de resultado final del reconocimiento de voz
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

        // Inicializar los textos
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        if (instructionText != null)
        {
            instructionText.text = "";
        }

        if (levelText != null)
        {
            levelText.text = "Nivel: " + nivelActual;
        }

        // Inicializar objetos 3D
        if (modeloInterrogacion != null) modeloInterrogacion.SetActive(false);
        if (modeloAdmiracion != null) modeloAdmiracion.SetActive(false);
        if (modeloCorazon != null) modeloCorazon.SetActive(false);
        if (botonGalleta != null) botonGalleta.SetActive(false);

        // Iniciar el sistema de entrenamiento
        StartCoroutine(IniciarEntrenamiento());
    }

    private IEnumerator IniciarEntrenamiento()
    {
        // Esperar un momento para que todo se inicialice correctamente
        yield return new WaitForSeconds(1f);

        // Iniciar el primer nivel
        IniciarNivel1();
    }

    private void IniciarNivel1()
    {
        nivelActual = 1;
        pasoActual = 0;

        if (levelText != null)
        {
            levelText.text = "Nivel: " + nivelActual;
        }

        // Avanzar al primer paso del nivel 1
        AvanzarPasoNivel1();
    }

    private void AvanzarPasoNivel1()
    {
        pasoActual++;

        switch (pasoActual)
        {
            case 1:
                // Mostrar instrucción para decir el comando
                MostrarInstruccion("Di el comando: \"Siéntate\" o \"Sentado\"");
                esperandoComando = true;
                comandoComprendido = false;
                break;

            case 2:
                // Mostrar instrucción para mostrar galleta
                MostrarInstruccion("Muestra galleta, dándole clic");
                if (botonGalleta != null) botonGalleta.SetActive(true);
                esperandoGalleta = true;
                break;

            case 3:
                // Volver a pedir que diga el comando
                MostrarInstruccion("Di nuevamente el comando: \"Siéntate\" o \"Sentado\"");
                esperandoComando = true;
                comandoComprendido = true;
                break;

            case 4:
                // Pedir que dé la galleta
                MostrarInstruccion("Da la galleta, dando clic");
                if (botonGalleta != null) botonGalleta.SetActive(true);
                esperandoGalleta = true;
                break;

            case 5:
                // Mostrar mensaje de avance de nivel
                MostrarInstruccion("¡Lograste aprender este comando!");
                StartCoroutine(MostrarYOcultarModelo(modeloCorazon, 3f));

                // Esperar un momento y luego avanzar
                StartCoroutine(AvanzarDeNivel(5f));
                break;
        }
    }

    private IEnumerator AvanzarDeNivel(float tiempoEspera)
    {
        yield return new WaitForSeconds(tiempoEspera);

        // Mostrar mensaje de avance
        MostrarInstruccion("Avanzando de nivel...");

        // Actualizar texto de nivel
        nivelActual++;
        if (levelText != null)
        {
            levelText.text = "Nivel: " + nivelActual;
        }

        // Restablecer el perro a su animación inicial
        yield return new WaitForSeconds(5f);

        // Aquí iría la lógica para el siguiente nivel...
        // Por ahora volvemos a iniciar el nivel 1 para pruebas
        IniciarNivel1();
    }

    private IEnumerator MostrarYOcultarModelo(GameObject modelo, float duracion)
    {
        if (modelo != null)
        {
            modelo.SetActive(true);
            yield return new WaitForSeconds(duracion);
            modelo.SetActive(false);
        }
    }

    public void ProcesarComandoVoz(string textoReconocido)
    {
        // Convertir a minúsculas para facilitar la comparación
        string textoLower = textoReconocido.ToLower().Trim();

        Debug.Log("Texto reconocido: " + textoLower);

        // Verificar si estamos esperando un comando de voz para el entrenamiento
        if (esperandoComando)
        {
            // Verificar si el comando corresponde a "Sentado"
            if (ContienePalabra(textoLower, comandosSentado))
            {
                esperandoComando = false;

                if (comandoComprendido)
                {
                    // El perro ya comprende el comando
                    EjecutarComando("sentado");
                    AvanzarPasoNivel1();
                }
                else
                {
                    // El perro no comprende aún el comando
                    StartCoroutine(MostrarYOcultarModelo(modeloInterrogacion, 2f));
                    MostrarFeedback("¿?");
                    AvanzarPasoNivel1();
                }
            }
        }
        // La lógica original para cuando no estamos en modo entrenamiento
        else if (!esperandoFinAnimacion)
        {
            // Verificar si el comando corresponde a "Sentado"
            if (ContienePalabra(textoLower, comandosSentado))
            {
                EjecutarComando("sentado");
            }
            // Verificar si el comando corresponde a "Pata"
            else if (ContienePalabra(textoLower, comandosPata))
            {
                EjecutarComando("pata");
            }
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
        // Si ya está esperando el fin de una animación, no hacer nada
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

    // Para evitar que se activen varias animaciones simultáneamente
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
            // Cancelar cualquier corrutina anterior
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

    private void MostrarInstruccion(string mensaje)
    {
        if (instructionText != null)
        {
            // Cancelar cualquier corrutina anterior
            if (instructionCoroutine != null)
            {
                StopCoroutine(instructionCoroutine);
            }

            StartCoroutine(MostrarTextoConEfectoEscritura(mensaje));
        }
    }

    private IEnumerator MostrarTextoConEfectoEscritura(string textoCompleto)
    {
        if (instructionText != null)
        {
            instructionText.text = "";

            // Mostrar letra por letra
            for (int i = 0; i < textoCompleto.Length; i++)
            {
                instructionText.text += textoCompleto[i];
                yield return new WaitForSeconds(0.03f); // Velocidad de escritura
            }

            // Mantener el texto visible por un tiempo
            instructionCoroutine = StartCoroutine(MantenerInstruccionVisible());
        }
    }

    private IEnumerator MantenerInstruccionVisible()
    {
        yield return new WaitForSeconds(tiempoMostrarInstruccion);
        // No borramos el texto automáticamente para que el usuario tenga tiempo de leer
        instructionCoroutine = null;
    }

    // Método para el botón galleta
    public void ClickGalleta()
    {
        if (esperandoGalleta)
        {
            esperandoGalleta = false;
            botonGalleta.SetActive(false);

            if (pasoActual == 2)
            {
                // Primera vez con la galleta
                StartCoroutine(MostrarYOcultarModelo(modeloAdmiracion, 2f));
                MostrarFeedback("¡!");
                AvanzarPasoNivel1();
            }
            else if (pasoActual == 4)
            {
                // Segunda vez con la galleta (recompensa)
                AvanzarPasoNivel1();
            }
        }
    }
}