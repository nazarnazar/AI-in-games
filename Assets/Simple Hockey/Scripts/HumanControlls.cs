using UnityEngine;

public class HumanControlls : MonoBehaviour
{
    [SerializeField] private string _horizontalAxisName;
    [SerializeField] private string _verticalAxisName;
    [SerializeField] private string _kickAxisName;

    public Vector2 LastKickDirection { get; set; }

    private void Update()
    {
        if (IsWillingToKick())
        {
            LastKickDirection = GetDirection();
        }
    }

    public Vector2 GetDirection()
    {
        return new Vector2(Input.GetAxis(_horizontalAxisName), Input.GetAxis(_verticalAxisName)).normalized;
    }

    public bool IsWillingToKick()
    {
        if (Input.GetAxis(_kickAxisName) > 0.1f)
        {
            return true;
        }
        return false;
    }
}