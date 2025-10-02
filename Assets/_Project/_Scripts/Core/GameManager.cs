using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gamePauseUI;
    public GameObject MenuOptionUI;
    public PlayerController playerController;
    public bool isGameEnd;
    public static int SceneIndex;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickClip;
    public GameObject FullMapUI;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        gamePauseUI.SetActive(false);
        MenuOptionUI.SetActive(false);
        isGameEnd = false;
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        FullMapUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isGameEnd == false )
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                Pause();
                if (sfxSource != null && clickClip != null)
                {
                    sfxSource.PlayOneShot(clickClip);
                }
            }
        }
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (Input.GetKey(KeyCode.M))
        {
            FullMapUI.SetActive(true);
        }
        else {  FullMapUI.SetActive(false);}
    }
    public void Option()
    {
        MenuOptionUI.SetActive(true);
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Again()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
        isGameEnd = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController.GrafityDown = true;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        gamePauseUI.SetActive(true);

        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Remuse()
    {
        Time.timeScale = 1f;
        gamePauseUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void MainMenuInGameEndUI()
    {
        SceneManager.LoadScene("MainMenu");
        isGameEnd = false;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Play()  
    {
        if (SceneIndex == 0)
        {
            SceneIndex = 1;
        }
        SceneManager.LoadScene(SceneIndex);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
    public void Back()
    {
        if (gamePauseUI.activeSelf == true  && gamePauseUI != null)
        {
            Time.timeScale = 1f;
            gamePauseUI.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (sfxSource != null && clickClip != null)
            {
                sfxSource.PlayOneShot(clickClip);
            }
        }

        if (MenuOptionUI.activeSelf == true && MenuOptionUI != null)
        {
            MenuOptionUI.SetActive(false);
            if (sfxSource != null && clickClip != null)
            {
                sfxSource.PlayOneShot(clickClip);
            }
        }

    }
    public void Exitgame()
    {
        Application.Quit();
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip);
        }
    }
}
