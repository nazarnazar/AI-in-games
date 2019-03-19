using UnityEngine;

public class Attack : IHockeyState
{
    private static Attack _attack;

    public static Attack Instance()
    {
        if (_attack == null)
        {
            _attack = new Attack();
        }
        return _attack;
    }

    public void Enter(Team team)
    {
        team.GoToAttack();
    }

    public void Execute(Team team)
    {
        if (!team.InControl())
        {
            team.ChangeState(Defence.Instance());
        }
    }

    public void Exit(Team team)
    {
    }
}