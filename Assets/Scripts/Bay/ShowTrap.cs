using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTrap : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject spikeTrapSprite;
    public GameObject HammerTrapSprite;
    public GameObject RockTrapSprite;

    [Header("Count down")]
    public Text countDownShowTrapText;
    public float countDownTime = 5f;
    private float currentTime;

    public GameObject ShowTrapAt(int x, int y)
    {
        GameObject trapPos = gridManager.gridArray[x, y];
        GameObject trapSprite = null;
        if (trapPos.transform.GetChild(0).TryGetComponent<HammerTrigger>(out HammerTrigger hammer))
        {
            trapSprite = Instantiate(HammerTrapSprite, trapPos.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
        else if (trapPos.transform.GetChild(0).TryGetComponent<RockTrap>(out RockTrap rockTrap))
        {
            trapSprite = Instantiate(RockTrapSprite, trapPos.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
        else if (trapPos.transform.GetChild(0).TryGetComponent<SpikeTrap>(out SpikeTrap spikeTrap))
        {
            trapSprite = Instantiate(spikeTrapSprite, trapPos.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }

        return trapSprite;
    }

    public void BeginShowTrap()
    { 
        StartCoroutine(ShowAllTrap());
    }

    public IEnumerator ShowAllTrap()
    {
        List<GameObject> trapSprites = new List<GameObject>();
        foreach (Vector2Int trapPos in gridManager.trapPositions)
        { 
            GameObject trapSprite = ShowTrapAt(trapPos.x, trapPos.y);

            ////Xoay sprite về hướng camera
            if (trapSprite != null)
            {
                //trapSprite.transform.LookAt(Camera.main.transform);
                trapSprite.transform.rotation = Quaternion.Euler(-90, 90, 0);
            }

            trapSprites.Add(trapSprite);
        }

        // Start countdown
        currentTime = countDownTime;
        while (currentTime > 0)
        {
            countDownShowTrapText.text = "Traps will be hidden in: " + Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        // Hide all trap sprites
        foreach (GameObject trapSprite in trapSprites)
        {
            Destroy(trapSprite);
        }
        currentTime = 0;
        countDownShowTrapText.text = "";
    }
}
