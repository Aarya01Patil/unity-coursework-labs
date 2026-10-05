using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Optional")]
    [SerializeField] private string gameSceneName;
    [SerializeField] private bool pauseWithTimeScale = true;

    private void Start()
    {
        ShowMainMenu();
    }

    public void StartGame()
    {
        Resume();

        if (!string.IsNullOrWhiteSpace(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void OpenOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void BackFromOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowMainMenu()
    {
        Pause();
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void Pause()
    {
        if (pauseWithTimeScale) Time.timeScale = 0f;
    }

    private void Resume()
    {
        if (pauseWithTimeScale) Time.timeScale = 1f;
    }
}

