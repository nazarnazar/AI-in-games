using UnityEngine;

public class KickOff : IHockeyState
{
    private static KickOff _kickOff;

    public static KickOff Instance()
    {
        if (_kickOff == null)
        {
            _kickOff = new KickOff();
        }
        return _kickOff;
    }

    public void Enter(Team team)
    {
        team.SetClosestToBallPlayer(null);
        team.SetReceivingPlayer(null);
        team.SetControllingPlayer(null);
        team.NullSupportingPlayers();

        team.GoBackHome();
    }

    public void Execute(Team team)
    {
        if (team.AreEverybodyHome() && team.GetOpponentTeam().AreEverybodyHome())
        {
            team.ChangeState(Defence.Instance());
        }
    }

    public void Exit(Team team)
    {
    }
}