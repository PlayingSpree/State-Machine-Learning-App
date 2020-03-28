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
                break;
            case EditMode.SelectTransition:
                break;
            default:
                break;
        }
    }

    // UpdateEdit
    private void UpdateEdit()
    {
        if (input.Tap.HasValue)
        {
            SelectThing(input.Tap.Value);
        }
    }

    private void SelectThing(Vector2 pos)
    {
        if (!IsInCanvas(pos))
        {
            return;
        }

        smc.DeselectState(selectedState);
        selectedState = null;

        Vector2 wp = Camera.main.ScreenToWorldPoint(pos);
        selectedState = smc.SelectState(wp);

        if (selectedState == null)
        {
            selectedTransition = smc.SelectTransition(wp);
        }
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

    // Modify State Machine Function
    private void CreateState(Vector2 pos)
    {
        smc.DrawState(smc.stateMachineData.CreateState("S" + (smc.stateMachineData.states.Count + 1), pos));
    }

    private void ModifyState(int id, Vector2 pos)
    {
        smc.DrawState(smc.stateMachineData.ModifyState(id, pos));
        smc.DrawTransitions();
    }

    private void ModifyState(int id, string name)
    {
        smc.DrawState(smc.stateMachineData.ModifyState(id, name));
    }

    private void CreateTransition(int from, int to, int input)
    {
        smc.stateMachineData.CreateTransition(from, to, input);
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
    private void ChangeEditMode(EditMode editMode)
    {
        smc.DeselectState(selectedState);
        selectedState = null;
        smc.DeselectTransition(selectedTransition);
        selectedTransition = null;
        switch (editMode)
        {
            case EditMode.Edit:
                this.editMode = EditMode.Edit;
                animator.SetTrigger("Cancle");
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
                break;
            case EditMode.SelectTransition:
                break;
            default:
                break;
        }
    }
}

