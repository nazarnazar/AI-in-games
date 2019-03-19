using UnityEngine;

public class GlobalGoalkeeperState : IGoalkeeperState
{
    private static GlobalGoalkeeperState _globalGoalkeeperState;

    public static GlobalGoalkeeperState Instance()
    {
        if (_globalGoalkeeperState == null)
        {
            _globalGoalkeeperState = new GlobalGoalkeeperState();
        }
        return _globalGoalkeeperState;
    }

    public void Enter(GoalKeeper player)
    {
        player.SetMaxSpeed(MatchData.Instance().PlayerMaxSpeed);
    }

    public void Execute(GoalKeeper player)
    {
        if (player.CurrentState() == GoalkeeperGuardGoal.Instance())
        {
            player.StopMovingAnimation();
        }
        else
        {
            player.StartMovingAnimation();
        }
    }

    public void Exit(GoalKeeper player)
    {
    }

    public bool NewMessage(GoalKeeper player, MessageType messageType)
    {
        switch (messageType)
        {
            
        }

        return false;
    }
}