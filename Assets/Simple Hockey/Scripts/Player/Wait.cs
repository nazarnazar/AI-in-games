using UnityEngine;

public class Wait : IFieldPlayerState
{
    private static Wait _wait;

    public static Wait Instance()
    {
        if (_wait == null)
        {
            _wait = new Wait();
        }
        return _wait;
    }

    public void Enter(FieldPlayer player)
    {
        player.ChangeColor(Color.green);
    }

    public void Execute(FieldPlayer player)
    {
        if (player.Team.GetCurrentState() == Defence.Instance() &&
            Vector2.Distance(player.GetPosition(), player.HomeRegion.Center) > MatchData.Instance().E)
        {
            player.ChangeState(ReturnHome.Instance());
        }
        else if (player.Team.GetCurrentState() == Attack.Instance() &&
            Vector2.Distance(player.GetPosition(), player.AttackRegion.Center) > MatchData.Instance().E)
        {
            player.ChangeState(GoAttack.Instance());
        }
        else
        {
            player.SetCurrentVelocity(Vector2.zero);
            player.TrackPuck();
        }

        if (player.Team.GetRink().IsGameOn())
        {
            if (player.IsClosestToBall() && player.Team.GetReceivingPlayer() == null && !player.Team.GetRink().GoalKeeperHasBall())
            {
                player.ChangeState(ChasePuck.Instance());
                return;
            }
            if (player.Team.InControl() && !player.IsInControl() && player.IsAheadOfAttacker())
            {
                player.SendMessage(MessageType.PassToMe);
                return;
            }
        }
    }

    public void Exit(FieldPlayer player)
    {
        player.ChangeColor(Color.white);
    }
}