using System;

public class StateMachineStateData<T> where T : StateMachineState<T>
{
    public readonly T state;
    public readonly Action initializeFunction;

    public StateMachineStateData(T state, Action initializeFunction)
    {
        this.state = state;
        this.initializeFunction = initializeFunction;
    }
}