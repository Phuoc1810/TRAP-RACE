using UnityEngine;

public class TriggerTrap : MonoBehaviour
{
    public PlayerSkill playerSkill;
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //Xử lý va chạm với bẫy
        if (other.CompareTag("Bẫy rơi"))// || other.CompareTag("Bẫy búa"))
        {
            if (playerSkill.shieldActive)//nếu có khiên thì không sao
            {
                Debug.Log("Nhân vật có khiên");
            }
            else
            {
                //Xử lý nhân vật bị bẫy rơi
                //Debug.Log("Nhân vật bị bẫy rơi");
                LoseGame();
            }
        }
        else if (other.CompareTag("Bẫy gai"))
        {
            if (playerSkill.shoesActive)//nếu có giày thép gai thì không sao
            {
                Debug.Log("Nhân vật có giày thép gai");
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
        animator.SetTrigger("isDead");

        //hiển thị UI thua game
    }
}
