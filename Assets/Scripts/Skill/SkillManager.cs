using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private string skill1 = "Record Trap";
    private string skill2 = "Shoes";
    private string skill3 = "Shield";

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
    public void CheckTrap()
    {
        if(currentSkill == skill1)
        {
            Debug.Log("Record lại 5s xem trap");
        }
        else if(currentSkill == skill2)
        {
            Debug.Log("Kích hoạt giày thép gai");
        }
        else if(currentSkill == skill3)
        {
            Debug.Log("Kích hoạt khiên");
        }
    }
}
