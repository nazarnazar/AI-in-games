
public class GoalkeeperStateMachine
{
    private GoalKeeper _player;
    private IGoalkeeperState _currentState;
    private GlobalGoalkeeperState _globalState;

    public GoalkeeperStateMachine(GoalKeeper player, IGoalkeeperState initialState)
    {
        _player = player;
        _globalState = GlobalGoalkeeperState.Instance();
        _currentState = initialState;
        _globalState.Enter(_player);
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

    public void ChangeState(IGoalkeeperState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(_player);
        }
        _currentState = newState;
        _currentState.Enter(_player);
    }

    public IGoalkeeperState GetCurrentState()
    {
        return _currentState;
    }

    public void NewMessage(GoalKeeper player, MessageType messageType)
    {
        _globalState.NewMessage(player, messageType);
    }
}