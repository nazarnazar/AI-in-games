using UnityEngine;

public class RecievePuck : IFieldPlayerState
{
    private static RecievePuck _recievePuck;

    public static RecievePuck Instance()
    {
        if (_recievePuck == null)
        {
            _recievePuck = new RecievePuck();
        }
        return _recievePuck;
    }

    public void Enter(FieldPlayer player)
    {
        player.ChangeColor(Color.blue);

        player.Team.SetReceivingPlayer(player);
        player.Team.SetControllingPlayer(player);

        if ((player.IsInHotRegion()) &&
            !player.Team.IsOpponentInRadius(player.GetPosition(), MatchData.Instance().PassIntersectionRadius))
        {
            player.MovingAgent.SetTarget(player.GetPuck().GetPosition());
            player.MovingAgent.AddSteeringType(SteeringType.Arrive);
        }
        else
        {
            player.MovingAgent.SetTarget(player.GetPuck().GetPosition());
            player.MovingAgent.Evader = player.GetPuck().GetPuckMovingAgent();
            player.MovingAgent.AddSteeringType(SteeringType.Pursuit);
        }
    }

    public void Execute(FieldPlayer player)
    {
        if (!player.Team.IsPassSafe(player.GetPuck().GetPosition(), player.GetPosition(), MatchData.Instance().MaxPassForce) ||
            !player.Team.InControl())
        {
            player.ChangeState(ChasePuck.Instance());
            return;
        }

        player.MovingAgent.SetTarget(player.GetPuck().GetPosition());

        if (player.CanKickThePuck())
        {
            player.ChangeState(KickPuck.Instance());
            return;
        }

        if (player.IsAtTarget())
        {
            player.ChangeState(Wait.Instance());
        }
    }

    public void Exit(FieldPlayer player)
    {
        player.ChangeColor(Color.white);

        player.Team.SetReceivingPlayer(null);
        player.Team.UnsetControllingPlayer(player);

        player.MovingAgent.RemoveSteeringType(SteeringType.Arrive);
        player.MovingAgent.RemoveSteeringType(SteeringType.Pursuit);
    }
}