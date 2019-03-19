﻿using UnityEngine;

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
        if (Vector2.Distance(player.GetPosition(), player.AttackRegion.Center) < MatchData.Instance().E)
        {
            player.SendMessage(MessageType.SupportAttacker);
        }

        if (player.Team.GetRink().IsGameOn())
        {
            if (player.IsClosestToBall() && player.Team.GetReceivingPlayer() == null && !player.Team.GetRink().GoalKeeperHasBall())
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