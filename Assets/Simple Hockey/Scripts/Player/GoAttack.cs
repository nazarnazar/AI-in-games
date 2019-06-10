using UnityEngine;

public class GoAttack : IFieldPlayerState
{
    private static GoAttack _goAttack;

    public static GoAttack Instance()
    {
        if (_goAttack == null)
        {
            _goAttack = new GoAttack();
        }
        return _goAttack;
    }

    public void Enter(FieldPlayer player)
    {
        player.ChangeColor(Color.cyan);

        player.MovingAgent.SetTarget(player.AttackRegion.Center);
        player.MovingAgent.AddSteeringType(SteeringType.Arrive);
    }

    public void Execute(FieldPlayer player)
    {
        if (player.IsAtTarget())
        {
            player.SendMessage(MessageType.SupportAttacker);
            return;
        }

        if (player.Team.GetRink().IsGameOn())
        {
            if (player.IsClosestToPuck() && player.Team.GetReceivingPlayer() == null && !player.Team.GetRink().GoalKeeperHasPuck())
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