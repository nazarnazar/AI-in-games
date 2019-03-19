using UnityEngine;
using System.Collections.Generic;

public class SupportSpotsController : MonoBehaviour
{
    [SerializeField] private Vector2 _spotsTopLeft;
    [SerializeField] private Vector2 _spotsBottomRight;
    [SerializeField] private int _spotsRowsNumber;
    [SerializeField] private int _spotsColumnsNumber;
    [SerializeField] private GameObject _spotPref;
    [SerializeField] private Team _team;

    private Region _spotsRegion;
    private SupportSpot[] _supportSpots;
    private SupportSpot _bestSupportSpot;
    private Player[] _homePlayers;
    private Player[] _opponentPlayers;

	private void Start()
    {
        _spotsRegion = new Region(_spotsTopLeft, _spotsBottomRight);
        _supportSpots = new SupportSpot[_spotsRowsNumber * _spotsColumnsNumber];
        float columnStep = _spotsRegion.GetWidth() / (_spotsColumnsNumber - 1);
        float rowStep = _spotsRegion.GetHeight() / (_spotsRowsNumber - 1);
        GameObject tempSpot;
        int spotIndex;
        for (int i = 0; i < _spotsRowsNumber; i++)
        {
            for (int j = 0; j < _spotsColumnsNumber; j++)
            {
                tempSpot = Instantiate(_spotPref, transform);
                spotIndex = i * _spotsColumnsNumber + j;
                _supportSpots[spotIndex] = tempSpot.GetComponent<SupportSpot>();
                _supportSpots[spotIndex].Position = new Vector2(_spotsRegion.TopLeft.x + columnStep * j,
                    _spotsRegion.TopLeft.y - rowStep * i);
                _supportSpots[spotIndex].Score = 1.0f;
            }
        }

        _homePlayers = _team.GetPlayers();
        _opponentPlayers = _team.GetOpponentTeam().GetPlayers();
	}

    public SupportSpot GetBestSupportingSpot(Player controllingPlayer, List<Player> supportingPlayers)
    {
        // Debug.Log("GetBestSupportingSpot");

        _bestSupportSpot = null;
        SupportSpot tempSpot;
        float bestScore = 0.0f;
        for (int i = 0; i < _supportSpots.Length; i++)
        {
            tempSpot = _supportSpots[i];
            tempSpot.Score = 1.0f;
            // Debug.LogFormat("tempSpot{0}: {1}", i, tempSpot.Position);

            if (_team.IsPassSafe(controllingPlayer.GetPosition(), tempSpot.Position, MatchData.Instance().MaxPassForce))
            {
                tempSpot.Score += MatchData.Instance().PassToSpotIsSafeValue;
            }
            // Debug.LogFormat("tempSpot{0}: {1}", i, tempSpot.Score);

            if (_team.CanShoot(tempSpot.Position, MatchData.Instance().MaxShootForce))
            {
                tempSpot.Score += MatchData.Instance().CanScoreFromSpotValue;
            }
            // Debug.LogFormat("tempSpot{0}: {1}", i, tempSpot.Score);
           
            float distToSpot = Vector2.Distance(controllingPlayer.GetPosition(), tempSpot.Position);
            if (distToSpot < MatchData.Instance().MaxOptimalDistanceToSpot)
            {
                tempSpot.Score += distToSpot / MatchData.Instance().MaxOptimalDistanceToSpot * MatchData.Instance().DistanceToSpotValue;
            }
            // Debug.LogFormat("tempSpot{0}: {1}", i, tempSpot.Score);

            float totalDist = 0.0f;
            foreach (var supportPlayer in supportingPlayers)
            {
                if (supportPlayer != controllingPlayer)
                {
                    totalDist += Vector2.Distance(supportPlayer.SupportSpot.Position, tempSpot.Position) * MatchData.Instance().KeepAwayFromAnotherSupportPlayerValue;
                }
            }
            tempSpot.Score += totalDist;
            // Debug.LogFormat("tempSpot{0}: {1}", i, tempSpot.Score);

            if (tempSpot.Score > bestScore)
            {
                bestScore = tempSpot.Score;
                _bestSupportSpot = tempSpot;
            }
        }

        return _bestSupportSpot;
    }
}