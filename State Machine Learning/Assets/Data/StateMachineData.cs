using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachineData
{
    // Meta Data
    public List<Input> inputsData = new List<Input>();

    // State Machine Data
    public List<State> states = new List<State>();
    public List<int> inputs = new List<int>();
    public List<Transition> transitions = new List<Transition>();
    public int initialState = -1;

    // Defalut Data
    public StateMachineData()
    {
        inputsData.Add(new Input("A", 0));
        inputsData.Add(new Input("B", 1));
        states.Add(new State("S0", 0, Vector2.left * 1.5f));
        states.Add(new State("S1", 1, Vector2.right * 1.5f));
        transitions.Add(new Transition(0, 1, 0, 0));
        inputs.Add(0);
        initialState = 0;
    }

    // Meta Data Function
    public void AddInputData(string name)
    {
        int id = inputsData.Count == 0 ? 0 : inputsData[inputsData.Count - 1].id++;
        inputsData.Add(new Input(name, id));
    }

    public void ModifyInputData(int id, string name)
    {
        inputsData.Find(x => x.id == id).name = name;
    }

    public void RemoveInputData(int id)
    {
        // Remove InputData
        inputsData.RemoveAll(x => x.id == id);
        // Delete all related trasitions
        transitions.RemoveAll(x => x.input == id);
        // Delete all related inputs
        inputs.Remove(id);
    }

    // State Machine Function
    public State CreateState(string name, Vector2 pos)
    {
        int id = states.Count == 0 ? 0 : states[states.Count - 1].id + 1;
        initialState = states.Count == 0 ? 0 : initialState;
        State s = new State(name, id, pos);
        states.Add(s);
        return s;
    }

    //public State ModifyState(int id, Vector2 pos, string name = "")
    //{
    //    State s = states.Find(x => x.id == id);
    //    s.name = name == "" ? s.name : name;
    //    s.pos = pos;
    //    return s;
    //}

    //public State ModifyState(int id, string name)
    //{
    //    State s = states.Find(x => x.id == id);
    //    s.name = name;
    //    return s;
    //}

    //public void RemoveState(int id)
    //{
    //    // Remove state
    //    states.RemoveAll(x => x.id == id);
    //    // Delete all related trasitions
    //    transitions.RemoveAll(x => x.from == id || x.to == id);
    //    // Initial state?
    //    if (initialState == id)
    //    {
    //        initialState = -1;
    //    }
    //}

    public void RemoveState(State s)
    {
        // Remove state
        states.Remove(s);
        // Delete all related trasitions
        transitions.RemoveAll(x => x.from == s.id || x.to == s.id);
        // Initial state?
        if (initialState == s.id)
        {
            initialState = -1;
        }
    }

    public void CreateTransition(int from, int to, int input)
    {
        int id = transitions.Count == 0 ? 0 : transitions[transitions.Count - 1].id++;
        transitions.Add(new Transition(from, to, input, id));
    }

    //public void ModifyTransition(int id, int from = -1, int to = -1, int input = -1)
    //{
    //    Transition obj = transitions.Find(x => x.id == id);
    //    obj.from = from == -1 ? obj.from : from;
    //    obj.to = to == -1 ? obj.to : to;
    //    obj.input = input == -1 ? obj.input : input;
    //}

    //public void RemoveTransition(int id)
    //{
    //    transitions.RemoveAll(x => x.id == id);
    //}

    public void RemoveTransition(Transition t)
    {
        transitions.Remove(t);
    }

    public void AddInput(int id)
    {
        inputs.Add(id);
    }

    //public void ModifyInput(int index, int id)
    //{
    //    inputs[index] = id;
    //}

    //public void RemoveInput(int index)
    //{
    //    // Remove Input
    //    inputs.RemoveAt(index);
    //}

    public string InputToString(int id)
    {
        return inputsData.Find(x => x.id == id).name;
    }

    // State Machine Class
    public class State
    {
        public string name;
        public int id;
        public Vector2 pos;

        public State(string name, int id, Vector2 pos)
        {
            this.name = name;
            this.id = id;
            this.pos = pos;
        }
    }
    public class Input
    {
        public string name;
        public int id;

        public Input(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
    public class Transition
    {
        public int from;
        public int to;
        public int input;
        public int id;

        public Transition(int from, int to, int input, int id)
        {
            this.from = from;
            this.to = to;
            this.input = input;
            this.id = id;
        }
    }
}
