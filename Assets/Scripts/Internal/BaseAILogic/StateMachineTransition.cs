public abstract class StateMachineTransition<T> where T : StateMachineState<T>
{
    public readonly StateMachine<T> stateMachine;
    public readonly T checkingState;
    public readonly T targetState;

    public StateMachineTransition(T targetState)
    {
        stateMachine = StateMachine<T>.currentInitializingStateMachine;
        checkingState = StateMachine<T>.currentInitializingState;
        this.targetState = targetState;

        checkingState.stateMachineTransitions.Add(this);
    }

    public virtual void OnEnable() { }
    public virtual bool ShouldTransition_FixedUpdate(float fixedDeltaTime) { return false; }
    public virtual bool ShouldTransition_Update(float deltaTime) { return false; }
    public virtual void OnDisable() { }
}