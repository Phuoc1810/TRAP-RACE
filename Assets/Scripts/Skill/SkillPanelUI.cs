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

    [SerializeField] private GameObject skillInforPanel;
    [SerializeField] private Text textInfor;

    [Header("Text Setting")]
    private float minAlpha = 0.3f;
    private float maxAlpha = 1f;
    private float speedAlpha = 0.5f;
    private float alphaValue;

    private void Start()
    {
        hiddenPos =new Vector2(0, -582);
        shownPos = new Vector2(0, 0);
    }
    private void Update()
    {
        alphaValue = Mathf.PingPong(Time.time * speedAlpha, maxAlpha - minAlpha) + minAlpha;
        ChangeAlphaValueForText();
    }
    public void ShowPanel()
    {
        skillPanel.gameObject.SetActive(true);
        Vector2 targetPos = Vector2.Lerp(skillPanel.anchoredPosition, shownPos, speedPosition);
        skillPanel.anchoredPosition = targetPos;
        StartCoroutine(ShowInfor());
    }
    public void HidePanel()
    {
        Vector2 targetPos = Vector2.Lerp(skillPanel.anchoredPosition, hiddenPos, speedPosition);
        skillPanel.anchoredPosition = targetPos;
       StartCoroutine(HideInfor());
    }
    private void ChangeAlphaValueForText()
    {
        Color color = textInfor.color;
        color.a = alphaValue;
        textInfor.color = color;
    }
    private IEnumerator ShowInfor()
    {
        yield return new WaitForSeconds(1.5f);
        skillInforPanel.SetActive(true);
    }
    private IEnumerator HideInfor()
    {
        yield return new WaitForSeconds(1.5f);
        skillInforPanel.SetActive(false);
    }
}
