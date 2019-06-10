using UnityEngine;

public class Dribble : IFieldPlayerState
{
    private static Dribble _dribble;

    public static Dribble Instance()
    {
        if (_dribble == null)
        {
            _dribble = new Dribble();
        }
        return _dribble;
    }

    public void Enter(FieldPlayer player)
    {
        player.ChangeColor(Color.magenta);

        player.Team.SetControllingPlayer(player);
    }

    public void Execute(FieldPlayer player)
    {
        Vector2 facing = player.Team.AIControll ? AIDribbleDirection(player) : HumanDribbleDirection(player);
        float noise = MatchData.Instance().KickNoiseWhileDribble;
        float force = MatchData.Instance().KickForceWhileDribble + Random.Range(-noise, noise);

        if (player.GetPuck().CanKick(player) && Vector2.Dot(facing, player.GetPuck().GetPuckMovingAgent().GetAgentVelocity()) < 0.0f)
        {
            player.GetPuck().Trap();
        }

        player.GetPuck().Kick(facing, force, player);

        player.ChangeState(ChasePuck.Instance());
    }

    public void Exit(FieldPlayer player)
    {
        player.ChangeColor(Color.white);

        player.Team.UnsetControllingPlayer(player);
    }

    private Vector2 AIDribbleDirection(FieldPlayer player)
    {
        if (Random.Range(0.0f, 1.0f) > 0.25f)
        {
            return (player.Team.GetOpponentTeam().GetHomeGoal().GetPosition() - player.GetPosition());
        }
        else
        {
            return player.Team.GetHomeGoal().Facing();
        }
    }

    private Vector2 HumanDribbleDirection(FieldPlayer player)
    {
        return player.Team.GetHumanControlls().GetDirection();
    }
}