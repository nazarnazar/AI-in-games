
public class GoalKeeper : Player
{
    private GoalkeeperStateMachine _goalkeeperStateMachine;

    public void InitGoalkeeper()
    {
        base.ConnectMovingAgent();
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
}