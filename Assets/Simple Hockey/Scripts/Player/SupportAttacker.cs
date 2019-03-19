using UnityEngine;

public class SupportAttacker : IFieldPlayerState
{
    private static SupportAttacker _supportAttacker;

    public static SupportAttacker Instance()
    {
        if (_supportAttacker == null)
        {
            _supportAttacker = new SupportAttacker();
        }
        return _supportAttacker;
    }

    public void Enter(FieldPlayer player)
    {
        player.ChangeColor(new Color(1.0f, 0.5f, 0.0f));

        SupportSpot supportSpot = player.Team.DetermineBestSupportingPosition();
        player.Team.AddSupportingPlayer(player, supportSpot);
        player.MovingAgent.SetTarget(supportSpot.Position);
        player.MovingAgent.AddSteeringType(SteeringType.Arrive);
    }

    public void Execute(FieldPlayer player)
    {
        if (player.Team.CanShoot(player.GetPosition(), MatchData.Instance().MaxShootForce))
        {
            player.SendMessage(MessageType.PassToMe);
        }

        if (player.IsAtTarget())
        {
            player.TrackPuck();
            player.SetCurrentVelocity(Vector2.zero);
            if (!player.IsThreatened())
            {
                player.SendMessage(MessageType.PassToMe);
            }
        }
    }

    public void Exit(FieldPlayer player)
    {
        player.ChangeColor(Color.white);

        player.Team.RemoveSupportingPlayer(player);
        player.MovingAgent.RemoveSteeringType(SteeringType.Arrive);
    }
}