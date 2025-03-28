using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject exitMenuPanel;
    [SerializeField] private GameObject optionMenuPanel;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        exitMenuPanel.SetActive(false);
        optionMenuPanel.SetActive(false);
    }
    public void onStartButtonClicked()
    {
        SceneManager.LoadScene(1);
    }
    public void OnOptionButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        optionMenuPanel.SetActive(true);
    }
    public void onOptionCloseButtonClicked()
    {
        mainMenuPanel.SetActive(true);
        optionMenuPanel.SetActive(false);
    }
    public void onExitButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        exitMenuPanel.SetActive(true);
    }
    public void onYesButtonClicked()
    {
        Application.Quit();
    }
    public void onNoButtonClicked()
    {
        mainMenuPanel.SetActive(true);
        exitMenuPanel.SetActive(false);
    }
}
