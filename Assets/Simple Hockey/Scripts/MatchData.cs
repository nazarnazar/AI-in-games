
public class MatchData
{
    public float KickMaxForce = 10.0f;
    public float KickForceWhileDribble = 15.0f;
    public float KickNoiseWhileDribble = 5.0f;
    public float MinPassForce = 100.0f;
    public float MaxPassForce = 300.0f;
    public float MaxShootForce = 400.0f;
    public float PlayerMaxSpeed = 2.0f;
    public float PlayerMaxSpeedWithPuck = 1.0f;
    public float MaxOptimalDistanceToSpot = 5.0f;
    public float PassIntersectionRadius = 1.0f;
    public float ThreatenedRadius = 2.0f;
    public float AttackPuckKickRadius = 0.3f;
    public float DefencePuckKickRadius = 0.3f;
    public float MinPassDistance = 0.1f;
    public float PassToSpotIsSafeValue = 1.0f;
    public float KeepAwayFromAnotherSupportPlayerValue = 0.25f;
    public float CanScoreFromSpotValue = 1.0f;
    public float DistanceToSpotValue = 1.0f;
    public float ChancePlayerAttemptsToShot = 0.8f;
    public float KickNoise = 0.1f;
    public float E = 0.1f;
    public float PassAheadValue = 0.5f;
    public bool IsDebugMode = false;

    private static MatchData _matchData;

    public static MatchData Instance()
    {
        if (_matchData == null)
        {
            _matchData = new MatchData();
        }
        return _matchData;
    }
}