using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Añadido el namespace necesario para SceneManager
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Singleton para acceso fácil
    public static UIManager Instance { get; private set; }

    // Referencias a todos los paneles UI (que reemplazarán las escenas)
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Trucos;
    [SerializeField] private GameObject Perros;
    [SerializeField] private GameObject Tips;
    [SerializeField] private GameObject InsSentado;
    [SerializeField] private GameObject InsPata;
    [SerializeField] private GameObject EntSentado;
    [SerializeField] private GameObject Trufoso;

    // Referencia a la cámara que activas/desactivas
    [SerializeField] private GameObject camara;

    // Variables de estado (las mismas que tienes ahora)
    public int Chihuahua = 0;
    public int SentadoSel = 0;
    public int PataSel = 0;

    private void Awake()
    {
        // Configuración del singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Inicialmente mostrar solo el menú principal
        ShowOnlyPanel(Menu);
    }

    // Función para mostrar solo un panel específico
    public void ShowOnlyPanel(GameObject panelToShow)
    {
        Menu.SetActive(panelToShow == Menu);
        Trucos.SetActive(panelToShow == Trucos);
        Perros.SetActive(panelToShow == Perros);
        Tips.SetActive(panelToShow == Tips);
        InsSentado.SetActive(panelToShow == InsSentado);
        InsPata.SetActive(panelToShow == InsPata);
        EntSentado.SetActive(panelToShow == EntSentado);
        Trufoso.SetActive(panelToShow == Trufoso);
    }

    // Funciones corregidas para usar ShowOnlyPanel en lugar de SceneManager

    public void GoToHome()
    {
        ShowOnlyPanel(Menu);
    }

    public void GoToTrucos()
    {
        ShowOnlyPanel(Trucos);
    }

    public void GoToPerros()
    {
        ShowOnlyPanel(Perros);
    }

    public void GoToTips()
    {
        ShowOnlyPanel(Tips);
    }

    public void chihuahua()
    {
        Debug.Log("Chihuahua seleccionado");
        ShowOnlyPanel(Trufoso);
        camara.SetActive(true);
    }

    public void pastor()
    {
        Debug.Log("Pastor seleccionado");
        ShowOnlyPanel(Trufoso);
        camara.SetActive(true);
    }

    public void OK()
    {
        Chihuahua = 1;
        Debug.Log("Trufosos a entrenar, elegir truco:");
        ShowOnlyPanel(Trucos);
        camara.SetActive(true);
    }

    public void SentadoTru()
    {
        Debug.Log("Pasos para seguir el comando Sentado");
        ShowOnlyPanel(InsSentado);
        SentadoSel = 1;
    }

    public void PataTru()
    {
        Debug.Log("Pasos para seguir el comando Pata");
        ShowOnlyPanel(InsPata);
        PataSel = 1;
    }

    public void Play()
    {
        if (Chihuahua == 1 && SentadoSel == 1)
        {
            Debug.Log("Entrenando de Chihuahua");
            ShowOnlyPanel(EntSentado);
            camara.SetActive(true);
        }
        else if (Chihuahua == 1 && PataSel == 1)
        {
            Debug.Log("Entrenando Chihuahua");
            // Panel de entrenamiento pata pendiente
            // ShowOnlyPanel(pataEntrenamientoPanel);
            camara.SetActive(true);
        }
        else
        {
            if (Chihuahua == 0)
            {
                Debug.Log("No has seleccionado un perro");
                ShowOnlyPanel(Perros);
            }
            else if (SentadoSel == 0 || PataSel == 0)
            {
                Debug.Log("No has seleccionado un truco");
                ShowOnlyPanel(Trucos);
            }
        }
    }

    public void Salir()
    {
        Debug.Log("Saliendo de la app");
        Application.Quit();
    }

    // Si necesitas cambiar escenas en el futuro, puedes usar este método
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
