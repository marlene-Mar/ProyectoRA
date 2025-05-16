using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    //Paneles de la aplicación
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Trucos;
    [SerializeField] private GameObject Perros;
    [SerializeField] private GameObject Tips;
    [SerializeField] private GameObject InsSentado;
    [SerializeField] private GameObject InsPata;
    [SerializeField] private GameObject EntSentado;
    [SerializeField] private GameObject EntPata;
    [SerializeField] private GameObject Trufoso;
    [SerializeField] private GameObject Trufosos;
    [SerializeField] private GameObject camara;

    //Botones auxiliares para resetear datos al finalizar el entrenamiento
    [SerializeField] private GameObject HomeEntSentado;
    [SerializeField] private GameObject HomeEntPata;

    // Variables de estado
    public int Chihuahua = 0;
    public int SentadoSel = 0;
    public int PataSel = 0;

    // URL de los marcadores a descargar 
    private string urlMarcadores = "https://drive.google.com/drive/folders/1Ik577slklY2LfqS4HS4PPy8ICC8acpBN?usp=sharing";

   
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
        // Inicialmente solo muestra el menú principal
        ShowOnlyPanel(Menu);
    }

    // Función para mostrar solo el panel que el usuario elige
    public void ShowOnlyPanel(GameObject panelToShow)
    {
        if (Menu) Menu.SetActive(panelToShow == Menu);
        if (Trucos) Trucos.SetActive(panelToShow == Trucos);
        if (Perros) Perros.SetActive(panelToShow == Perros);
        if (Tips) Tips.SetActive(panelToShow == Tips);
        if (InsSentado) InsSentado.SetActive(panelToShow == InsSentado);
        if (InsPata) InsPata.SetActive(panelToShow == InsPata);
        if (EntSentado) EntSentado.SetActive(panelToShow == EntSentado);
        if (EntPata) EntPata.SetActive(panelToShow == EntPata);
        if (Trufoso) Trufoso.SetActive(panelToShow == Trufoso);
        if (Trufosos) Trufosos.SetActive(panelToShow == Trufosos);
    }

    // Método para ir al menú principal
    public void GoToHome()
    {
        ShowOnlyPanel(Menu);
    }

    // Métodos para ir a los diferentes paneles
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

    // Método para descargar los marcadores
    public void DownloadMarcadores()
    {
        Application.OpenURL(urlMarcadores);  
    }

    // Métodos para seleccionar los perros
    public void chihuahua()
    {
        ShowOnlyPanel(Trufoso);
        camara.SetActive(true);
    }

    public void trufosos()
    {
        ShowOnlyPanel(Trufosos);
        camara.SetActive(true);
    }

    // Métodos para regresar al panel de perros
    public void back()
    {
        ShowOnlyPanel(Perros);
        camara.SetActive(false);
    }

    // Métodos para seleccionar el perro a entrenar e ir a elegir truco
    public void OK()
    {
        Chihuahua = 1;
        ShowOnlyPanel(Trucos);
        camara.SetActive(true);
    }

    // Métodos para seleccionar los trucos
    public void SentadoTru()
    {
        PataSel = 0;
        SentadoSel = 1;
        ShowOnlyPanel(InsSentado);
    }

    public void PataTru()
    {
        SentadoSel = 0;
        PataSel = 1;
        ShowOnlyPanel(InsPata);
    }

    // Metodo para iniciar el entrenamiento
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
            ShowOnlyPanel(EntPata);
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
    // Método específico para volver al Home desde pantallas de entrenamiento
    public void Home2()
    {
        // Resetear selecciones de trucos
        SentadoSel = 0;
        PataSel = 0;

        // Desactivar la cámara
        if (camara) camara.SetActive(false);

        // Volver al menú principal
        ShowOnlyPanel(Menu);
    }

    public void Salir()
    {
        Debug.Log("Saliendo de la app");
        Application.Quit();
    }
}
