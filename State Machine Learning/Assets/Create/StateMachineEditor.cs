using System;
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

    public TMPro.TMP_InputField stateNameInput;

    public SquareList transfromList;
    public TMPro.TMP_Dropdown transitionAddButton;

    public TMPro.TMP_Text NextButtonText;

    public SquareList testInputList;
    public TMPro.TMP_Dropdown testInputAddButton;
    public RectTransform testInputListPos;

    public TMPro.TMP_Text fileNameText;

    // UI Const
    const float canvasBottom = 160f / 1920f;
    const float canvasBottomTest = 440f / 1920f;
    const float canvasTop = 160f / 1920f;
    readonly string testTopPanelText = Environment.NewLine + "<size=50%>Testing";

    // Status
    enum EditMode { Edit, AddState, AddTransition, SelectState, SelectTransition, Test }
    EditMode editMode = EditMode.Edit;
    StateMachineData.State selectedState = null;
    TransitionPrefab selectedTransition = null;
    List<StateMachineData.Transition> editedTransition = new List<StateMachineData.Transition>();
    public bool PopupMode { get; set; } = false;
    bool testing = false;
    bool testError = false;
    bool testRuning = false;
    float testRunningTimer = 0f;

    #region Unity
    private void Start()
    {
        smc = GetComponent<StateMachineController>();
        ChangeEditMode(EditMode.Edit);
        fileNameText.SetText(smc.stateMachineData.meta_name);
    }

    private void Update()
    {
        if (!PopupMode)
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
                case EditMode.Test:
                    UpdateTest();
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    #region Edit Update
    // UpdateEdit
    private void UpdateEdit(bool ignoreDrag = false, bool ignoreSelect = false)
    {
        if (input.Tap.HasValue && !ignoreSelect)
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
        if (input.Drag.HasValue && !ignoreDrag && IsInCanvas(input.DragStart.Value))
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
            return;
        }
        UpdateEdit(ignoreSelect: true);
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
            return;
        }
        UpdateEdit(ignoreSelect: true);
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
                StateMachineData.Transition t = smc.stateMachineData.transitions.Find(x => x.from == selectedState.id && x.to == otherState.id);
                if (t != null)
                {
                    smc.DeselectState(otherState);
                    ChangeEditMode(EditMode.SelectTransition);
                    selectedTransition = smc.SelectTransition(t.from, t.to);
                }
                else
                {
                    CreateTransition(selectedState.id, otherState.id, 0);
                    smc.DeselectState(otherState);
                    ChangeEditMode(EditMode.Edit);
                }
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
        smc.stateMachineData.transitions.FindAll(x => x.from == t.transitions[0].from && x.to == t.transitions[0].to).ForEach(x => smc.stateMachineData.RemoveTransition(x));
        smc.DrawTransitions();
    }
    #endregion

    #region Edit UI
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
    // Popup
    public void SetStateNamePopup()
    {
        PopupMode = true;
        stateNameInput.SetTextWithoutNotify(selectedState.name);
        stateNameInput.placeholder.GetComponent<TMPro.TMP_Text>().SetText(selectedState.name);
    }
    public void SetStateNameOKButton()
    {
        if (stateNameInput.text.Trim() != "")
        {
            selectedState.name = stateNameInput.text;
            smc.DrawState(selectedState, true);
        }
        PopupMode = false;
    }
    public void TransitionEditPopup()
    {
        PopupMode = true;
        editedTransition = new List<StateMachineData.Transition>(selectedTransition.transitions);
        TransitionPopupUpdate();
    }
    private void TransitionPopupUpdate()
    {
        List<SquareList.SquareListOptions> l = new List<SquareList.SquareListOptions>();
        foreach (StateMachineData.Transition item in editedTransition)
        {
            l.Add(new SquareList.SquareListOptions(smc.stateMachineData.InputToString(item.input), item));
        }
        l.Sort((x, y) => ((StateMachineData.Transition)x.obj).input - ((StateMachineData.Transition)y.obj).input);
        transfromList.SetButtons(l, TransitionEditListButton);

        // Add Input Options
        transitionAddButton.ClearOptions();
        // Only not yet added input
        smc.stateMachineData.inputsData
            .FindAll(x => !editedTransition.Exists(y => y.input == x.id))
            .ForEach(x => transitionAddButton.options.Add(new TMPro.TMP_Dropdown.OptionData(x.name)));
        // Unity Sucks
        transitionAddButton.options.Add(new TMPro.TMP_Dropdown.OptionData("Dummy"));
        transitionAddButton.SetValueWithoutNotify(transitionAddButton.options.Count - 1);
        transitionAddButton.options.RemoveAt(transitionAddButton.options.Count - 1);
    }
    public void TransitionEditAddButton(int value)
    {
        StateMachineData.Input i = smc.stateMachineData.inputsData.Find(x => transitionAddButton.options[value].text == x.name);
        editedTransition.Add(new StateMachineData.Transition(selectedTransition.transitions[0].from, selectedTransition.transitions[0].to, i.id, -1));
        TransitionPopupUpdate();
    }
    public void TransitionEditListButton(object o)
    {
        StateMachineData.Transition t = (StateMachineData.Transition)o;
        editedTransition.Remove(t);
        TransitionPopupUpdate();
    }
    public void TransitionEditOKButton()
    {
        // Check Diff Then Update
        foreach (StateMachineData.Transition item in editedTransition)
        {
            // Add new Transitions
            if (!smc.stateMachineData.transitions.Exists(x => x.from == item.from && x.to == item.to && x.input == item.input))
            {
                smc.stateMachineData.CreateTransition(item.from, item.to, item.input);
            }
        }
        // Remove deleted Transitions
        List<StateMachineData.Transition> deleteThis = new List<StateMachineData.Transition>();
        foreach (StateMachineData.Transition item in smc.stateMachineData.transitions)
        {
            if (selectedTransition.transitions[0].from == item.from && selectedTransition.transitions[0].to == item.to && !editedTransition.Exists(x => x.input == item.input))
            {
                deleteThis.Add(item);
            }
        }
        deleteThis.ForEach(x => smc.stateMachineData.RemoveTransition(x));
        // Empty Transition?
        if (editedTransition.Count == 0)
        {
            ChangeEditMode(EditMode.Edit);
            smc.DrawTransitions();
        }
        else
        {
            smc.DrawTransitions();
            selectedTransition = smc.SelectTransition(selectedTransition.transitions[0].from, selectedTransition.transitions[0].to);
        }
        PopupMode = false;
    }
    // Helper
    public bool IsInCanvas(Vector2 pos)
    {
        if (pos.y < Screen.height * canvasBottom)
        {
            return false;
        }
        else if (editMode == EditMode.Test && pos.y < Screen.height * canvasBottomTest)
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
        if (editMode != EditMode.Test && this.editMode == EditMode.Test)
        {
            NextButtonText.SetText("Test");
            fileNameText.SetText(smc.stateMachineData.meta_name);
            smc.TestClear();
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
            case EditMode.Test:
                this.editMode = EditMode.Test;
                animator.SetTrigger("Test");
                tooltip.SetText("");
                NextButtonText.SetText("Run");
                fileNameText.SetText(smc.stateMachineData.meta_name + testTopPanelText);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Test Update
    private void UpdateTest()
    {
        UpdateEdit(ignoreSelect: true);
        TestRunningUpdate();
    }
    #endregion

    #region Test UI
    public void NextButton()
    {
        if (editMode != EditMode.Test)
        {
            ChangeEditMode(EditMode.Test);
            TestInputUpdate();
            TestReset();
        }
        else
        {
            if (testError)
            {
                TestReset();
            }
            else if (testRuning)
            {
                testRuning = false;
                NextButtonText.SetText("Continue");
            }
            else
            {
                TestRunningStart();
                NextButtonText.SetText("Pause");
            }
        }
    }
    private void TestInputUpdate()
    {
        List<SquareList.SquareListOptions> l = new List<SquareList.SquareListOptions>();
        for (int i = 0; i < smc.stateMachineData.inputs.Count; i++)
        {
            l.Add(new SquareList.SquareListOptions(smc.stateMachineData.InputToString(smc.stateMachineData.inputs[i]), i));
        }
        testInputList.SetButtons(l, TestInputListButton);

        // Highlight
        if (smc.CurrentInput != -1 && testing && l.Count > 0)
        {
            Color c;
            if (testError)
            {
                c = Appdata.errorColor;
            }
            else
            {
                c = Appdata.highlightColor;
            }
            testInputList.buttons[smc.CurrentInput].GetComponentInChildren<UnityEngine.UI.Image>().color = c;
        }

        // Add Input Options
        testInputAddButton.ClearOptions();
        // Only not yet added input
        smc.stateMachineData.inputsData
            .ForEach(x => testInputAddButton.options.Add(new TMPro.TMP_Dropdown.OptionData(x.name)));
        // Unity Sucks
        testInputAddButton.options.Add(new TMPro.TMP_Dropdown.OptionData("Dummy"));
        testInputAddButton.SetValueWithoutNotify(testInputAddButton.options.Count - 1);
        testInputAddButton.options.RemoveAt(testInputAddButton.options.Count - 1);

        if (testing)
        {
            testInputList.buttons.ForEach(x => x.GetComponentInChildren<UnityEngine.UI.Button>().enabled = false);
            ((RectTransform)testInputList.transform).offsetMax = new Vector2(0f, 150f);
            testInputAddButton.gameObject.SetActive(false);
        }
        else
        {
            ((RectTransform)testInputList.transform).offsetMax = new Vector2(-150f, 150f);
            testInputAddButton.gameObject.SetActive(true);
        }
    }
    public void TestInputListButton(object o)
    {
        smc.stateMachineData.inputs.RemoveAt((int)o);
        TestInputUpdate();
    }
    public void TestInputAddButton(int value)
    {
        StateMachineData.Input i = smc.stateMachineData.inputsData.Find(x => testInputAddButton.options[value].text == x.name);
        smc.stateMachineData.inputs.Add(i.id);
        TestInputUpdate();
    }
    public void TestBackStateButton()
    {
        if (testing)
        {
            testRuning = false;
            testError = false;
            tooltip.SetText("");
            NextButtonText.SetText("Continue");
            if (smc.TestBackState() == StateMachineController.TestStatus.SoI)
            {
                TestReset();
            }
            else
            {
                TestCenterInputList();
                testing = true;
                TestInputUpdate();
            }
        }
    }
    public void TestNextStateButton()
    {
        if (!testError)
        {
            NextButtonText.SetText("Continue");

            testRuning = false;
            TestNextState();
        }
    }
    private void TestNextState()
    {
        TestErrorCatch(smc.TestNextState());
        TestCenterInputList();

        testing = true;
        TestInputUpdate();
    }
    private void TestErrorCatch(StateMachineController.TestStatus s)
    {
        string errorColor = "<color=#" + ColorUtility.ToHtmlStringRGB(Appdata.errorColor) + ">";
        string warningColor = "<color=#FF813B>";
        switch (s)
        {
            case StateMachineController.TestStatus.Normal:
                tooltip.SetText("");
                break;
            case StateMachineController.TestStatus.TransitionError_Multiple:
                tooltip.SetText(errorColor + "Transition Error:" + Environment.NewLine + "Found multiple transitions for current input.");
                break;
            case StateMachineController.TestStatus.TransitionError_None:
                tooltip.SetText(errorColor + "Transition Error:" + Environment.NewLine + "Transition not found for current input.");
                break;
            case StateMachineController.TestStatus.EoI_Empty:
                tooltip.SetText(warningColor + "Input Warning:" + Environment.NewLine + "Input is empty." + Environment.NewLine + "</color>End of input.");
                break;
            case StateMachineController.TestStatus.EoI:
                tooltip.SetText("End of input.");
                break;
            default:
                break;
        }
        if (s != StateMachineController.TestStatus.Normal)
        {
            testError = true;
            testRuning = false;
            NextButtonText.SetText("Restart");
        }
    }
    public void TestReset()
    {
        smc.TestReset();

        tooltip.SetText("");
        NextButtonText.SetText("Run");

        testing = false;
        testError = false;
        testRuning = false;
        TestInputUpdate();
    }
    private void TestCenterInputList()
    {
        float offset = (159f * (smc.CurrentInput == 0 ? 1 : smc.CurrentInput)) - 159f;
        if (testInputListPos.rect.width < ((RectTransform)testInputListPos.parent).rect.width)
        {
            testInputListPos.offsetMin = Vector2.zero;
        }
        else if (testInputListPos.rect.width - ((RectTransform)testInputListPos.parent).rect.width > offset)
        {
            testInputListPos.offsetMin = Vector2.left * offset;
        }
        else
        {
            testInputListPos.offsetMin = Vector2.left * (testInputListPos.rect.width - ((RectTransform)testInputListPos.parent).rect.width);
        }
    }
    #endregion

    #region TestPlayer
    private void TestRunningStart()
    {
        testRunningTimer = Appdata.testRunningTime / 2f;
        testRuning = true;
    }
    private void TestRunningUpdate()
    {
        if (testRuning)
        {
            testRunningTimer -= Time.deltaTime;
            if (testRunningTimer <= 0)
            {
                TestNextState();
                testRunningTimer = Appdata.testRunningTime;
            }
        }
    }
    #endregion

    #region TopPanel
    public void BackButton()
    {
        ChangeEditMode(EditMode.Edit);
    }
    #endregion
}

