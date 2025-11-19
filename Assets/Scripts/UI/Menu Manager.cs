using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public Button startButton;
    public GameObject settingPanel;
    public GameObject menuPanel;
    public bool isInGame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        // start button disappear
        startButton.gameObject.SetActive(false);

        TextMoving.instance.isInGame = true;
        TextMoving.instance.TextMove();

    }

    public void BackToMenu()
    {
        SettingPanelClose();
        // start button appear
        startButton.gameObject.SetActive(true);
        TextMoving.instance.isInGame = false;
        TextMoving.instance.TextMove();
    }

    public void SettingPanelOpen()
    {
        // open setting panel
        settingPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void SettingPanelClose()
    {
        // close setting panel
        settingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
   