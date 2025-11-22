using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelUI : MonoBehaviour
{
    [SerializeField] private RectTransform skillPanel;
    [SerializeField] private float speedPosition = 0.08f;

    private Vector2 hiddenPos; //Vị trí dưới màn hình
    private Vector2 shownPos; //Vị trí chính giữa màn hình
    private Vector2 targetPos;

    [SerializeField] private GameObject skillInforPanel;
    [SerializeField] private Text textInfor;
    [SerializeField] Button[] skillButtons;

    [Header("Text Setting")]
    private float minAlpha = 0.3f;
    private float maxAlpha = 1f;
    private float speedAlpha = 0.5f;
    private float alphaValue;

    [Header("References")]
    [SerializeField] private PathDrawer pathDrawer;
    [SerializeField] private SkillManager skillManager;

    private bool isShowing = false;
    private bool canClickButtons = false;
    private void Start()
    {
        hiddenPos =new Vector2(0, -582);
        shownPos = new Vector2(0, 0);
    }
    private void Update()
    {
        targetPos = isShowing ? shownPos : hiddenPos;
        alphaValue = Mathf.PingPong(Time.time * speedAlpha, maxAlpha - minAlpha) + minAlpha;
        ChangeButtonsState();
        ChangeAlphaValueForText();
    }
    public void ShowPanel()
    {
        isShowing = true;
        skillPanel.gameObject.SetActive(true);
        StartCoroutine(MovePanel());
        StartCoroutine(ShowInfor());
    }
    public IEnumerator HidePanel(bool enableDrawing)
    {
        isShowing = false;
        canClickButtons = false;//Không cho bấm nút khi đang ẩn panel
        skillInforPanel.SetActive(false);
        yield return StartCoroutine(MovePanel());
        skillPanel.gameObject.SetActive(false);

        if (enableDrawing)
        {
            pathDrawer.EnableDrawing();
        }

        //StartCoroutine(HideInfor());
    }
    private void ChangeAlphaValueForText()
    {
        Color color = textInfor.color;
        color.a = alphaValue;
        textInfor.color = color;
    }
    private IEnumerator ShowInfor()
    {
        yield return new WaitForSeconds(0.4f);
        skillInforPanel.SetActive(true);
    }
    private IEnumerator MovePanel()
    {
        float duration = 0.5f;
        float t = 0f;
        Vector2 startPos = skillPanel.anchoredPosition;
        while (t < duration)
        {
            t += Time.deltaTime;
            skillPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t / duration);   
            yield return null;
        }

        skillPanel.anchoredPosition = targetPos;
        if(skillPanel.anchoredPosition == shownPos)
        {
            canClickButtons = true;
        }
    }
    private void ChangeButtonsState()
    {
        foreach (Button btn in skillButtons)
        {
            btn.interactable = canClickButtons;
        }
    }
}
