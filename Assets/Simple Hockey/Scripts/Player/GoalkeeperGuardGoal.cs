using UnityEngine;

public class GoalkeeperGuardGoal : IGoalkeeperState
{
    private static GoalkeeperGuardGoal _guardGoal;

    public static GoalkeeperGuardGoal Instance()
    {
        if (_guardGoal == null)
        {
            _guardGoal = new GoalkeeperGuardGoal();
        }
        return _guardGoal;
    }

    public void Enter(GoalKeeper player)
    {
        player.ChangeColor(Color.black);

        player.TrackPuck();
        player.SetCurrentVelocity(Vector2.zero);
    }

    public void Execute(GoalKeeper player)
    {
        
    }

    public void Exit(GoalKeeper player)
    {
        player.ChangeColor(Color.white);
    }
}