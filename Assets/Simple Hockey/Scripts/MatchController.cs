using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MatchController : MonoBehaviour
{
    [SerializeField] private Rink _rink;
    [SerializeField] private Team _teamA;
    [SerializeField] private Team _teamB;
    [SerializeField] private TextMeshProUGUI _teamAScore;
    [SerializeField] private TextMeshProUGUI _teamBScore;
    [SerializeField] private FansController _fansController;

    public void BeginMatch()
    {
        _teamAScore.text = "0";
        _teamBScore.text = "0";

        _rink.InitRink();
        _teamA.InitTeam();
        _teamB.InitTeam();

        _teamA.OpponentsScored(() =>
            {
                _teamBScore.text = (int.Parse(_teamBScore.text) + 1).ToString();
                _fansController.TeamBScored();
                _teamA.InitTeam();
                _teamB.InitTeam();
            });
        _teamB.OpponentsScored(() =>
            {
                _teamAScore.text = (int.Parse(_teamAScore.text) + 1).ToString();
                _fansController.TeamAScored();
                _teamA.InitTeam();
                _teamB.InitTeam();
            });

        StartCoroutine(Updating());
    }

    public void ChangeTeamAColor(Image btnImage)
    {
        Color color = new Color(Random.Range(0.0f, 1.0f),
                          Random.Range(0.0f, 1.0f),
                          Random.Range(0.0f, 1.0f));
        _teamA.ChangeTeamColor(color);
        btnImage.color = color;
        _fansController.ChangeTeamAFansColor(color);
    }

    public void ChangeTeamBColor(Image btnImage)
    {
        Color color = new Color(Random.Range(0.0f, 1.0f),
                          Random.Range(0.0f, 1.0f),
                          Random.Range(0.0f, 1.0f));
        _teamB.ChangeTeamColor(color);
        btnImage.color = color;
        _fansController.ChangeTeamBFansColor(color);
    }

    private IEnumerator Updating()
    {
        while (true)
        {
            _teamA.UpdateTeam();
            _teamB.UpdateTeam();
            yield return null;
        }
    }
}