using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public string currentSkill; //Lưu kĩ năng hiện tại
    private bool skillSelected = false;
    private bool recordTrapActive = false;

    public PlayerSkill playerSkill;
    [SerializeField] private SkillPanelUI skillPanelUI;
    [SerializeField] private ShowTrap showTrapScript;
    [SerializeField] private ScoreController scoreController;

    public bool SkillSelected => skillSelected;
    public bool RecordTrapActive => recordTrapActive;

    public void SelecterSkill(string skillName)
    {
        if (skillSelected)
        {
            StartCoroutine(skillPanelUI.HidePanel(true));
            return;
        }
        currentSkill = skillName;
        Debug.Log("Kĩ năng hiện tại: " + currentSkill);

        //Bật kỹ năng của nhân vật
        if (playerSkill != null)
        {
            if (currentSkill == "Shield")
            {
                playerSkill.ActivateShield();
                StartCoroutine(skillPanelUI.HidePanel(true));
                scoreController.DecreaseScoreWhenSelectorSkill();
                GamePhaseManager.Instance.CompleteChooseSkill();
            }
            else if (currentSkill == "Shoes")
            {
                playerSkill.shoesActive = true;
                StartCoroutine(skillPanelUI.HidePanel(true));
                scoreController.DecreaseScoreWhenSelectorSkill();
                GamePhaseManager.Instance.CompleteChooseSkill();
            }
            else if (currentSkill == "Record Trap")
            {
                recordTrapActive = true;
                StartCoroutine(RecordTrap());
                scoreController.DecreaseScoreWhenSelectorSkill();
                recordTrapActive = false;
            }
            else
            {
                StartCoroutine(skillPanelUI.HidePanel(true));
                GamePhaseManager.Instance.CompleteChooseSkill();
            }
        }

        skillSelected = true;
    }
    public void EnableSelectSkill()
    { 
        skillSelected = false;
    }

    public IEnumerator RecordTrap()
    {
        yield return StartCoroutine(skillPanelUI.HidePanel(false));
        yield return StartCoroutine(showTrapScript.ShowAllTrap(3f));
        showTrapScript.EnableController();

        
    }
}
