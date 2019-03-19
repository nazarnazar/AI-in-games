public interface IGoalkeeperState
{
    void Enter(GoalKeeper player);
    void Execute(GoalKeeper player);
    void Exit(GoalKeeper player);
}