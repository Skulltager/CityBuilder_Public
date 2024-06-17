
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<T> where T : StateMachineState<T>
{
    public static StateMachine<T> currentInitializingStateMachine;
    public static T currentInitializingState;

    public readonly List<StateMachineStateData<T>> statesList;

    public T lastState { private set; get; }
    private T currentState;

    protected StateMachine()
    {
        statesList = new List<StateMachineStateData<T>>();
        currentInitializingStateMachine = this;
    }

    protected void Initialize(T initialState)
    {
        foreach (StateMachineStateData<T> stateData in statesList)
        {
            currentInitializingState = stateData.state;
            stateData.initializeFunction();
        }

        ChangeState(initialState);
    }

    public void ChangeState(T newState)
    {
        lastState = currentState;

        if (currentState != null)
            currentState.DisableState();

        if (newState != null)
            newState.EnableState();

        currentState = newState;
    }

    public void Update(float timeScale)
    {
        int loopsRemaining = 5;

        while (currentState.CheckTransitions_Update(timeScale) && loopsRemaining > 0)
            loopsRemaining--;

        currentState.Update(Time.deltaTime * timeScale);
    }

    public void FixedUpdate(float timeScale)
    {
        int loopsRemaining = 5;

        while (currentState.CheckTransitions_FixedUpdate(timeScale) && loopsRemaining > 0)
            loopsRemaining--;

        currentState.FixedUpdate(Time.fixedDeltaTime * timeScale);
    }
}
