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
    Dictionary<StateMachineData.Transition, GameObject> drawnTransitions = new Dictionary<StateMachineData.Transition, GameObject>();

    // Prefab
    public GameObject statePrefab;
    public GameObject transitionPrefab;

    // Reference
    public Transform camPos;

    private void Start()
    {
        // Draw all
        stateMachineData.states.ForEach(DrawState);
        stateMachineData.transitions.ForEach(DrawTransition);
    }

    private void DrawState(StateMachineData.State state)
    {
        // Draw
        GameObject o = Instantiate(statePrefab);
        o.transform.position = state.pos;
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

    private void DrawTransition(StateMachineData.Transition transition)
    {
        // Draw
        GameObject o = Instantiate(transitionPrefab);
        Vector2 startPos = stateMachineData.states.Find(x => x.id == transition.from).pos;
        Vector2 endPos = stateMachineData.states.Find(x => x.id == transition.to).pos;
        o.transform.position = Vector3.zero;
        o.GetComponent<TransitionPrefab>().set(startPos, endPos, transition.input.ToString());

        // Remove old and add new obj to list
        GameObject old;
        if (drawnTransitions.TryGetValue(transition, out old))
        {
            Destroy(old);
            drawnTransitions[transition] = o;
        }
        else
        {
            drawnTransitions.Add(transition, o);
        }
    }

    private void CreateState(Vector2 pos)
    {
        DrawState(stateMachineData.CreateState("S" + (stateMachineData.states.Count + 1), pos));
    }

    private void ModifyState(int id, Vector2 pos, string name = "")
    {
        stateMachineData.ModifyState(id, pos, name);
        DrawState(stateMachineData.states.Find(x => x.id == id));

        // Redraw all relate transitions
        stateMachineData.transitions.FindAll(x => x.from == id || x.to == id).ForEach(DrawTransition);
    }

    private void ModifyState(int id, string name)
    {
        stateMachineData.ModifyState(id, name);
        DrawState(stateMachineData.states.Find(x => x.id == id));
    }

    public void CreateStateButton()
    {
        CreateState(camPos.position);
    }

    public void EditButton()
    {
        ModifyState(0, Vector2.up);
    }
}
