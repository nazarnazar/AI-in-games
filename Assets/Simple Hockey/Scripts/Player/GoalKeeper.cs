using UnityEngine;

public class GoalKeeper : Player
{
    [SerializeField] private MovingAgent _goalPositioner;

    private GoalkeeperStateMachine _goalkeeperStateMachine;

    private const float _goalkeeperForceKoef = 10.0f;

    public void InitGoalkeeper()
    {
        base.InitPlayer();
        MovingAgent.MaxForce *= _goalkeeperForceKoef;
    }

    public void StartPlaying()
    {
        _goalkeeperStateMachine = new GoalkeeperStateMachine(this, null);
    }

    public void UpdateGoalKeeper()
    {
        _goalkeeperStateMachine.ExecuteSomeState();
    }

    public IGoalkeeperState CurrentState()
    {
        return _goalkeeperStateMachine.GetCurrentState();
    }

    public void ChangeState(IGoalkeeperState newState)
    {
        _goalkeeperStateMachine.ChangeState(newState);
    }

    public void SendMessage(MessageType messageType)
    {
        _goalkeeperStateMachine.NewMessage(this, messageType);
    }

    public MovingAgent GetGoalPositioner()
    {
        return _goalPositioner;
    }
}