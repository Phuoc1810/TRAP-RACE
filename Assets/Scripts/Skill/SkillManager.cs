using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public string currentSkill; //Lưu kĩ năng hiện tại
    private bool skillSelected = false;

    public PlayerSkill playerSkill;
    [SerializeField] private SkillPanelUI skillPanelUI;

    private void Update()
    {
        if (skillSelected)
        {
            skillPanelUI.HidePanel();
        }
    }
    public void SelecterSkill(string skillName)
    {
        if(skillSelected)
        {
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
            }
            else if (currentSkill == "Shoes")
            {
                playerSkill.shoesActive = true;
            }
        }

        skillSelected = true;
    }
}
