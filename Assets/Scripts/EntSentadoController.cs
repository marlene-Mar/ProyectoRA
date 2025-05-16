using UnityEngine;
using System.Collections;
using TMPro;

public class EntrenamientoSentadoController : MonoBehaviour
{
    [SerializeField] private RecordingCanvas recordingCanvas;  // Referencia al canvas de grabación
    [SerializeField] private Animator characterAnimator; // Referencia al Animator del personaje
    [SerializeField] private TextMeshProUGUI feedbackText; // Texto para mostrar feedback al usuario
    [SerializeField] private TextMeshProUGUI instructionText; //Texto para mostrar instrucciones al usuario
    [SerializeField] private TextMeshProUGUI levelText; // Texto para mostrar el nivel actual

    // Definición de los triggers y parámetros para las animaciones
    private readonly int triggerPata = Animator.StringToHash("Sentado"); // Trigger para la animación de "Pata"
    private readonly int paramReset = Animator.StringToHash("Reset"); // Parámetro booleano para volver a animación inicial

    // Palabras clave para activar las animaciones
    [SerializeField] private string[] comandoSentado= { "sentado", "sientate" }; // Comandos de voz para activar la animación de dar la pata

    // Tiempo que se mostrará el texto de retroalimentación
    [SerializeField] private float tiempoMostrarFeedback = 2f;
    [SerializeField] private float tiempoMostrarInstruccion = 3f;

    // Objetos 3D para feedback visual
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
        // Inicializar el canvas de grabación
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

        // Desactivar interacción de botones
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

    // Metodo para avanzar al siguiente paso del nivel 1
    private void AvanzarPasoNivel1()
    {
        pasoActual++;

        switch (pasoActual)
        {
            case 1:
                MostrarInstruccion("Di el comando: \"Sentado\" o \"Sientate\"");
                esperandoComando = true; // Esperando el comando de voz
                comandoComprendido = false; // El perro no comprende el comando aún
                break;

            case 2:
                MostrarInstruccion("Muestra galleta, dándole clic");
                if (botonGalleta != null) botonGalleta.SetActive(true); // Mostrar botón de galleta
                esperandoGalleta = true; // Esperando que el usuario haga clic en la galleta
                break;

            case 3:
                MostrarInstruccion("Di el comando: \"Sentado\" o \"Sientate\"");
                esperandoComando = true; // Esperando el comando de voz
                comandoComprendido = true; // El perro ya comprende el comando
                break;

            case 4:
                MostrarInstruccion("Da la galleta, dando clic");
                if (botonGalleta != null) botonGalleta.SetActive(true); // Mostrar botón de galleta
                esperandoGalleta = true; // Esperando que el usuario haga clic en la galleta
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
                MostrarInstruccion("Muestra galleta, dándole clic");
                if (botonGalleta != null) botonGalleta.SetActive(true);
                esperandoGalleta = true;
                break;

            case 2:
                MostrarInstruccion("Di el comando: \"Sentado\" o \"Sientate\"");
                esperandoComando = true;
                break;

            case 3:
                // Instrucción para acariciar al perro
                MostrarInstruccion("Acaricia a tu perrito, dando clic en la mano");
                if (botonAcariciar != null) botonAcariciar.SetActive(true); // Mostrar botón de acariciar
                esperandoAcariciar = true; // Esperando que el usuario haga clic en acariciar
                break;

            case 4:
                MostrarInstruccion("Muy bien, tu perrito ya no necesita más premios para hacer el comando.");

                // Esperar un momento y luego avanzar
                StartCoroutine(AvanzarDeNivel(5f));
                break;
        }
    }

