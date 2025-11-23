using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int score { get; private set; }
    private float totalTime = 0; //Biến thời gian xác định chuyển phase
    private float intervalTimer = 0; //Xác định thời gian trừ điểm theo phase
    [SerializeField] private Text scoreText;

    private bool isCouting = false; //Bật/ tắt tính điểm
    public enum ScorePhase
    {
        EarlyPhase,
        LatePhase
    }
    private ScorePhase phase = ScorePhase.EarlyPhase;

    void Start()
    {
        score = 100;
    }
    void Update()
    {
        if(isCouting)
        {
            totalTime += Time.deltaTime;
            intervalTimer += Time.deltaTime;
            DeductScore();
        }
    }
    public void StartCountingScore()
    {
        isCouting = true;
    }
    public void StopCountingScore()
    {
        isCouting = false;
    }
    private void LateUpdate()
    {
        scoreText.text = "Score: " + score + " " + totalTime;
    }
   
    private void DeductScore()
    {
        if (phase == ScorePhase.EarlyPhase)
        {
            if(totalTime > 5f)
            {
                phase = ScorePhase.LatePhase;
            }
            if(intervalTimer >= 5f)
            {
                score -= 10;
                score = Mathf.Max(score, 0);
                intervalTimer = 0;
            }
        }
        if(phase == ScorePhase.LatePhase)
        {
            if(intervalTimer > 2f)
            {
                score -= 10;
                score = Mathf.Max(score, 0);
                intervalTimer = 0;
            }
        }
    }
    public void DecreaseScoreWhenSelectorSkill()
    {
        score -= 20;
        score = Mathf.Max(score, 0);
    }
    public void ResetScore()
    {
        phase = ScorePhase.EarlyPhase;
        score = 100;
        totalTime = 0;
        intervalTimer = 0;
    }
}
