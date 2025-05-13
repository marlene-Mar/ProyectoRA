using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;  // Importar TextMeshPro
using KKSpeech;

namespace KKSpeech
{
    public class SpeechRecognitionLanguageDropdown : MonoBehaviour
    {
        // Cambiar de Dropdown a TMP_Dropdown
        private TMP_Dropdown dropdown;
        private List<LanguageOption> languageOptions;

        void Start()
        {
            // Obtener el componente TMP_Dropdown en lugar de Dropdown
            dropdown = GetComponent<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            dropdown.ClearOptions();
            GameObject.FindObjectOfType<SpeechRecognizerListener>().
                onSupportedLanguagesFetched.
                AddListener(OnSupportedLanguagesFetched);
            SpeechRecognizer.GetSupportedLanguages();
        }

        public void GoToRecordingScene()
        {
            SceneManager.LoadScene("ExampleScene");
        }

        void OnDropdownValueChanged(int index)
        {
            LanguageOption languageOption = languageOptions[index];
            SpeechRecognizer.SetDetectionLanguage(languageOption.id);
        }

        void OnSupportedLanguagesFetched(List<LanguageOption> languages)
        {
            // Cambiar a usar TMP_Dropdown.OptionData
            List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
            foreach (LanguageOption langOption in languages)
            {
                dropdownOptions.Add(new TMP_Dropdown.OptionData(langOption.displayName));
            }
            dropdown.AddOptions(dropdownOptions);
            languageOptions = languages;
        }
    }
}

