using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate bool Test(); 

public class Node
{
    private Node _true;
    private Node _false;
    private Test _test;
    private Action _action;

    public Node(Node trueNode, Node falseNode, Test test, Action action)
    {
        this._true = trueNode;
        this._false = falseNode;
        this._test = test;
        this._action = action;
    }

    public void MakeDecision()
    {
        if (_action != null)
            _action();
        else
        {
            if (_test())
                _true.MakeDecision();
            else
                _false.MakeDecision();
        }
    }
}

public class DecisionTreeController : MonoBehaviour
{
    private bool _enemyIsVisible;
    private bool _enemyIsAudible;
    private bool _enemyIsFarAway;
    private bool _enemyIsOnFlank;

	void Start ()
    {
        _enemyIsVisible = true;
        _enemyIsAudible = true;
        _enemyIsFarAway = true;
        _enemyIsOnFlank = true;

        Node move = new Node(null, null, null, Move);
        Node attack = new Node(null, null, null, Attack);
        Node creep = new Node(null, null, null, Creep);
        Node doNothing = new Node(null, null, null, DoNothing);

        Node flank = new Node(move, attack, IsEnemyOnFlank, null);
        Node far = new Node(flank, attack, IsEnemyFarAway, null);
        Node audible = new Node(creep, doNothing, IsEnemyAudible, null);
        Node visible = new Node(far, audible, IsEnemyVisible, null);

        visible.MakeDecision();
    }

    private bool IsEnemyVisible()
    {
        return _enemyIsVisible;
    }

    private bool IsEnemyAudible()
    {
        return _enemyIsAudible;
    }

    private bool IsEnemyFarAway()
    {
        return _enemyIsFarAway;
    }

    private bool IsEnemyOnFlank()
    {
        return _enemyIsOnFlank;
    }

    private void Move()
    {
        Debug.Log("I am moving");
    }

    private void Attack()
    {
        Debug.Log("I am attacking");
    }

    private void Creep()
    {
        Debug.Log("I am creeping");
    }

    private void DoNothing()
    {
        Debug.Log("I am doing nothing");
    }
}
