using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : NetworkBehaviour
{
    public GameObject settingsPanel;

    public void onHostButton()
    {
        Debug.Log("Start button pressed");
        NetworkManager.StartHost();
        NetworkManager.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
    public void onJoinButton()
    {
        NetworkManager.StartClient();
    }
    public void onSettingsButton()
    {
        Debug.Log("Settings button pressed");
        settingsPanel.SetActive(true);
    }
    public void onReturnButton()
    {
        Debug.Log("Return button pressed");
        settingsPanel.SetActive(false);
    }
    public void onExitGameButton()
    {
        Debug.Log("Exit game button pressed");
        Application.Quit();
    }
}
