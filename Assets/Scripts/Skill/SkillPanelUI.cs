using System.Collections;
using UnityEngine;

public class SkillPanelUI : MonoBehaviour
{
    [SerializeField] private RectTransform skillPanel;
    [SerializeField] private float speedPosition = 0.08f;

    private Vector2 hiddenPos; //Vị trí dưới màn hình
    private Vector2 shownPos; //Vị trí chính giữa màn hình

    private void Start()
    {
        hiddenPos =new Vector2(0, -582);
        shownPos = new Vector2(0, 0);
        //skillPanel.gameObject.SetActive(false);
    }
    public void ShowPanel()
    {
        skillPanel.gameObject.SetActive(true);
        Vector2 targetPos = Vector2.Lerp(skillPanel.anchoredPosition, shownPos, speedPosition);
        skillPanel.anchoredPosition = targetPos;
    }
    public void HidePanel()
    {
        Vector2 targetPos = Vector2.Lerp(skillPanel.anchoredPosition, hiddenPos, speedPosition);
        skillPanel.anchoredPosition = targetPos;
       // StartCoroutine(SetActive());
    }
    private IEnumerator SetActive()
    {
        yield return new WaitForSeconds(1.5f);
        skillPanel.gameObject.SetActive(false);
    }
}
