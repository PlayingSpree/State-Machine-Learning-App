﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateMachineController : MonoBehaviour
{
    // Data
    public StateMachineData stateMachineData = new StateMachineData();

    // Drawn Obj
    Dictionary<StateMachineData.State, GameObject> drawnStates = new Dictionary<StateMachineData.State, GameObject>();
    List<TransitionPrefab> drawnTransitions = new List<TransitionPrefab>();

    // Prefab
    public GameObject statePrefab;
    public GameObject transitionPrefab;

    private void Start()
    {
        // Draw all
        stateMachineData.states.ForEach(DrawState);
        DrawTransitions();
    }
    public void DrawState(StateMachineData.State state)
    {
        // Draw
        GameObject o = Instantiate(statePrefab);
        o.transform.position = (Vector3)state.pos + (Vector3.back * state.id);
        TMP_Text t = o.GetComponentInChildren<TMP_Text>();
        t.SetText(state.name);

        // Remove old and add new obj to list
        GameObject old;
        if (drawnStates.TryGetValue(state, out old))
        {
            Destroy(old);
            drawnStates[state] = o;
        }
        else
        {
            drawnStates.Add(state, o);
        }
    }

    public void DrawTransitions()
    {
        // Remove old
        drawnTransitions.ForEach(x => Destroy(x.gameObject));
        drawnTransitions.Clear();

        // Copy of all transitions to draw
        List<StateMachineData.Transition> transitions = new List<StateMachineData.Transition>(stateMachineData.transitions);

        while (transitions.Count > 0)
        {
            // Stack same transition with other transitions
            List<StateMachineData.Transition> sameTransition = transitions.FindAll(x => x.from == transitions[0].from && x.to == transitions[0].to);
            transitions.RemoveAll(x => sameTransition.Contains(x));

            // Draw
            GameObject o = Instantiate(transitionPrefab);
            o.transform.position = Vector3.zero;

            // Pos
            Vector2 startPos = stateMachineData.states.Find(x => x.id == sameTransition[0].from).pos;
            Vector2 endPos = stateMachineData.states.Find(x => x.id == sameTransition[0].to).pos;

            // Set & Add Text
            TransitionPrefab t = o.GetComponent<TransitionPrefab>();
            t.set(startPos, endPos, stateMachineData.InputToString(sameTransition[0].input), sameTransition[0].from, sameTransition[0].to);
            sameTransition.RemoveAt(0);
            foreach (StateMachineData.Transition item in sameTransition)
            {
                t.AddText(stateMachineData.InputToString(item.input));
            }

            drawnTransitions.Add(t);
        }
    }

    public StateMachineData.State SelectState(Vector2 pos)
    {
        return stateMachineData.states.Find(x => Vector2.Distance(x.pos, pos) < Appdata.circleSize);
    }

    public TransitionPrefab SelectTransition(Vector2 pos)
    {
        return drawnTransitions.Find(x => x.coll.OverlapPoint(pos));
    }
}
