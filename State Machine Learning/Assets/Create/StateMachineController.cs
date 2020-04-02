using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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

    private void DrawState(StateMachineData.State state)
    {
        // Draw
        GameObject o = Instantiate(statePrefab);
        o.transform.position = (Vector3)state.pos + (Vector3.back * state.id * 0.01f);
        TMP_Text t = o.GetComponentInChildren<TMP_Text>();
        t.SetText(state.name);

        if (state.id == stateMachineData.initialState)
        {
            o.transform.Find("InitialStateArrow").gameObject.SetActive(true);
        }


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

    public void DrawState(StateMachineData.State state, bool selected = false)
    {
        DrawState(state);
        if (selected)
            ColorSelectedState(state);
    }

    public void UndrawState(StateMachineData.State state)
    {
        if (drawnStates.TryGetValue(state, out GameObject g))
        {
            Destroy(g);
            DrawTransitions();
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
            t.Set(startPos, endPos, stateMachineData.InputToString(sameTransition[0].input), sameTransition[0]);
            sameTransition.RemoveAt(0);
            foreach (StateMachineData.Transition item in sameTransition)
            {
                t.AddText(stateMachineData.InputToString(item.input), item);
            }

            drawnTransitions.Add(t);
        }
    }

    public StateMachineData.State SelectState(Vector2 pos)
    {
        StateMachineData.State s = stateMachineData.states.FindLast(x => Vector2.Distance(x.pos, pos) < Appdata.circleSize);
        if (s == null)
        {
            return null;
        }
        ColorSelectedState(s);
        return s;
    }

    private void ColorSelectedState(StateMachineData.State s)
    {
        // Change Color
        GameObject g = drawnStates[s];
        g.GetComponent<SpriteRenderer>().color = Appdata.highlightColor;
        g.GetComponentInChildren<TMP_Text>().color = Appdata.highlightColor;
        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -100f);
    }

    public void DeselectState(StateMachineData.State s)
    {
        if (s == null)
        {
            return;
        }
        GameObject g = drawnStates[s];
        g.GetComponent<SpriteRenderer>().color = Color.black;
        g.GetComponentInChildren<TMP_Text>().color = Color.black;
        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, s.id * -0.01f);
    }

    public TransitionPrefab SelectTransition(Vector2 pos)
    {
        TransitionPrefab t = drawnTransitions.Find(x => x.coll.OverlapPoint(pos));
        if (t == null)
        {
            return null;
        }
        t.SetColor(Appdata.highlightColor);
        t.lineRenderer.sortingOrder = -9;
        return t;
    }

    public void DeselectTransition(TransitionPrefab t)
    {
        if (t == null)
        {
            return;
        }
        t.SetColor(Color.black);
        t.lineRenderer.sortingOrder = -10;
        t.transform.position = Vector3.zero;
    }
}
