using UnityEngine;

public class WinManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;

    [Header("UI")]
    [SerializeField] private GameObject winUI;

    public void ContinueNextLevel()//Add to the button
    { 
        gridManager.StartNextLevel();

        //Ẩn UI win
        winUI.SetActive(false);
    }
}
