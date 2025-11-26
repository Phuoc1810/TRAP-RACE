using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int currentLevelIndex = 0;
    public int currentLevel = 1;
    public List<Level> levels = new List<Level>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentLevel = 1;
            currentLevelIndex = 0;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levels.Count)
        {
            currentLevel = levels[currentLevelIndex].levelNumber;
            // Load the next level scene here using levels[currentLevelIndex].sceneNameOfNextLevel
            if (levels[currentLevelIndex].sceneNameOfNextLevel != "")//If not empty, load the specified scene
            { 
                SceneManager.LoadScene(levels[currentLevelIndex].sceneNameOfNextLevel);
            }
        }
        else
        {
            Debug.Log("No more levels to load.");
        }
    }

    public bool CheckIfLastLevel()
    {
        return currentLevelIndex >= levels.Count - 1;
    }
}
