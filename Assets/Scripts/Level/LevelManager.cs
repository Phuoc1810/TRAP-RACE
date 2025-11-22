using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int currentLevelIndex = 0;
    public int currentLevel = 1;
    public List<Level> levels = new List<Level>();
}
