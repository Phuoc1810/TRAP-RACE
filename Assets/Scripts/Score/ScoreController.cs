using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int score { get; private set; }
    private float totalTime = 0; //Biến thời gian xác định chuyển phase
    private float intervalTimer = 0; //Xác định thời gian trừ điểm theo phase
    [SerializeField] private Text scoreText;
    private bool changePhase = false;

    [SerializeField] private PathDrawer drawer;
    [SerializeField] private SkillPanelUI skillPanelUI;
    [SerializeField] private ShowTrap showTrap;
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
        if(drawer.drawing == false && skillPanelUI.IsShowing == false && showTrap.IsShowingTraps == false)
        {
            totalTime += Time.deltaTime;
            intervalTimer += Time.deltaTime;
            SetPhase();
        }
    }
    private void LateUpdate()
    {
        scoreText.text = "Score: " + score;
    }
   
    private void SetPhase()
    {
        if (phase == ScorePhase.EarlyPhase)
        {
            if(totalTime > 5f)
            {
                changePhase = true;
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
        changePhase = false;
        phase = ScorePhase.EarlyPhase;
        score = 100;
        totalTime = 0;
        intervalTimer = 0;
    }
}
