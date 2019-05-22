using UnityEngine;

public class ReturnHome : IFieldPlayerState
{
    private static ReturnHome _goHome;

    public static ReturnHome Instance()
    {
        if (_goHome == null)
        {
            _goHome = new ReturnHome();
        }
        return _goHome;
    }

    public void Enter(FieldPlayer player)
    {
        player.ChangeColor(Color.grey);

        player.MovingAgent.SetTarget(player.HomeRegion.Center);
        player.MovingAgent.AddSteeringType(SteeringType.Arrive);
    }

    public void Execute(FieldPlayer player)
    {
        if (player.IsAtTarget())
        {
            player.ChangeState(Wait.Instance());
            return;
        }

        if (player.Team.GetRink().IsGameOn())
        {
            if (player.IsClosestToBall() && player.Team.GetReceivingPlayer() == null && !player.Team.GetRink().GoalKeeperHasBall())
            {
                player.ChangeState(ChasePuck.Instance());
            }
        }
    }

    public void Exit(FieldPlayer player)
    {
        player.ChangeColor(Color.white);

        player.MovingAgent.RemoveSteeringType(SteeringType.Arrive);
    }
}