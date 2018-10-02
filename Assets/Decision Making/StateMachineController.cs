using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class State
{
    public Action Action { get; private set; }
    public Action EntryAction { get; private set; }
    public Action ExitAction { get; private set; }
    public List<Transition> Transitions { get; private set; }

    public State(Action action, Action entryAction, Action exitAction)
    {
        this.Action = action;
        this.EntryAction = entryAction;
        this.ExitAction = exitAction;
        this.Transitions = new List<Transition>();
    }

    public void SetTransitions(Transition[] transitions)
    {
        this.Transitions.AddRange(transitions);
    }
}

public class Transition
{
    public State TargetState { get; private set; }
    public Action Action { get; private set; }
    public bool IsTriggered { get; set; }

    public Transition(State targetState, Action action)
    {
        IsTriggered = false;
        this.TargetState = targetState;
        this.Action = action;
    }
}

public class StateMachine
{
    private State _initialState;
    private State _currentState;

    public StateMachine(State initialState)
    {
        this._currentState = this._initialState = initialState;
    }

    public void UpdateStateMachine()
    {
        Transition triggeredTransition = null;
        foreach (var transition in _currentState.Transitions)
        {
            if (transition.IsTriggered)
            {
                triggeredTransition = transition;
                transition.IsTriggered = false;
                break;
            }
        }

        if (triggeredTransition != null)
        {
            State targetState = triggeredTransition.TargetState;
            _currentState.ExitAction();
            triggeredTransition.Action();
            targetState.EntryAction();
            _currentState = targetState;
        }
        else
        {
            _currentState.Action();
        }
    }
}

public class StateMachineController : MonoBehaviour
{
    State guard;
    State fight;
    State runAway;

    Transition seeSmallEnemy;
    Transition seeBigEnemy;
    Transition losingFight;
    Transition escaped;

	void Start ()
    {
        guard = new State(OnGuardAction, OnGuardEntryAction, OnGuardExitAction);
        fight = new State(OnFightAction, OnFightEntryAction, OnFightExitAction);
        runAway = new State(OnRunAwayAction, OnRunAwayEntryAction, OnRunAwayExitAction);

        seeSmallEnemy = new Transition(fight, SeeSmallEnemyAction);
        seeBigEnemy = new Transition(runAway, SeeBigEnemyAction);
        losingFight = new Transition(runAway, LosingFightAction);
        escaped = new Transition(guard, EscapedAction);

        guard.SetTransitions(new Transition[] { seeSmallEnemy, seeBigEnemy });
        fight.SetTransitions(new Transition[] { losingFight });
        runAway.SetTransitions(new Transition[] { escaped });

        StateMachine sm = new StateMachine(guard);
        sm.UpdateStateMachine();
        seeSmallEnemy.IsTriggered = true;
        sm.UpdateStateMachine();
        sm.UpdateStateMachine();
    }

    private void OnGuardAction()
    {
        Debug.Log("Guard action");
    }

    private void OnGuardEntryAction()
    {
        Debug.Log("Guard entry action");
    }

    private void OnGuardExitAction()
    {
        Debug.Log("Guard exit action");
    }

    private void OnFightAction()
    {
        Debug.Log("Fight action");
    }

    private void OnFightEntryAction()
    {
        Debug.Log("Fight entry action");
    }

    private void OnFightExitAction()
    {
        Debug.Log("Fight exit action");
    }

    private void OnRunAwayAction()
    {
        Debug.Log("Run away action");
    }

    private void OnRunAwayEntryAction()
    {
        Debug.Log("Run away entry action");
    }

    private void OnRunAwayExitAction()
    {
        Debug.Log("Run away exit action");
    }

    private void SeeSmallEnemyAction()
    {
        Debug.Log("See small enemy action");
    }

    private void SeeBigEnemyAction()
    {
        Debug.Log("See big enemy action");
    }

    private void LosingFightAction()
    {
        Debug.Log("Losing fight action");
    }

    private void EscapedAction()
    {
        Debug.Log("Escaped action");
    }
}
