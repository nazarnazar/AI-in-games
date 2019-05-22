using UnityEngine;

public class Defence : IHockeyState
{
    private static Defence _defence;

    public static Defence Instance()
    {
        if (_defence == null)
        {
            _defence = new Defence();
        }
        return _defence;
    }

    public void Enter(Team team)
    {
        if (!team.AreEverybodyHome())
        {
            team.GoBackHome(false);
        }
    }

    public void Execute(Team team)
    {
        if (team.InControl())
        {
            team.ChangeState(Attack.Instance());
        }
    }

    public void Exit(Team team)
    {
    }
}