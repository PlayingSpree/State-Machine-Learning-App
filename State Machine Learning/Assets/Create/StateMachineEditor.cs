using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StateMachineEditor : MonoBehaviour
{
    // Ref
    StateMachineController smc;
    InputHandler input = new InputHandler();
    public Animator animator;
    public TMPro.TMP_Text tooltip;

    // UI Ref
    const float canvasBottom = 160f / 1920f;
    const float canvasTop = 160f / 1920f;

    // Status
    enum EditMode { Edit, AddState, AddTransition, SelectState, SelectTransition }
    EditMode editMode = EditMode.Edit;

    StateMachineData.State selectedState = null;
    TransitionPrefab selectedTransition = null;

    private void Start()
    {
        smc = GetComponent<StateMachineController>();
    }

    private void Update()
    {
        input.Update();
        switch (editMode)
        {
            case EditMode.Edit:
                UpdateEdit();
                break;
            case EditMode.AddState:
                UpdateAddState();
                break;
            case EditMode.AddTransition:
                UpdateAddTransition();
                break;
            case EditMode.SelectState:
                UpdateSelectState();
                break;
            case EditMode.SelectTransition:
                UpdateSelectTransition();
                break;
            default:
                break;
        }
    }

    // UpdateEdit
    private void UpdateEdit(bool ignoreDrag = false)
    {
        if (input.Tap.HasValue)
        {
            if (SelectThing(input.Tap.Value))
            {
                if (selectedState != null)
                {
                    ChangeEditMode(EditMode.SelectState, false);
                }
                else if (selectedTransition != null)
                {
                    ChangeEditMode(EditMode.SelectTransition, false);
                }
                else if (editMode != EditMode.Edit)
                {
                    ChangeEditMode(EditMode.Edit);
                }
            }
        }
        if (input.Drag.HasValue && !ignoreDrag)
        {
            Camera.main.transform.Translate(-input.Drag.Value * Camera.main.orthographicSize * 2f / Screen.height);
        }
    }

    private bool SelectThing(Vector2 pos)
    {
        if (!IsInCanvas(pos))
        {
            return false;
        }

        smc.DeselectState(selectedState);
        smc.DeselectTransition(selectedTransition);
        selectedState = null;

        Vector2 wp = Camera.main.ScreenToWorldPoint(pos);
        selectedState = smc.SelectState(wp);

        if (selectedState == null)
        {
            selectedTransition = smc.SelectTransition(wp);
        }
        return true;
    }

    // UpdateAddState
    private void UpdateAddState()
    {
        if (input.Tap.HasValue)
        {
            ClickCreateState(input.Tap.Value);
        }
    }

    private void ClickCreateState(Vector2 pos)
    {
        if (!IsInCanvas(pos))
        {
            return;
        }
        Vector2 wp = Camera.main.ScreenToWorldPoint(pos);
        CreateState(wp);
        ChangeEditMode(EditMode.Edit);
    }

    // UpdateAddTransition
    private void UpdateAddTransition()
    {
        if (input.Tap.HasValue)
        {
            ClickAddTransition(input.Tap.Value);
        }
    }

    private void ClickAddTransition(Vector2 pos)
    {
        if (!IsInCanvas(pos))
        {
            return;
        }
        Vector2 wp = Camera.main.ScreenToWorldPoint(pos);

        if (selectedState == null)
        {
            selectedState = smc.SelectState(wp);
        }
        else
        {
            StateMachineData.State otherState = smc.SelectState(wp);
            if (otherState != null)
            {
                CreateTransition(selectedState.id, otherState.id, 0);
                smc.DeselectState(otherState);
                ChangeEditMode(EditMode.Edit);
            }
        }
    }

    // UpdateSelectState
    private void UpdateSelectState()
    {
        UpdateEdit(true);
        if (input.Drag.HasValue)
        {
            if (Vector2.Distance(selectedState.pos, Camera.main.ScreenToWorldPoint(input.LastPos.Value)) < Appdata.circleSize)
            {
                selectedState.pos += input.Drag.Value * Camera.main.orthographicSize * 2f / Screen.height;
                smc.DrawState(selectedState, true);
                smc.DrawTransitions();
            }
            else
            {
                Camera.main.transform.Translate(-input.Drag.Value * Camera.main.orthographicSize * 2f / Screen.height);
            }
        }
    }

    private void UpdateSelectTransition()
    {
        UpdateEdit();
    }

    // Modify State Machine Function
    private void CreateState(Vector2 pos)
    {
        string name;
        if (smc.stateMachineData.states.Count == 0)
        {
            name = "S0";
        }
        else
        {
            name = "S" + (smc.stateMachineData.states[smc.stateMachineData.states.Count - 1].id + 1);
        }

        smc.DrawState(smc.stateMachineData.CreateState(name, pos));
    }

    private void CreateTransition(int from, int to, int input)
    {
        smc.stateMachineData.CreateTransition(from, to, input);
        smc.DrawTransitions();
    }

    private void RemoveState(StateMachineData.State s)
    {
        smc.stateMachineData.RemoveState(s);
        smc.UndrawState(s);
    }

    private void RemoveTransition(TransitionPrefab t)
    {
        smc.stateMachineData.transitions.FindAll(x => x.from == t.transition.from && x.to == t.transition.to).ForEach(x => smc.stateMachineData.RemoveTransition(x));
        smc.DrawTransitions();
    }
    // UI
    public void SelectButton()
    {
        ChangeEditMode(EditMode.Edit);
    }

    public void StateButton()
    {
        ChangeEditMode(EditMode.AddState);
    }
    public void TransitionButton()
    {
        ChangeEditMode(EditMode.AddTransition);
    }
    public void RemoveButton()
    {
        if (selectedState != null)
        {
            RemoveState(selectedState);
        }
        else if (selectedTransition != null)
        {
            RemoveTransition(selectedTransition);
        }
        ChangeEditMode(EditMode.Edit);
    }
    public void SetInitialButton()
    {
        if (smc.stateMachineData.initialState == -1)
        {
            smc.stateMachineData.initialState = selectedState.id;
        }
        else
        {
            if (smc.stateMachineData.initialState == selectedState.id)
            {
                smc.stateMachineData.initialState = -1;
            }
            else
            {
                int old = smc.stateMachineData.initialState;
                smc.stateMachineData.initialState = selectedState.id;
                smc.DrawState(smc.stateMachineData.states.Find(x => x.id == old));
            }
        }
        smc.DrawState(selectedState, true);
    }
    public bool IsInCanvas(Vector2 pos)
    {
        if (pos.y < Screen.height * canvasBottom)
        {
            return false;
        }
        else if (pos.y > Screen.height * (1 - canvasTop))
        {
            return false;
        }
        return true;
    }
    private void ChangeEditMode(EditMode editMode, bool deselect = true)
    {
        if (deselect)
        {
            smc.DeselectState(selectedState);
            selectedState = null;
            smc.DeselectTransition(selectedTransition);
            selectedTransition = null;
        }
        switch (editMode)
        {
            case EditMode.Edit:
                this.editMode = EditMode.Edit;
                animator.SetTrigger("Cancle");
                tooltip.SetText("Tap to select a state or transition.");
                break;
            case EditMode.AddState:
                this.editMode = EditMode.AddState;
                animator.SetTrigger("AddState");
                tooltip.SetText("Tap to add a new state.");
                break;
            case EditMode.AddTransition:
                this.editMode = EditMode.AddTransition;
                animator.SetTrigger("AddTransition");
                tooltip.SetText("Tap to add a new transition.");
                break;
            case EditMode.SelectState:
                this.editMode = EditMode.SelectState;
                animator.SetTrigger("SelectState");
                tooltip.SetText("Drag the selected state to move.");
                break;
            case EditMode.SelectTransition:
                this.editMode = EditMode.SelectTransition;
                animator.SetTrigger("SelectTransition");
                tooltip.SetText("");
                break;
            default:
                break;
        }
    }
}

