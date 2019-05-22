using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchController : MonoBehaviour
{
    [SerializeField] private Rink _rink;
    [SerializeField] private Team _teamA;
    [SerializeField] private Team _teamB;
    [SerializeField] private TextMeshProUGUI _teamAScore;
    [SerializeField] private TextMeshProUGUI _teamBScore;
    [SerializeField] private FansController _fansController;

    private void Start()
    {
        _rink.InitRink();
        _teamA.InitTeam();
        _teamB.InitTeam();
    }

    public void BeginMatch(Button btn)
    {
        _teamAScore.text = "0";
        _teamBScore.text = "0";

        _teamA.StartPlaying();
        _teamB.StartPlaying();

        _teamA.OpponentsScored(() =>
            {
                _teamBScore.text = (int.Parse(_teamBScore.text) + 1).ToString();
                _fansController.TeamBScored();
                _teamA.StartPlaying();
                _teamB.StartPlaying();
                Invoke("PlacePuck", 2.0f);
            });
        _teamB.OpponentsScored(() =>
            {
                _teamAScore.text = (int.Parse(_teamAScore.text) + 1).ToString();
                _fansController.TeamAScored();
                _teamA.StartPlaying();
                _teamB.StartPlaying();
                Invoke("PlacePuck", 2.0f);
            });

        StartCoroutine(Updating());

        btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Restart";
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
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

    private void PlacePuck()
    {
        _rink.GetPuck().PlaceAtPosition(Vector2.zero);
    }
}