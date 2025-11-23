using System;
using UnityEngine;

public enum GamePhase //Phân loại các giai đoạn trong game và quản lý chúng xuyên suốt game
{
    ShowTraps,
    ChosseSkill,
    RecordTrap,
    Draw,
    Move
}
public class GamePhaseManager : MonoBehaviour
{
    public static GamePhaseManager Instance;
    public GamePhase CurrentPhase { get; private set; }

    public event Action<GamePhase> OnPhaseChanged;

    [Header("References")]
    [SerializeField] private ShowTrap showTrapScript;
    [SerializeField] private SkillPanelUI skillPanelUI;
    [SerializeField] private PathDrawer pathDrawer;
    [SerializeField] private ScoreController scoreController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CurrentPhase = GamePhase.ShowTraps; //Bắt đầu game ở giai đoạn ShowTraps
    }

    public void ChangePhase(GamePhase newPhase)
    {
        CurrentPhase = newPhase;

        OnPhaseChanged?.Invoke(newPhase);
        HandlePhase(newPhase);
    }
    private void HandlePhase(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.ShowTraps:
                showTrapScript.BeginShowTrap();
                break;
            case GamePhase.ChosseSkill:
                skillPanelUI.ShowPanel();
                break;
            case GamePhase.RecordTrap:
                skillPanelUI.HidePanel(true);
                break;
            case GamePhase.Draw:
                pathDrawer.EnableDrawing();
                scoreController.StartCountingScore();
                break;
            case GamePhase.Move:
                scoreController.StopCountingScore();
                break;
        }
    }

    #region Các hàm hepler để chuyển phase
    public void CompleteShowTrap()
    {
        ChangePhase(GamePhase.ChosseSkill);
    }
    public void CompleteRecordTrap()
    {
        ChangePhase(GamePhase.RecordTrap);
    }
    public void CompleteChooseSkill()
    {
        ChangePhase(GamePhase.Draw);
    }
    public void CompleteDraw()
    {
        ChangePhase(GamePhase.Move);
    }

    public void ChangePhaseToShowTrap()
    {
        ChangePhase(GamePhase.ShowTraps);
    }
    #endregion 
}
