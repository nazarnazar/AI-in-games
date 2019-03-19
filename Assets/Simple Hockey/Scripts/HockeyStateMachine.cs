
public class HockeyStateMachine
{
    private Team _team;
    private IHockeyState _currentState;

    public HockeyStateMachine(Team team, IHockeyState initialState)
    {
        _team = team;
        _currentState = initialState;
        _currentState.Enter(_team);
    }

    public void ExecuteSomeState()
    {
        _currentState.Execute(_team);
    }

    public void ChangeState(IHockeyState newState)
    {
        _currentState.Exit(_team);
        _currentState = newState;
        _currentState.Enter(_team);
    }

    public IHockeyState GetCurrentState()
    {
        return _currentState;
    }
}