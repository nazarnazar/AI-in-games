using UnityEngine;

public class FansController : MonoBehaviour
{
    [SerializeField] private GameObject _fanPrefab;
    [SerializeField] private Bounds[] _teamAFanSectors;
    [SerializeField] private Bounds[] _teamBFanSectors;
    [SerializeField] private int _numberOfFans;

    private GameObject[,] _teamAFans;
    private GameObject[,] _teamBFans;

    private void Start()
    {
        _teamAFans = new GameObject[_teamAFanSectors.Length, _numberOfFans];
        _teamBFans = new GameObject[_teamBFanSectors.Length, _numberOfFans];
        SetTeamFans(_teamAFanSectors, _teamAFans);
        SetTeamFans(_teamBFanSectors, _teamBFans);
    }

    public void ChangeTeamAFansColor(Color newColor)
    {
        ChangeTeamFansColor(_teamAFanSectors, _teamAFans, newColor);
    }

    public void ChangeTeamBFansColor(Color newColor)
    {
        ChangeTeamFansColor(_teamBFanSectors, _teamBFans, newColor);
    }

    public void TeamAScored()
    {
        TeamScored(_teamAFanSectors, _teamAFans);
    }

    public void TeamBScored()
    {
        TeamScored(_teamBFanSectors, _teamBFans);
    }

    private void SetTeamFans(Bounds[] teamFanSector, GameObject[,] teamFans)
    {
        for (int i = 0; i < teamFanSector.Length; i++)
        {
            Bounds bounds = teamFanSector[i];
            for (int j = 0; j < _numberOfFans; j++)
            {
                Vector2 randomPos = new Vector2(Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y));
                teamFans[i, j] = Instantiate(_fanPrefab, transform);
                teamFans[i, j].transform.localPosition = randomPos;
            }
        }
    }

    private void ChangeTeamFansColor(Bounds[] teamFanSectors, GameObject[,] teamFans, Color newColor)
    {
        for (int i = 0; i < teamFanSectors.Length; i++)
        {
            for (int j = 0; j < _numberOfFans; j++)
            {
                teamFans[i, j].GetComponent<SpriteRenderer>().color = newColor;
            }
        }
    }

    private void TeamScored(Bounds[] teamFanSectors, GameObject[,] teamFans)
    {
        for (int i = 0; i < teamFanSectors.Length; i++)
        {
            for (int j = 0; j < _numberOfFans; j++)
            {
                teamFans[i, j].GetComponent<Fan>().GetExcited();
            }
        }
    }
}