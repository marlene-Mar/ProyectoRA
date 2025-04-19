using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSystem : MonoBehaviour
{
    public void home()
    {
        SceneManager.LoadScene(0);
    }

    public void play()
    {
        Debug.Log("Iniciando el juego");
    }

}
