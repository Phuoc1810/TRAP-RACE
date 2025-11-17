using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public string currentSkill; //Lưu kĩ năng hiện tại
    private bool skillSelected = false;
    public void SelecterSkill(string skillName)
    {
        if(skillSelected)
        {
            return;
        }
        currentSkill = skillName;
        Debug.Log("Kĩ năng hiện tại: " + currentSkill);
        skillSelected = true;
    }
}
