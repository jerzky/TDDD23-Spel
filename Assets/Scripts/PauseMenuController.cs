
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;
    public static bool IsPaused { get; private set; }

    [SerializeField] 
    [NotNull] 
    private GameObject _pausePanel;
    void Start()
    {
        Instance = this;
        _pausePanel.SetActive(false);
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) 
            return;
        

     
        if (!_pausePanel.activeInHierarchy)
        {
            Time.timeScale = 0;
            _pausePanel.SetActive(true);
            IsPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            _pausePanel.SetActive(false);
            IsPaused = false;
        }
    }

    public void SaveGameButtonClicked()
    {
        GameController.Instance.SaveGame();
    }
    public void ExitToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        Application.Quit();
    }
}