    // Método para avanzar de nivel
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
            MostrarInstruccion("Reiniciando entrenamiento...");
            yield return new WaitForSeconds(3f);
            nivelActual = 0;
            IniciarNivel1();
        }
    }

    // Método para mostrar y ocultar el modelo 3D
    private IEnumerator MostrarYOcultarModelo(GameObject modelo, float duracion)
    {
        if (modelo != null)
        {
            modelo.SetActive(true);
            yield return new WaitForSeconds(duracion);
            modelo.SetActive(false);
        }
    }

    // Método para procesar el comando de voz
    public void ProcesarComandoVoz(string textoReconocido)
    {
        // Convertir a minúsculas para facilitar la comparación
        string textoLower = textoReconocido.ToLower().Trim();

        Debug.Log("Texto reconocido: " + textoLower);

        // Verificar si estamos esperando un comando de voz para el entrenamiento
        if (esperandoComando)
        {
            // Verificar si el comando corresponde a "Sentado"
            if (ContienePalabra(textoLower, comandoSentado))
            {
                esperandoComando = false;

                // Niveles de entrenamiento
                if (nivelActual == 1)
                {
                    if (comandoComprendido)
                    {
                        // Paso2: El perro ya comprende el comando
                        EjecutarComando("sentado");
                        AvanzarPasoNivel1();
                    }
                    else
                    {
                        // Paso 1: El perro no comprende aún el comando
                        StartCoroutine(MostrarYOcultarModelo(modeloInterrogacion, 2f));
                        MostrarFeedback("¿?");
                        AvanzarPasoNivel1();
                    }
                }
                else if (nivelActual == 2)
                {
                    // Nivel 2: el perro ya comprende el comando pero aun necesita refuerzo
                    EjecutarComando("sentado");
                    AvanzarPasoNivel2();
                }
                else if (nivelActual == 3)
                {
                    // Nivel 3: el perro comprende perfectamente el comando
                    EjecutarComando("sentado");
                    AvanzarPasoNivel3();
                }
            }
        }
        else if (!esperandoFinAnimacion)
        {
            // Verificar si el comando corresponde a "Pata"
            if (ContienePalabra(textoLower, comandoSentado))
            {
                EjecutarComando("sentado");
            }
        }
    }

    // Método para verificar si el texto contiene alguna de las palabras clave
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

    // Método para ejecutar el comando de animación
    public void EjecutarComando(string comando)
    {
        // Si ya está esperando el fin de una animación, no hacer nada
        if (esperandoFinAnimacion)
            return;

        switch (comando.ToLower())
        {
            case "sentado":
                if (characterAnimator != null)
                {
                    // Primero desactivamos el reset si estaba activo
                    characterAnimator.SetBool(paramReset, false);

                    // Ejecutamos la animación solicitada
                    characterAnimator.SetTrigger(triggerPata);
                    MostrarFeedback("¡Sentado!");
                    StartCoroutine(EsperarFinAnimacion(1.5f)); // Ajusta este tiempo según la duración de la animación
                }
                break;

            default:
                Debug.LogWarning("Comando desconocido: " + comando);
                break;
        }
    }

    // Evita que se activen varias animaciones simultáneamente
    private IEnumerator EsperarFinAnimacion(float tiempoEspera)
    {
        esperandoFinAnimacion = true;
        yield return new WaitForSeconds(tiempoEspera);
        esperandoFinAnimacion = false;
    }

    // Método para mostrar feedback al usuario
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

    // Método para ocultar el feedback después de un tiempo
    private IEnumerator OcultarFeedbackDespuesDeTiempo()
    {
        yield return new WaitForSeconds(tiempoMostrarFeedback);
        if (feedbackText != null)
        {
            feedbackText.text = ""; // Limpiar el texto de feedback
        }
        feedbackCoroutine = null;
    }

    // Método para mostrar instrucciones al usuario
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

    // Método para mostrar el texto con efecto de escritura
    private IEnumerator MostrarTextoConEfectoEscritura(string textoCompleto)
    {
        if (instructionText != null)
        {
            instructionText.text = ""; // Limpiar el texto anterior

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

    // Método para mantener la instrucción visible por un tiempo
    private IEnumerator MantenerInstruccionVisible()
    {
        yield return new WaitForSeconds(tiempoMostrarInstruccion); // Tiempo de espera
        instructionCoroutine = null; // Limpiar la corrutina
    }

    // Método para el botón galleta
    public void ClickGalleta()
    {
        if (esperandoGalleta)
        {
            esperandoGalleta = false; // Desactivar la espera
            botonGalleta.SetActive(false); // Desactivar el botón

            if (nivelActual == 1)
            {
                if (pasoActual == 2)
                {
                    // Se muestra galleta para llamar la atención del perro
                    StartCoroutine(MostrarYOcultarModelo(modeloAdmiracion, 2f));
                    MostrarFeedback("¡Galleta!");
                    AvanzarPasoNivel1();
                }
                else if (pasoActual == 4)
                {
                    // Se da la galleta después de que el perro ha hecho el comando
                    AvanzarPasoNivel1();
                }
            }
            else if (nivelActual == 2)
            {
                // Se muestra galleta para llamar la atención del perro
                StartCoroutine(MostrarYOcultarModelo(modeloAdmiracion, 2f));
                MostrarFeedback("¡Galleta!");
                AvanzarPasoNivel2();
            }
        }
    }

    // Método para el botón acariciar
    public void ClickAcariciar()
    {
        if (esperandoAcariciar)
        {
            esperandoAcariciar = false; // Desactivar la espera
            botonAcariciar.SetActive(false);// Desactivar el botón

            // Se acaricia al perro en el nivel 2 o 3 como recompensa
            if (nivelActual == 2 && pasoActual == 3)
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
            esperandoPelota = false; // Desactivar la espera
            botonPelota.SetActive(false); // Desactivar el botón

            // Se juega con el perro en el nivel 3 como recompensa final
            if (nivelActual == 3 && pasoActual == 3)
            {
                StartCoroutine(MostrarYOcultarModelo(modeloCorazon, 2f));
                MostrarFeedback("♥");
                AvanzarPasoNivel3();
            }
        }
    }

    // Método para iniciar el nivel 3
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
                // Mostrar instrucción para decir el comando directamente
                MostrarInstruccion("Di el comando: \"Sentado\" o \"Sientate\"");
                esperandoComando = true;
                break;

            case 2:
                // Instrucción para acariciar al perro
                MostrarInstruccion("¡Muy bien! Tu perro ya sabe el comando. Acaricia a tu perrito, dando clic en la mano");
                if (botonAcariciar != null) botonAcariciar.SetActive(true);
                esperandoAcariciar = true;
                break;

            case 3:
                // Instrucción para jugar con el perro
                MostrarInstruccion("Tu perrito merece una mayor recompensa, ¡juega con él! Da clic en la pelota");
                if (botonPelota != null) botonPelota.SetActive(true);
                esperandoPelota = true;
                break;

            case 4:
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
        }
    }
}