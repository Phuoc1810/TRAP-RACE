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
        }
        else
        {
            gridManager.StartNextLevel();
        }
        //Ẩn UI win
        winUI.SetActive(false);
    }
}
