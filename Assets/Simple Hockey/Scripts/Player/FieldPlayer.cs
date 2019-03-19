
public class FieldPlayer : Player
{
    private FieldPlayerStateMachine _fieldPlayerStateMachine;

    public void InitFieldPlayer()
    {
        base.ConnectMovingAgent();
        _fieldPlayerStateMachine = new FieldPlayerStateMachine(this, null);
    }

    public void UpdateFieldPlayer()
    {
        _fieldPlayerStateMachine.ExecuteSomeState();
    }

    public IFieldPlayerState CurrentState()
    {
        return _fieldPlayerStateMachine.GetCurrentState();
    }

    public void ChangeState(IFieldPlayerState newState)
    {
        _fieldPlayerStateMachine.ChangeState(newState);
    }

    public void SendMessage(MessageType messageType)
    {
        _fieldPlayerStateMachine.NewMessage(this, messageType);
    }
}