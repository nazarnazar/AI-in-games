public interface IFieldPlayerState
{
    void Enter(FieldPlayer player);
    void Execute(FieldPlayer player);
    void Exit(FieldPlayer player);
}