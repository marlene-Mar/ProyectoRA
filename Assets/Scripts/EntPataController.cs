using UnityEngine;
using System.Collections;
using TMPro;

public class EntPataController : MonoBehaviour
{
    [SerializeField] private RecordingCanvas recordingCanvas;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI levelText;

    // Definición de los triggers y parámetros para las animaciones
    private readonly int triggerSentado = Animator.StringToHash("Sentado");
    private readonly int triggerPata = Animator.StringToHash("Pata");
    private readonly int paramReset = Animator.StringToHash("Reset"); // Parámetro booleano para volver a pose inicial

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
    [SerializeField] private GameObject botonAcariciar;
    [SerializeField] private GameObject botonPelota;

    // Para el sistema de nivel de entrenamiento
    private int nivelActual = 1;
    private int pasoActual = 0;
    private bool esperandoComando = false;
    private bool esperandoComandoSentado = false;
    private bool esperandoGalleta = false;
    private bool esperandoAcariciar = false;
    private bool esperandoPelota = false;
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
        if (botonAcariciar != null) botonAcariciar.SetActive(false);
        if (botonPelota != null) botonPelota.SetActive(false);

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

        // Reiniciar la animación antes de comenzar el nivel
        ReiniciarAnimacion();

        // Avanzar al primer paso del nivel 1
        AvanzarPasoNivel1();
    }

    private void AvanzarPasoNivel1()
    {
        pasoActual++;

        switch (pasoActual)
        {
            case 1:
                // Primero pedimos que el perro se siente
                MostrarInstruccion("Primero di: \"Siéntate\" o \"Sentado\"");
                esperandoComandoSentado = true;
                break;

            case 2:
                // Mostrar instrucción para decir el comando
                MostrarInstruccion("Ahora di: \"Pata\" o \"Dame la pata\"");
                esperandoComando = true;
                comandoComprendido = false;
                break;

            case 3:
                // Mostrar instrucción para mostrar galleta
                MostrarInstruccion("Muestra galleta, dándole clic");
                if (botonGalleta != null) botonGalleta.SetActive(true);
                esperandoGalleta = true;
                break;

            case 4:
                // Pedir que se siente de nuevo
                MostrarInstruccion("Di nuevamente: \"Siéntate\" o \"Sentado\"");
                esperandoComandoSentado = true;
                break;

            case 5:
                // Volver a pedir que diga el comando
                MostrarInstruccion("Di otra vez: \"Pata\" o \"Dame la pata\"");
                esperandoComando = true;
                comandoComprendido = true;
                break;

            case 6:
                // Pedir que dé la galleta
                MostrarInstruccion("Da la galleta, dando clic");
                if (botonGalleta != null) botonGalleta.SetActive(true);
                esperandoGalleta = true;
                break;

            case 7:
                // Mostrar mensaje de avance de nivel
                MostrarInstruccion("¡Lograste enseñar el comando Pata!");
                StartCoroutine(MostrarYOcultarModelo(modeloCorazon, 3f));

                // Esperar un momento y luego avanzar
                StartCoroutine(AvanzarDeNivel(5f));
                break;
        }
    }

    private void IniciarNivel2()
    {
        nivelActual = 2;
        pasoActual = 0;

        if (levelText != null)
        {
            levelText.text = "Nivel: " + nivelActual;
        }

        // Reiniciar la animación antes de comenzar el nivel
        ReiniciarAnimacion();

        // Avanzar al primer paso del nivel 2
        AvanzarPasoNivel2();
    }

    private void AvanzarPasoNivel2()
    {
        pasoActual++;

        switch (pasoActual)
        {
            case 1:
                // Primero pedimos que el perro se siente
                MostrarInstruccion("Primero di: \"Siéntate\" o \"Sentado\"");
                esperandoComandoSentado = true;
                break;

            case 2:
                // Mostrar instrucción para mostrar galleta
                MostrarInstruccion("Muestra galleta, dándole clic");
                if (botonGalleta != null) botonGalleta.SetActive(true);
                esperandoGalleta = true;
                break;

            case 3:
                // Pedir que diga el comando de pata
                MostrarInstruccion("Di: \"Pata\" o \"Dame la pata\"");
                esperandoComando = true;
                break;

            case 4:
                // Instrucción para acariciar al perro
                MostrarInstruccion("Acaricia a tu perrito, dando clic en la mano");
                if (botonAcariciar != null) botonAcariciar.SetActive(true);
                esperandoAcariciar = true;
                break;

            case 5:
                // Mostrar mensaje de refuerzo y avance de nivel
                MostrarInstruccion("Muy bien, tu perrito ya casi no necesita premios para dar la pata.");

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
        ReiniciarAnimacion();

        yield return new WaitForSeconds(5f);

        // Decidir qué nivel iniciar
        if (nivelActual == 2)
        {
            IniciarNivel2();
        }
        else if (nivelActual == 3)
        {
            IniciarNivel3();
        }
        else if (nivelActual > 3)
        {
            // Si ya completamos todos los niveles, reiniciamos el entrenamiento
            MostrarInstruccion("Reiniciando entrenamiento...");
            yield return new WaitForSeconds(3f);
            nivelActual = 0;
            IniciarNivel1();
        }
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

        // Verificar si estamos esperando comando sentado primero (siempre debe ejecutarse antes del comando pata)
        if (esperandoComandoSentado)
        {
            if (ContienePalabra(textoLower, comandosSentado))
            {
                esperandoComandoSentado = false;
                // Ejecutar comando sentado (en todos los niveles se comprende)
                EjecutarComando("sentado");

                // Avanzar al siguiente paso según el nivel
                if (nivelActual == 1)
                {
                    AvanzarPasoNivel1();
                }
                else if (nivelActual == 2)
                {
                    AvanzarPasoNivel2();
                }
                else if (nivelActual == 3)
                {
                    AvanzarPasoNivel3();
                }
            }
        }
        // Verificar si estamos esperando comando de pata para el entrenamiento
        else if (esperandoComando)
        {
            // Verificar si el comando corresponde a "Pata"
            if (ContienePalabra(textoLower, comandosPata))
            {
                esperandoComando = false;

                if (nivelActual == 1)
                {
                    if (comandoComprendido)
                    {
                        // El perro ya comprende el comando (Nivel 1)
                        EjecutarComando("pata");
                        AvanzarPasoNivel1();
                    }
                    else
                    {
                        // El perro no comprende aún el comando (Nivel 1)
                        StartCoroutine(MostrarYOcultarModelo(modeloInterrogacion, 2f));
                        MostrarFeedback("¿?");
                        AvanzarPasoNivel1();
                    }
                }
                else if (nivelActual == 2)
                {
                    // En el nivel 2 el perro ya comprende el comando
                    EjecutarComando("pata");
                    AvanzarPasoNivel2();
                }
                else if (nivelActual == 3)
                {
                    // En el nivel 3 el perro comprende perfectamente el comando
                    EjecutarComando("pata");
                    AvanzarPasoNivel3();
                }
            }
        }
        // La lógica para cuando no estamos en modo entrenamiento específico
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
                // Verificamos si el perro está sentado para dar la pata
                if (characterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sentado"))
                {
                    EjecutarComando("pata");
                }
                else
                {
                    // Si el perro no está sentado, mostrar un mensaje de que primero debe sentarse
                    MostrarFeedback("¡Primero siéntate!");
                }
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
                    // Desactivamos el reset si estaba activo
                    characterAnimator.SetBool(paramReset, false);

                    // Reset del trigger de pata si estaba activo
                    characterAnimator.ResetTrigger(triggerPata);

                    // Ejecutamos la animación de sentado
                    characterAnimator.SetTrigger(triggerSentado);
                    MostrarFeedback("¡Sentado!");
                    StartCoroutine(EsperarFinAnimacion(1.5f));
                }
                break;

            case "pata":
                if (characterAnimator != null)
                {
                    // Para dar la pata, el perro primero debe estar sentado
                    // Esto se verifica en ProcesarComandoVoz

                    // Desactivamos el reset si estaba activo
                    characterAnimator.SetBool(paramReset, false);

                    // Ejecutamos la animación de pata
                    characterAnimator.SetTrigger(triggerPata);
                    MostrarFeedback("¡Dame la pata!");
                    StartCoroutine(EsperarFinAnimacion(1.5f));
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

            if (nivelActual == 1)
            {
                if (pasoActual == 3)
                {
                    // Primera vez con la galleta (Nivel 1)
                    StartCoroutine(MostrarYOcultarModelo(modeloAdmiracion, 2f));
                    MostrarFeedback("¡!");
                    AvanzarPasoNivel1();
                }
                else if (pasoActual == 6)
                {
                    // Segunda vez con la galleta (recompensa Nivel 1)
                    AvanzarPasoNivel1();
                }
            }
            else if (nivelActual == 2)
            {
                // Galleta en el Nivel 2
                StartCoroutine(MostrarYOcultarModelo(modeloAdmiracion, 2f));
                MostrarFeedback("¡!");
                AvanzarPasoNivel2();
            }
        }
    }

    // Método para el botón acariciar
    public void ClickAcariciar()
    {
        if (esperandoAcariciar)
        {
            esperandoAcariciar = false;
            botonAcariciar.SetActive(false);

            if (nivelActual == 2 && pasoActual == 4)
            {
                StartCoroutine(MostrarYOcultarModelo(modeloCorazon, 2f));
                MostrarFeedback("♥");
                AvanzarPasoNivel2();
            }
            else if (nivelActual == 3 && pasoActual == 2)
            {
                StartCoroutine(MostrarYOcultarModelo(modeloCorazon, 2f));
                MostrarFeedback("♥");
                AvanzarPasoNivel3();
            }
        }
    }

    // Método para el botón pelota
    public void ClickPelota()
    {
        if (esperandoPelota)
        {
            esperandoPelota = false;
            botonPelota.SetActive(false);

            if (nivelActual == 3 && pasoActual == 3)
            {
                StartCoroutine(MostrarYOcultarModelo(modeloCorazon, 2f));
                MostrarFeedback("♥");
                AvanzarPasoNivel3();
            }
        }
    }

    private void IniciarNivel3()
    {
        nivelActual = 3;
        pasoActual = 0;

        if (levelText != null)
        {
            levelText.text = "Nivel: " + nivelActual;
        }

        // Reiniciar la animación antes de comenzar el nivel
        ReiniciarAnimacion();

        // Avanzar al primer paso del nivel 3
        AvanzarPasoNivel3();
    }

    private void AvanzarPasoNivel3()
    {
        pasoActual++;

        switch (pasoActual)
        {
            case 1:
                // Primero pedimos que el perro se siente
                MostrarInstruccion("Primero di: \"Siéntate\" o \"Sentado\"");
                esperandoComandoSentado = true;
                break;

            case 2:
                // Instrucción para decir el comando de pata directamente
                MostrarInstruccion("Ahora di: \"Pata\" o \"Dame la pata\"");
                esperandoComando = true;
                break;

            case 3:
                // Instrucción para acariciar al perro
                MostrarInstruccion("¡Muy bien! Tu perro ya sabe el comando. Acaricia a tu perrito, dando clic en la mano");
                if (botonAcariciar != null) botonAcariciar.SetActive(true);
                esperandoAcariciar = true;
                break;

            case 4:
                // Instrucción para jugar con el perro
                MostrarInstruccion("Tu perrito merece una mayor recompensa, ¡juega con él! Da clic en la pelota");
                if (botonPelota != null) botonPelota.SetActive(true);
                esperandoPelota = true;
                break;

            case 5:
                // Mostrar mensaje de finalización
                MostrarInstruccion("¡Finalizamos el entrenamiento, suerte con tu peludo!");

                // Esperar y reiniciar el entrenamiento
                StartCoroutine(FinalizarEntrenamiento());
                break;
        }
    }

    private IEnumerator FinalizarEntrenamiento()
    {
        yield return new WaitForSeconds(5f);
        MostrarInstruccion("Reiniciando entrenamiento...");

        yield return new WaitForSeconds(3f);

        // Reiniciar todo el proceso
        nivelActual = 0;
        IniciarNivel1();
    }

    // Método para reiniciar a la animación inicial
    private void ReiniciarAnimacion()
    {
        if (characterAnimator != null)
        {
            // Reiniciar todos los parámetros
            characterAnimator.Rebind();
            characterAnimator.Update(0f);

            // Establecer explícitamente el parámetro Reset
            characterAnimator.SetBool(paramReset, true);

            Debug.Log("Reiniciando animación usando Rebind y Reset=true");

            // Asegurarse de que se reproduzca la animación por defecto
            StartCoroutine(DesactivarResetDespuesDeTiempo(0.5f));
        }
    }

    // Método para desactivar el parámetro Reset después de un tiempo
    private IEnumerator DesactivarResetDespuesDeTiempo(float tiempoEspera)
    {
        yield return new WaitForSeconds(tiempoEspera);

        // Desactivar el parámetro Reset una vez que la animación ya ha comenzado
        if (characterAnimator != null)
        {
            characterAnimator.SetBool(paramReset, false);
            Debug.Log("Parámetro Reset desactivado después de " + tiempoEspera + " segundos");
        }
    }
}