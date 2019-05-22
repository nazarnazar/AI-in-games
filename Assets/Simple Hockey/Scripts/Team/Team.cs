using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Team : MonoBehaviour
{
    [SerializeField] private GoalKeeper _goalkeeper;
    [SerializeField] private FieldPlayer[] _homeFieldPlayers;
    [SerializeField] private Team _opponentTeam;
    [SerializeField] private Rink _rink;
    [SerializeField] private Goal _homeGoal;
    [SerializeField] private SupportSpotsController _supportSpotsController;
    [SerializeField] private HumanControlls _humanControlls;
    [SerializeField] private Animator _configsPanelAnimator;

    public bool AIControll = true;

    private Player _receivingPlayer;
    private Player _closestToBallPlayer;
    private Player _controllingPlayer;
    private List<Player> _supportingPlayers;
    private HockeyStateMachine _stateMachine;
    private bool _configsPanelIsOpen;

    private void Start()
    {
        _configsPanelIsOpen = false;
    }

    public void InitTeam()
    {
        _supportingPlayers = new List<Player>();
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            _homeFieldPlayers[i].InitFieldPlayer();
            int regionIndex = _homeFieldPlayers[i].GetHomeRegionIndex();
            _homeFieldPlayers[i].HomeRegion = _rink.GetRinkRegions()[regionIndex];
            int attackRegionIndex = _homeFieldPlayers[i].GetAttackRegionIndex();
            _homeFieldPlayers[i].AttackRegion = _rink.GetRinkRegions()[attackRegionIndex];
            _homeFieldPlayers[i].Team = this;
        }
        _goalkeeper.InitGoalkeeper();
        int index = _goalkeeper.GetHomeRegionIndex();
        _goalkeeper.HomeRegion = _rink.GetRinkRegions()[index];
        index = _goalkeeper.GetAttackRegionIndex();
        _goalkeeper.AttackRegion = _rink.GetRinkRegions()[index];
        _goalkeeper.Team = this;
	}

    public void StartPlaying()
    {
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            _homeFieldPlayers[i].StartPlaying();
        }
        _goalkeeper.StartPlaying();
        _stateMachine = new HockeyStateMachine(this, KickOff.Instance());
    }
	
    public void OpponentsScored(System.Action scored)
    {
        _homeGoal.SetGoalScoredAction(scored);
    }

    public void UpdateTeam()
    {
        _stateMachine.ExecuteSomeState();
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            _homeFieldPlayers[i].UpdateFieldPlayer();
        }
        _goalkeeper.UpdateGoalKeeper();

        _closestToBallPlayer = CalculateClosestToBallPlayer();
	}

    public void UpdatePlayerHomeRegion(Player player)
    {
        player.HomeRegion = _rink.GetRinkRegions()[player.GetHomeRegionIndex()];
    }

    public void UpdatePlayerAttackRegion(Player player)
    {
        player.AttackRegion = _rink.GetRinkRegions()[player.GetAttackRegionIndex()];
    }

    public Player[] GetPlayers()
    {
        return _homeFieldPlayers;
    }

    public Team GetOpponentTeam()
    {
        return _opponentTeam;
    }

    public void SetControllingPlayer(Player p)
    {
        _controllingPlayer = p;
    }

    public Player GetControllingPlayer()
    {
        return _controllingPlayer;
    }

    public void UnsetControllingPlayer(Player p)
    {
        if (_controllingPlayer != null && p == _controllingPlayer)
        {
            _controllingPlayer = null;
        }
    }

    public void SetReceivingPlayer(Player p)
    {
        _receivingPlayer = p;
    }

    public Player GetReceivingPlayer()
    {
        return _receivingPlayer;
    }

    public void SetClosestToBallPlayer(Player p)
    {
        _closestToBallPlayer = p;
    }

    public Player GetClosestToBallPlayer()
    {
        return _closestToBallPlayer;
    }

    public void AddSupportingPlayer(Player p, SupportSpot ss)
    {
        p.SupportSpot = ss;
        _supportingPlayers.Add(p);
    }

    public void RemoveSupportingPlayer(Player p)
    {
        p.SupportSpot = null;
        _supportingPlayers.Remove(p);
    }

    public void NullSupportingPlayers()
    {
        foreach (var p in _supportingPlayers)
            p.SupportSpot = null;
        _supportingPlayers = new List<Player>();
    }

    public List<Player> GetSupportingPlayers()
    {
        return _supportingPlayers;
    }

    public FieldPlayer GetPlayerToPass(Player p)
    {
        if (_controllingPlayer != null)
        {
            for (int i = 0; i < _homeFieldPlayers.Length; i++)
            {
                if (_homeFieldPlayers[i] != p && _homeFieldPlayers[i].IsAheadOfAttacker() &&
                    IsPassSafe(p.GetPosition(), _homeFieldPlayers[i].GetPosition()))
                {
                    return _homeFieldPlayers[i];
                }
            }
        }

        if (_supportingPlayers.Count > 0)
        {
            for (int i = 0; i < _homeFieldPlayers.Length; i++)
            {
                foreach (var supportPlayer in _supportingPlayers)
                {
                    if (_homeFieldPlayers[i] != p && _homeFieldPlayers[i] == supportPlayer &&
                        IsPassSafe(p.GetPosition(), _homeFieldPlayers[i].GetPosition()))
                    {
                        return _homeFieldPlayers[i];
                    }
                }
            }
        }

        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            if (_homeFieldPlayers[i] != p &&
                IsPassSafe(p.GetPosition(), _homeFieldPlayers[i].GetPosition()))
            {
                return _homeFieldPlayers[i];
            }
        }

        return null;
    }

    public FieldPlayer GetClosestPlayerToDirection(Player p, Vector2 direction)
    {
        float minAngle = 180.0f;
        FieldPlayer minPlayer = null;
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            if (_homeFieldPlayers[i] != p)
            {
                Vector2 toAnotherPlayer = _homeFieldPlayers[i].GetPosition() - p.GetPosition();
                toAnotherPlayer.Normalize();
                if (minAngle > Vector2.Angle(direction, toAnotherPlayer))
                {
                    minAngle = Vector2.Angle(direction, toAnotherPlayer);
                    minPlayer = _homeFieldPlayers[i];
                }
            }
        }
        return minPlayer;
    }

    public void GoBackHome(bool kickOff)
    {
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            _homeFieldPlayers[i].ChangeState(ReturnHome.Instance());
        }
        if (kickOff)
        {
            _goalkeeper.ChangeState(GoalkeeperReturnHome.Instance());
        }
    }

    public void GoToAttack()
    {
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            _homeFieldPlayers[i].ChangeState(GoAttack.Instance());
        }
    }

    public bool AreEverybodyHome()
    {
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            if (_homeFieldPlayers[i].IsAtTarget())
            {
                if (_homeFieldPlayers[i].CurrentState() != Wait.Instance())
                {
                    _homeFieldPlayers[i].ChangeState(Wait.Instance());
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void ChangeState(IHockeyState newState)
    {
        _stateMachine.ChangeState(newState);
    }

    public bool InControl()
    {
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            if (_homeFieldPlayers[i] == GetPuck().GetLastPlayerKickedPuck())
            {
                return true;
            }
        }
        return false;
    }

    public SupportSpot DetermineBestSupportingPosition()
    {
        return _supportSpotsController.GetBestSupportingSpot(_controllingPlayer, _supportingPlayers);
    }

    public Puck GetPuck()
    {
        return _rink.GetPuck();
    }

    public IHockeyState GetCurrentState()
    {
        return _stateMachine.GetCurrentState();
    }

    public Goal GetHomeGoal()
    {
        return _homeGoal;
    }

    public Rink GetRink()
    {
        return _rink;
    }

    public Vector2[] GetPlayersPos()
    {
        Vector2[] pos = new Vector2[_homeFieldPlayers.Length];
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            pos[i] = _homeFieldPlayers[i].GetPosition();
        }
        return pos;
    }

    public bool IsOpponentInRadius(Vector2 pos, float radius)
    {
        Vector2[] opponentsPos = _opponentTeam.GetPlayersPos();
        for (int i = 0; i < opponentsPos.Length; i++)
        {
            if ((pos - opponentsPos[i]).magnitude < radius)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanShoot(Vector2 playerPos, float force)
    {
        return Vector2.Distance(playerPos, GetOpponentTeam().GetHomeGoal().GetPosition()) < _rink.GetRinkLength() / 4.0f;
    }

    public bool IsPassSafe(Vector2 playerWithBallPos, Vector2 targetPos)
    {
        const int trajectoryPrecision = 5;
        Vector2[] opponentsPos = _opponentTeam.GetPlayersPos();
        Vector2 trajectory = targetPos - playerWithBallPos;
        for (int i = 0; i < opponentsPos.Length; i++)
        {
            for (int j = 0; j < trajectoryPrecision; j++)
            {
                Vector2 point = Vector2.Lerp(playerWithBallPos, targetPos, (float)j / (float)trajectoryPrecision);
                if (Vector2.Distance(playerWithBallPos, point) > Vector2.Distance(opponentsPos[i], point) &&
                    Vector2.Distance(targetPos, point) > Vector2.Distance(opponentsPos[i], point))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void ChangeTeamColor(Color newColor)
    {
        for (int i = 0; i < _homeFieldPlayers.Length; i++)
        {
            _homeFieldPlayers[i].ChangeColor(newColor, false);
        }
        _goalkeeper.ChangeColor(newColor, false);
    }

    private Player CalculateClosestToBallPlayer()
    {
        float currClosestDist = Vector2.Distance(_homeFieldPlayers[0].GetPosition(), GetPuck().GetPosition());
        Player currClosestPlayer = _homeFieldPlayers[0];
        for (int i = 1; i < _homeFieldPlayers.Length; i++)
        {
            float tempClosestDist = Vector2.Distance(_homeFieldPlayers[i].GetPosition(), GetPuck().GetPosition());
            if (tempClosestDist < currClosestDist)
            {
                currClosestDist = tempClosestDist;
                currClosestPlayer = _homeFieldPlayers[i];
            }
        }
        return currClosestPlayer;
    }

    public void UseAIControll(Image buttonImage)
    {
        AIControll = !AIControll;
        if (AIControll)
        {
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = Color.gray;
        }
    }

    public HumanControlls GetHumanControlls()
    {
        return _humanControlls;
    }

    public void OpenCloseConfigs(RectTransform arrow)
    {
        _configsPanelIsOpen = !_configsPanelIsOpen;
        _configsPanelAnimator.SetBool("IsOpen", _configsPanelIsOpen);
        if (_configsPanelIsOpen)
        {
            arrow.localEulerAngles = new Vector3(0.0f, 0.0f, -180.0f);
            _rink.ShowRegions();
        }
        else
        {
            arrow.localEulerAngles = Vector3.zero;
            _rink.HideRegions();
        }
    }
}