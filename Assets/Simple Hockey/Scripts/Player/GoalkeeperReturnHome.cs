using UnityEngine;

public class GoalkeeperReturnHome : IGoalkeeperState
{
    private static GoalkeeperReturnHome _goHome;

    public static GoalkeeperReturnHome Instance()
    {
        if (_goHome == null)
        {
            _goHome = new GoalkeeperReturnHome();
        }
        return _goHome;
    }

    public void Enter(GoalKeeper player)
    {
        player.ChangeColor(Color.grey);

        player.MovingAgent.SetTarget(player.HomeRegion.Center);
        player.MovingAgent.AddSteeringType(SteeringType.Arrive);
    }

    public void Execute(GoalKeeper player)
    {
        if (player.IsAtTarget())
        {
            player.ChangeState(GoalkeeperGuardGoal.Instance());
        }
    }

    public void Exit(GoalKeeper player)
    {
        player.ChangeColor(Color.white);

        player.MovingAgent.RemoveSteeringType(SteeringType.Arrive);
    }
}