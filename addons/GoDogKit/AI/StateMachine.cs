namespace GoDogKit.AI;

/// <summary>
/// Used to switch between different states.
/// </summary>
public class StateMachine(IState initialState) : IState
{
    public IState CurrentState { get; protected set; } = initialState;
    public void Ready() => CurrentState.Ready();
    public void Enter() => CurrentState.Enter();
    public void Process(double delta) => CurrentState.Process(delta);
    public void Exit() => CurrentState.Exit();

    public void ChangeState(IState target)
    {
        CurrentState.Exit();
        CurrentState = target;
        CurrentState.Enter();
    }
}