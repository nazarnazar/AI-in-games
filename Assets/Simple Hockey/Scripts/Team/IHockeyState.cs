
public interface IHockeyState
{
    void Enter(Team team);
    void Execute(Team team);
    void Exit(Team team);
}