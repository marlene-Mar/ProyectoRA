using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingController : MonoBehaviour
{
    public GameObject animator;
    public bool nivel0 = false;
    public bool nivel1 = false;
    public bool nivel2 = false;

    public void ExecuteVoiceCommand(string command)
    {
        switch (command.ToLower())
        {
            case "sentado":
                Sit();
                break;
            case "pata":
                Paw();
                break;
            default:
                Debug.LogWarning("Comando no reconocido: " + command);
                break;
        }
    }

    public void Sit()
    {
        if (nivel0)
        {
            animator.GetComponent<Animator>().SetTrigger("Inicial");
            //El usuario muestra el premio
            Debug.Log("El usuario muestra el premio.");
        }
        else if (nivel1)
        { 
            
        }
        else if (nivel2)
        {
            animator.GetComponent<Animator>().SetTrigger("Sentado");
            // Implementar la lógica para sentarse
            Debug.Log("El personaje se sienta.");
        }
    }

    public void Paw()
    {
        if (nivel0)
        {
            animator.GetComponent<Animator>().SetTrigger("Inicial");
            //El usuario muestra el premio
            Debug.Log("El usuario muestra el premio.");
        }
        else if (nivel1)
        {
            //El usuario muestra el premio. 
            Debug.Log("El usuario muestra el premio.");
        }
        else if (nivel2)
        {
            animator.GetComponent<Animator>().SetTrigger("Pata");
            // Implementar la lógica para dar la pata
            Debug.Log("El personaje da la pata.");
        }
    }

    public void premio()
    {
        
    }
}
