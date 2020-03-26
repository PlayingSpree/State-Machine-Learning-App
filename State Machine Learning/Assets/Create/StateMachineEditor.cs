using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineEditor : MonoBehaviour
{
    // Ref
    StateMachineController smc;

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
        UpdateInput();
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
        // Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                SelectThing(touch.position);
            }
        }
        // M+KB
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SelectThing(Input.mousePosition);
        }
    }

    private void SelectThing(Vector2 pos)
    {
        if (!IsInCanvas(pos))
        {
            return;
        }
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
        // Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                ClickCreateState(touch.position);
            }
        }
        // M+KB
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClickCreateState(Input.mousePosition);
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
        editMode = EditMode.Edit;
    }

    // UpdateAddTransition
    private void UpdateAddTransition()
    {
        // Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                ClickAddTransition(touch.position);
            }
        }
        // M+KB
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClickAddTransition(Input.mousePosition);
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
                CreateTransition(selectedState.id,otherState.id,0);
                editMode = EditMode.Edit;
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

    private void CreateTransition(int from,int to,int input)
    {
        smc.stateMachineData.CreateTransition(from, to, input);
        smc.DrawTransitions();
    }

    // UI
    public void SelectButton()
    {
        editMode = EditMode.Edit;
    }

    public void StateButton()
    {
        editMode = EditMode.AddState;
    }
    public void TransitionButton()
    {
        editMode = EditMode.AddTransition;
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

    // Input
    private void UpdateInput()
    {
        // Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //ClickCreateState(touch.position);
            }
        }
        // M+KB
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //ClickCreateState(Input.mousePosition);
        }
    }
}
