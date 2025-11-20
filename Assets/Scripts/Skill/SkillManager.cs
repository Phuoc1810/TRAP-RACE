using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public string currentSkill; //Lưu kĩ năng hiện tại
    private bool skillSelected = false;

    public PlayerSkill playerSkill;
    [SerializeField] private SkillPanelUI skillPanelUI;
    [SerializeField] private ShowTrap showTrap;


    public void SelecterSkill(string skillName)
    {
        if(skillSelected)
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
            }
            else if (currentSkill == "Shoes")
            {
                playerSkill.shoesActive = true;
                StartCoroutine(skillPanelUI.HidePanel(true));
            }
            else if (currentSkill == "Record Trap")
            {
                StartCoroutine(skillPanelUI.HidePanel(false));
            }
        }

        skillSelected = true;
    }

    public void EnableSelectSkill()
    { 
        skillSelected = false;
    }
}
