using UnityEngine;

public class ChasePuck : IFieldPlayerState
{
    private static ChasePuck _chasePuck;

    public static ChasePuck Instance()
    {
        if (_chasePuck == null)
        {
            _chasePuck = new ChasePuck();
        }
        return _chasePuck;
    }

    public void Enter(FieldPlayer player)
    {
        player.ChangeColor(Color.yellow);

        player.MovingAgent.Evader = player.GetPuck().GetPuckMovingAgent();
        player.MovingAgent.AddSteeringType(SteeringType.Pursuit);
    }

    public void Execute(FieldPlayer player)
    {
        if (player.CanKickThePuck())
        {
            player.ChangeState(KickPuck.Instance());
            return;
        }

        if (player.IsClosestToPuck())
        {
            return;
        }
            
        if (player.Team.InControl())
        {
            player.ChangeState(GoAttack.Instance());
        }
        else
        {
            player.ChangeState(ReturnHome.Instance());
        }
    }

    public void Exit(FieldPlayer player)
    {
        player.ChangeColor(Color.white);

        player.MovingAgent.RemoveSteeringType(SteeringType.Pursuit);
    }
}