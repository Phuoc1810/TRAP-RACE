using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public bool shieldActive = false;//skill khiên càn vật rơi
    public bool shoesActive = false;//skill giày thép gai

    [Header("Helmet model")]
    public GameObject helmet;
    public GameObject helmetVisor;

    public void ActivateShield()
    {
        shieldActive = true;
        helmet.SetActive(true);
        helmetVisor.SetActive(true);
    }

    public void DeActivateShield()
    { 
        shieldActive = false;
        helmet.SetActive(false);
        helmetVisor.SetActive(false);
    }

    public void ResetSkill()
    { 
        if(shieldActive)
            DeActivateShield();
        else if(shoesActive)
            shoesActive = false;
    }
}
