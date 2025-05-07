using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Trufosos : MonoBehaviour
{

    private GameObject chihuahua; //
    // Referencia al componente Animator
    private Animator animator;

    // Estado actual del perro
    private string estadoActual = "Inicial";

    // GameObject que contendrá el mensaje de estado del reconocimiento (opcional)
    public GameObject panelEstadoVoz;
    public UnityEngine.UI.Text textoEstado;


    public void play()
    {
        Debug.Log("Iniciando entrenamiento");

        //SceneManager.LoadScene(4);
        //camara.SetActive(true);

        animator = GetComponent<Animator>();

       
    }

    // Método para ejecutar el comando "Sentado"
    public void EjecutarSentado()
    {
        // Si ya está en otro estado de entrenamiento, volvemos al estado inicial primero
        if (estadoActual != "Inicial")
        {
            ResetearEstado();
        }

        Debug.Log("Ejecutando comando: Sentado");

        // Activar el trigger para la animación "Sentado"
        animator.SetTrigger("Sentado");
        estadoActual = "Sentado";

        // Opcional: Programar el retorno al estado inicial después de un tiempo
        Invoke("ResetearEstado", 3.0f);
    }

    // Método para ejecutar el comando "Pata"
    public void EjecutarPata()
    {
        if (estadoActual != "Inicial")
        {
            ResetearEstado();
        }

        Debug.Log("Ejecutando comando: Pata");

        animator.SetTrigger("Pata");
        estadoActual = "Pata";

        Invoke("ResetearEstado", 3.0f);
    }

  

    // Método para volver al estado inicial
    public void ResetearEstado()
    {
        // Cancelar cualquier llamada pendiente a ResetearEstado
        CancelInvoke("ResetearEstado");

        // Reset de todos los triggers
        animator.ResetTrigger("Sentado");
        animator.ResetTrigger("Pata");

        // Volver al estado inicial (si has creado un trigger para esto)
        animator.SetTrigger("Inicial");
        estadoActual = "Inicial";
    }

    //Control por teclado para pruebas
    void Update()
    {
        // Control por teclado para pruebas
        if (Input.GetKeyDown(KeyCode.V))
        {
            //IniciarReconocimiento();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            EjecutarSentado();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            EjecutarPata();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ResetearEstado();
        }
    }

}
