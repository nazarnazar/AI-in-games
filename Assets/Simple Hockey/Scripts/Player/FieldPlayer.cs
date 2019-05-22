using UnityEngine;
using TMPro;

public class FieldPlayer : Player
{
    [SerializeField] private TextMeshProUGUI _tacticsHomeRegionText;
    [SerializeField] private TextMeshProUGUI _tacticsAttackRegionText;
    [SerializeField] private TextMeshProUGUI _tacticsPlayerNumber;

    private FieldPlayerStateMachine _fieldPlayerStateMachine;

    public void InitFieldPlayer()
    {
        base.InitPlayer();

        _tacticsHomeRegionText.text = GetHomeRegionIndex().ToString();
        _tacticsAttackRegionText.text = GetAttackRegionIndex().ToString();
        _tacticsPlayerNumber.text = "P" + GetPlayerNumber();
    }

    public void StartPlaying()
    {
        _fieldPlayerStateMachine = new FieldPlayerStateMachine(this, null);
    }

    public void UpdateFieldPlayer()
    {
        _fieldPlayerStateMachine.ExecuteSomeState();
    }

    public IFieldPlayerState CurrentState()
    {
        return _fieldPlayerStateMachine.GetCurrentState();
    }

    public void ChangeState(IFieldPlayerState newState)
    {
        _fieldPlayerStateMachine.ChangeState(newState);
    }

    public void SendMessage(MessageType messageType)
    {
        _fieldPlayerStateMachine.NewMessage(this, messageType);
    }

    public void UseNextHomeRegion(TextMeshProUGUI text)
    {
        int newHomeRegion = int.Parse(text.text) + 1;
        if (newHomeRegion > Team.GetRink().GetRinkRegions().Length - 1)
        {
            return;
        }
        text.text = newHomeRegion.ToString();
        SetHomeRegionIndex(newHomeRegion);
    }

    public void UsePreviousHomeRegion(TextMeshProUGUI text)
    {
        int newHomeRegion = int.Parse(text.text) - 1;
        if (newHomeRegion < 0)
        {
            return;
        }
        text.text = newHomeRegion.ToString();
        SetHomeRegionIndex(newHomeRegion);
    }

    public void UseNextAttackRegion(TextMeshProUGUI text)
    {
        int newAttackRegion = int.Parse(text.text) + 1;
        if (newAttackRegion > Team.GetRink().GetRinkRegions().Length - 1)
        {
            return;
        }
        text.text = newAttackRegion.ToString();
        SetAttackRegionIndex(newAttackRegion);
    }

    public void UsePreviousAttackRegion(TextMeshProUGUI text)
    {
        int newAttackRegion = int.Parse(text.text) - 1;
        if (newAttackRegion < 0)
        {
            return;
        }
        text.text = newAttackRegion.ToString();
        SetAttackRegionIndex(newAttackRegion);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != "Rink Wall")
        {
            Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), other.collider);
        }
    }
}