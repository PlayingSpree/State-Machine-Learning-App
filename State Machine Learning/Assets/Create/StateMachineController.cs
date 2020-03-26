using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateMachineController : MonoBehaviour
{
    // Data
    StateMachineData stateMachineData = new StateMachineData();

    // Drawn Obj
    Dictionary<StateMachineData.State, GameObject> drawnStates = new Dictionary<StateMachineData.State, GameObject>();
    List<GameObject> drawnTransitions = new List<GameObject>();

    // Prefab
    public GameObject statePrefab;
    public GameObject transitionPrefab;

    // Reference
    public Transform camPos;

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

    private void DrawTransitions()
    {
        // Remove old
        drawnTransitions.ForEach(Destroy);

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
            t.set(startPos, endPos, stateMachineData.InputToString(sameTransition[0].input));
            sameTransition.RemoveAt(0);
            foreach (StateMachineData.Transition item in sameTransition)
            {
                t.AddText(stateMachineData.InputToString(item.input));
            }

            drawnTransitions.Add(o);
        }
    }

    private void CreateState(Vector2 pos)
    {
        DrawState(stateMachineData.CreateState("S" + (stateMachineData.states.Count + 1), pos));
    }

    private void ModifyState(int id, Vector2 pos, string name = "")
    {
        StateMachineData.State s = stateMachineData.ModifyState(id, pos, name);
        DrawState(s);
        DrawTransitions();
    }

    private void ModifyState(int id, string name)
    {
        stateMachineData.ModifyState(id, name);
        DrawState(stateMachineData.states.Find(x => x.id == id));
    }
}
