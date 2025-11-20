using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public Button startButton;
    public GameObject optionsPanel;
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
        if (TextMoving.instance.isInGame)
        {
            OptionsPanelClose();
            // start button appear
            startButton.gameObject.SetActive(true);
            TextMoving.instance.isInGame = false;
            TextMoving.instance.TextMove();
        } else 
            {
            Debug.Log("Not in game");
        }
    }

    public void OptionsPanelOpen()
    {
        // open setting panel
        optionsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void OptionsPanelClose()
    {
        // close setting panel
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void SettingsPanelOpen()
    {
        settingPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }
    public void SettingPanelClose()
    {
        settingPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
}
   