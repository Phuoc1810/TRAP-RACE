using UnityEngine;

public class WinManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;

    [Header("UI")]
    [SerializeField] private GameObject winUI;

    public void ContinueNextLevel()//Add to the button
    {
        if (LevelManager.Instance.CheckIfLastLevel())// If it's the last level
        {
            //Call restart or main menu function
            gridManager.ResetLevel();
        }
        else
        {
            gridManager.StartNextLevel();
            MenuManager.instance.SetLevelText();
        }
        //Ẩn UI win
        winUI.SetActive(false);
    }
}
