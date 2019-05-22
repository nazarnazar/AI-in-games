
public class FieldPlayerStateMachine
{
    private FieldPlayer _player;
    private IFieldPlayerState _currentState;
    private GlobalPlayerState _globalState;

    public FieldPlayerStateMachine(FieldPlayer player, IFieldPlayerState initialState)
    {
        _player = player;
        _globalState = GlobalPlayerState.Instance();
        _globalState.Enter(_player);
        if (_currentState != null)
        {
            _currentState.Exit(_player);
        }
        _currentState = initialState;
        if (_currentState != null)
        {
            _currentState.Enter(_player);
        }
    }

    public void ExecuteSomeState()
    {
        _globalState.Execute(_player);
        if (_currentState != null)
        {
            _currentState.Execute(_player);
        }
    }

    public void ChangeState(IFieldPlayerState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(_player);
        }
        _currentState = newState;
        _currentState.Enter(_player);
    }

    public IFieldPlayerState GetCurrentState()
    {
        return _currentState;
    }

    public void NewMessage(FieldPlayer player, MessageType messageType)
    {
        _globalState.NewMessage(player, messageType);
    }
}