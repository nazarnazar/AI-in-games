using UnityEngine;

public class KickPuck : IFieldPlayerState
{
    private static KickPuck _kickPuck;

    public static KickPuck Instance()
    {
        if (_kickPuck == null)
        {
            _kickPuck = new KickPuck();
        }
        return _kickPuck;
    }

    public void Enter(FieldPlayer player)
    {
        player.ChangeColor(Color.red);

        player.Team.SetControllingPlayer(player);
        if (!player.IsReadyForNextKick())
        {
            player.ChangeState(ChasePuck.Instance());
        }
    }

    public void Execute(FieldPlayer player)
    {
        Vector2 toBall = player.GetPuck().GetPosition() - player.GetPosition();
        float dot = Vector2.Dot(player.Heading().normalized, toBall.normalized);
        if (player.Team.GetReceivingPlayer() != null || dot < 0.0f)
        {
            player.ChangeState(ChasePuck.Instance());
            return;
        }

        float kickPower = MatchData.Instance().MaxShootForce;
        Vector2 ballTarget = player.Team.GetOpponentTeam().GetHomeGoal().GetPosition();
        if (player.Team.CanShoot(player.GetPosition(), kickPower))
        {
            ballTarget = AddNoiseToKick(player.GetPuck().GetPosition(), ballTarget);
            Vector2 kickDirection = ballTarget - player.GetPuck().GetPosition();

            player.GetPuck().Trap();
            player.GetPuck().Kick(kickDirection, kickPower, player);
            return;
        }

        FieldPlayer receiver = player.Team.GetPlayerToPass(player);
        kickPower = Mathf.Clamp(MatchData.Instance().MaxPassForce * (Vector2.Distance(receiver.GetPosition(), player.GetPuck().GetPosition()) / (player.Team.GetRink().GetRinkLength() / 2.0f)),
            MatchData.Instance().MinPassForce,
            MatchData.Instance().MaxPassForce);
        if (player.IsThreatened() &&
            player.Team.CanPass(player, receiver, kickPower, MatchData.Instance().MinPassDistance))
        {
            ballTarget = receiver.GetPosition() + receiver.GetCurrentVelocity() * MatchData.Instance().PassAheadValue;
            ballTarget = AddNoiseToKick(player.GetPuck().GetPosition(), ballTarget);
            Vector2 kickDirection = ballTarget - player.GetPuck().GetPosition();

            player.GetPuck().Trap();
            player.GetPuck().Kick(kickDirection, kickPower, player);
            receiver.SendMessage(MessageType.RecieveBall);
            player.SendMessage(MessageType.SupportAttacker);
            return;
        }

        player.ChangeState(Dribble.Instance());
    }

    public void Exit(FieldPlayer player)
    {
        player.ChangeColor(Color.white);

        player.Team.UnsetControllingPlayer(player);
    }

    private Vector2 AddNoiseToKick(Vector2 ballPosition, Vector2 ballTarget)
    {
        float distanceToGoal = Vector2.Distance(ballPosition, ballTarget);
        float noise = distanceToGoal * MatchData.Instance().KickNoise;
        float xNoise = (Random.Range(0, 2) == 1) ? noise : -noise;
        float yNoise = (Random.Range(0, 2) == 1) ? noise : -noise;
        return new Vector2(ballTarget.x + xNoise, ballTarget.y + yNoise);
    }
}