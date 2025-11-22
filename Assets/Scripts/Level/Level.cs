using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int levelNumber;
    public int trapCount;
    public List<string> trapTypes = new List<string>();
    public float showTrapTime;
    public string sceneNameOfNextLevel;//If empty, use current scene
}
