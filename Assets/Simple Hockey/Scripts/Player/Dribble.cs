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
        Vector2 facing = player.Team.GetHomeGoal().Facing();
        float noise = MatchData.Instance().KickNoiseWhileDribble;
        float force = MatchData.Instance().KickForceWhileDribble + Random.Range(-noise, noise);
        player.GetPuck().Kick(facing, force, player);

        player.ChangeState(ChasePuck.Instance());
    }

    public void Exit(FieldPlayer player)
    {
        player.ChangeColor(Color.white);

        player.Team.UnsetControllingPlayer(player);
    }
}