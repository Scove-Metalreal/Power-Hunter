using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject box;
    public GameObject option;


    private bool isOptionActive = false;

    public void ToggleOption()
    {
        isOptionActive = !isOptionActive;

        option.SetActive(isOptionActive);
        box.SetActive(!isOptionActive);
    }

    public void BackToMenu()
    {
        isOptionActive = false;
        option.SetActive(false);
        box.SetActive(true);
    }
    public void PlayGame()
    {
        // Dừng nhạc nền Main Menu
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
        }

        SceneManager.LoadScene("Level 1");
    }
    public void ExitGame()
    { 
        Application.Quit(); 
    #if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false; 
    #endif 

    }

}
