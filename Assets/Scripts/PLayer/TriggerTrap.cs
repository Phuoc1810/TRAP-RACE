using UnityEngine;

public class TriggerTrap : MonoBehaviour
{
    [Header("Reference")]
    public PlayerSkill playerSkill;
    public PlayerMovement playerMovement;

    public Animator animator;
    private int isDeadHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDeadHash = Animator.StringToHash("isDead");
    }

    private void OnTriggerEnter(Collider other)
    {
        //Xử lý va chạm với bẫy
        if (other.CompareTag("BayDa") || other.CompareTag("BayBua"))//Bẫy rơi // || other.CompareTag("Bẫy búa"))
        {
            if (playerSkill.shieldActive)//nếu có khiên thì không sao
            {
                Debug.Log("Nhân vật có khiên");
                playerSkill.DeActivateShield();
            }
            else
            {
                //Xử lý nhân vật bị bẫy rơi
                //Debug.Log("Nhân vật bị bẫy rơi");
                LoseGame();
            }
        }
        else if (other.CompareTag("BayGai"))//Bẫy gai
        {
            if (playerSkill.shoesActive)//nếu có giày thép gai thì không sao
            {
                Debug.Log("Nhân vật có giày thép gai");
                playerSkill.shoesActive = false;
            }
            else
            {
                //Xử lý nhân vật bị bẫy gai
                //Debug.Log("Nhân vật bị bẫy gai");
                LoseGame();
            }
        }
        //else if (other.CompareTag("Bẫy búa"))
        //{ 
        //    LoseGame();
        //}
    }

    public void LoseGame()
    {
        animator.SetBool(isDeadHash, true);
        playerMovement.StopMovement();

        //hiển thị UI thua game
        if (MenuManager.instance != null)
        {
            MenuManager.instance.loseGame();
        }
        else
        {
            Debug.LogWarning("TriggerTrap: MenuManager.Instance is null. Cannot call LoseGame().");
        }
    }
}
