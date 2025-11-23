using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int score { get; private set; }
    private float totalTime = 0; //Biến thời gian xác định chuyển phase
    private float intervalTimer = 0; //Xác định thời gian trừ điểm theo phase
    [SerializeField] private Text timeText;
    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private GameObject midScorePanel;
    [SerializeField] private GameObject lowScorePanel;

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
        timeText.gameObject.SetActive(false);
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
        timeText.gameObject.SetActive(true);
    }
    public void StopCountingScore()
    {
        isCouting = false;
        timeText.gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        int seconds = Mathf.FloorToInt(totalTime);
        timeText.text = seconds.ToString("00:00");
        scoreText.text = "Score: " + score.ToString();
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
    private void Rating()
    {
        switch (score)
        {
            
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
