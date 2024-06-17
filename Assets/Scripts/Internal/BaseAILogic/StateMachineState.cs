using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineState<T> where T : StateMachineState<T>
{
    public readonly StateMachine<T> stateMachine;
    public readonly List<StateMachineTransition<T>> stateMachineTransitions;
    public float deltaTimeInState { private set; get; }
    public float unscaledDeltaTimeInState { private set; get; }
    public float fixedDeltaTimeInState { private set; get; }
    public float unscaledFixedDeltaTimeInState { private set; get; }

    public StateMachineState(Action iniatilizeFunction)
    {
        stateMachineTransitions = new List<StateMachineTransition<T>>();

        stateMachine = StateMachine<T>.currentInitializingStateMachine;
        stateMachine.statesList.Add(new StateMachineStateData<T>(this as T, iniatilizeFunction));
    }

    public void EnableState()
    {
        foreach (StateMachineTransition<T> transition in stateMachineTransitions)
            transition.OnEnable();

        OnEnable();
    }

    public bool CheckTransitions_Update(float timeScale)
    {
        foreach (StateMachineTransition<T> transition in stateMachineTransitions)
        {
            if (!transition.ShouldTransition_Update(Time.deltaTime * timeScale))
                continue;

            stateMachine.ChangeState(transition.targetState);
            return true;
        }

        deltaTimeInState += Time.deltaTime * timeScale;
        unscaledDeltaTimeInState += Time.unscaledDeltaTime * timeScale;
        return false;
    }

    public bool CheckTransitions_FixedUpdate(float timeScale)
    {
        foreach (StateMachineTransition<T> transition in stateMachineTransitions)
        {
            if (!transition.ShouldTransition_FixedUpdate(Time.fixedDeltaTime * timeScale))
                continue;

            stateMachine.ChangeState(transition.targetState);
            return true;
        }

        fixedDeltaTimeInState += Time.fixedDeltaTime * timeScale;
        unscaledFixedDeltaTimeInState += Time.fixedUnscaledDeltaTime * timeScale;

        return false;
    }

    public void DisableState()
    {
        foreach (StateMachineTransition<T> transition in stateMachineTransitions)
            transition.OnDisable();

        OnDisable();

        deltaTimeInState = 0;
        unscaledDeltaTimeInState = 0;
        fixedDeltaTimeInState = 0;
        unscaledFixedDeltaTimeInState = 0;
    }

    public virtual void Update(float deltaTime) { }
    public virtual void FixedUpdate(float fixedDeltaTime) { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
}
