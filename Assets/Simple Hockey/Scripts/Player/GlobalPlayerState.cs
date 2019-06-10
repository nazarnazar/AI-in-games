using UnityEngine;

public class GlobalPlayerState : IFieldPlayerState
{
    private static GlobalPlayerState _globalPlayerState;

    public static GlobalPlayerState Instance()
    {
        if (_globalPlayerState == null)
        {
            _globalPlayerState = new GlobalPlayerState();
        }
        return _globalPlayerState;
    }

    public void Enter(FieldPlayer player)
    {
    }

    public void Execute(FieldPlayer player)
    {
        if (player.Team.GetControllingPlayer() != null &&
            player.Team.GetControllingPlayer() == player)
        {
            player.SetMaxSpeed(MatchData.Instance().PlayerMaxSpeedWithPuck);
        }
        else
        {
            player.SetMaxSpeed(MatchData.Instance().PlayerMaxSpeed);
        }

        if (player.CurrentState() == Wait.Instance())
        {
            player.StopMovingAnimation();
        }
        else
        {
            player.StartMovingAnimation();
        }
    }

    public void Exit(FieldPlayer player)
    {
    }

    public bool NewMessage(FieldPlayer player, MessageType messageType)
    {
        switch (messageType)
        {
            case MessageType.SupportAttacker:

                if (player.CurrentState() == SupportAttacker.Instance() ||
                    player.Team.GetControllingPlayer() == null)
                {
                    return true;
                }
                player.ChangeState(SupportAttacker.Instance());
                return true;

            case MessageType.GoHome:

                player.ChangeState(ReturnHome.Instance());
                return true;

            case MessageType.RecieveBall:
                
                player.ChangeState(RecievePuck.Instance());
                return true;

            case MessageType.PassToMe:

                if (!player.Team.AIControll)
                {
                    return true;
                }

                FieldPlayer controllingPlayer = (FieldPlayer)player.Team.GetControllingPlayer();
                if (controllingPlayer == null)
                {
                    return true;
                }
                if (!controllingPlayer.CanKickThePuck())
                {
                    return true;
                }

                if (MatchData.Instance().IsDebugMode)
                {
                    Debug.DrawLine(player.GetPosition(), player.GetPuck().GetPosition(), Color.magenta);
                    Debug.Break();
                }

                player.GetPuck().Trap();
                if (!controllingPlayer.GetPuck().Kick(player.GetPosition() - player.GetPuck().GetPosition(), MatchData.Instance().MaxPassForce, controllingPlayer))
                {
                    return true;
                }
                player.SendMessage(MessageType.RecieveBall);
                controllingPlayer.SendMessage(MessageType.SupportAttacker);
                return true;

            case MessageType.Wait:

                player.ChangeState(Wait.Instance());
                return true;
        }

        return false;
    }
}