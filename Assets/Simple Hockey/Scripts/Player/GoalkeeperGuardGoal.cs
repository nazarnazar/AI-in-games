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

        player.MovingAgent.Evader = player.GetGoalPositioner();
        player.MovingAgent.Pursuer = player.GetPuck().GetPuckMovingAgent();
        player.MovingAgent.AddSteeringType(SteeringType.Interpose);
    }

    public void Execute(GoalKeeper player)
    {
        if (player.GetPuck().GetPuckMovingAgent().GetAgentVelocity().sqrMagnitude < MatchData.Instance().E)
        {
            player.MovingAgent.Stop();
        }

        Vector2 toPuck = player.MovingAgent.Pursuer.GetAgentPosition() - player.MovingAgent.Evader.GetAgentPosition();
        float angle = Mathf.Atan2(toPuck.y, toPuck.x) * Mathf.Rad2Deg;
        player.MovingAgent.SetAgentRotation(angle);
    }

    public void Exit(GoalKeeper player)
    {
        player.ChangeColor(Color.white);

        player.MovingAgent.RemoveSteeringType(SteeringType.Interpose);
    }
}