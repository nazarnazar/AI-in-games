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
            return;
        }
        else if (player.Team.GetCurrentState() == Attack.Instance() &&
            Vector2.Distance(player.GetPosition(), player.AttackRegion.Center) > MatchData.Instance().E)
        {
            player.ChangeState(GoAttack.Instance());
            return;
        }
        else if (player.Team.GetCurrentState() == KickOff.Instance() &&
            Vector2.Distance(player.GetPosition(), player.HomeRegion.Center) > MatchData.Instance().E)
        {
            player.ChangeState(ReturnHome.Instance());
            return;
        }
        else
        {
            player.MovingAgent.Stop();
            player.TrackPuck();
        }

        if (player.Team.GetRink().IsGameOn())
        {
            if (player.IsClosestToPuck() && player.Team.GetReceivingPlayer() == null && !player.Team.GetRink().GoalKeeperHasPuck())
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