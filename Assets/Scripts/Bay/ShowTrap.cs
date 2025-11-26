using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTrap : MonoBehaviour
{
    public static ShowTrap Instance;

    public GridManager gridManager;
    public GameObject spikeTrapSprite;
    public GameObject HammerTrapSprite;
    public GameObject RockTrapSprite;

    [Header("Count down")]
    public Text countDownShowTrapText;
    //public float countDownTime = 5f;
    private float currentTime;

    [Header("references đến các scripts")]
    public SkillPanelUI skillPanelUI;
    [SerializeField] private PathDrawer pathDrawer;
    [SerializeField] private SkillManager skillManager;


    public GameObject ShowTrapAt(int x, int y)
    {
        GameObject trapPos = gridManager.gridArray[x, y];
        GameObject trapSprite = null;
        if (trapPos.transform.GetChild(0).TryGetComponent<HammerTrigger>(out HammerTrigger hammer))
        {
            trapSprite = Instantiate(HammerTrapSprite, trapPos.transform.position + new Vector3(0, 0.6f, 0), Quaternion.identity);
        }
        else if (trapPos.transform.GetChild(0).TryGetComponent<RockTrap>(out RockTrap rockTrap))
        {
            trapSprite = Instantiate(RockTrapSprite, trapPos.transform.position + new Vector3(0, 0.6f, 0), Quaternion.identity);
        }
        else if (trapPos.transform.GetChild(0).TryGetComponent<SpikeTrap>(out SpikeTrap spikeTrap))
        {
            trapSprite = Instantiate(spikeTrapSprite, trapPos.transform.position + new Vector3(0, 0.6f, 0), Quaternion.identity);
        }

        trapSprite.transform.localScale = Vector3.zero;
        return trapSprite;
    }

    public void BeginShowTrap()
    {
        DisableController();
        StartCoroutine(ShowAllTrap(LevelManager.Instance.levels[LevelManager.Instance.currentLevelIndex].showTrapTime));
    }

    public IEnumerator ShowAllTrap(float countDownTime)
    {
        //Show text
        countDownShowTrapText.gameObject.SetActive(true);
        countDownShowTrapText.text = "Traps will be hidden in: " + countDownTime;
        countDownShowTrapText.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        // Scale up animation
        StartCoroutine(ScaleText(Vector3.one));


        List<GameObject> trapSprites = new List<GameObject>();
        foreach (Vector2Int trapPos in gridManager.trapPositions)
        { 
            GameObject trapSprite = ShowTrapAt(trapPos.x, trapPos.y);
            // Scale up animation
            StartCoroutine(ScaleSprite(trapSprite, Vector3.one));

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
            // Scale down animation
            StartCoroutine(ScaleSprite(trapSprite, Vector3.zero));
        }
        currentTime = 0;
        yield return StartCoroutine(ScaleText(new Vector3(0.5f, 0.5f, 0.5f)));
        countDownShowTrapText.text = "";
        countDownShowTrapText.gameObject.SetActive(false);

        //Show skill panel
        if (currentTime == 0 && skillManager.SkillSelected == false)
        {
            skillPanelUI.ShowPanel();
        }

        foreach (GameObject trapSprite in trapSprites)
        {
            Destroy(trapSprite);
        }

        //Thông báo cho GamePhaseManager
        if (skillManager != null && skillManager.SkillSelected == false)
        {
            GamePhaseManager.Instance.CompleteShowTrap();
        }
        else
        {
            if (skillManager.RecordTrapActive == false)
            {
                GamePhaseManager.Instance.CompleteChooseSkill();
            }
            else
            {
                GamePhaseManager.Instance.CompleteRecordTrap();
            }
        }
    }

    public IEnumerator ScaleSprite(GameObject sprite, Vector3 targetScale)
    {
        Vector3 originalScale = sprite.transform.localScale;
        float duration = 0.15f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            sprite.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        sprite.transform.localScale = targetScale;
    }

    public IEnumerator ScaleText(Vector3 targetScale)
    { 
        Vector3 originalScale = countDownShowTrapText.transform.localScale;
        float duration = 0.15f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            countDownShowTrapText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        countDownShowTrapText.transform.localScale = targetScale;
    }
    // sau khi hiển thị bẫy xong thì cho vẽ đường đi
    public void EnableController() 
    {
        if (pathDrawer != null)
        {
            pathDrawer.EnableDrawing();
        }
    }
    // trong khi hiển thị bẫy xong thì cho vẽ đường đi
    public void DisableController()
    {
        if (pathDrawer != null)
        {
            StartCoroutine(pathDrawer.DisableDrawing());
        }
    }
}
