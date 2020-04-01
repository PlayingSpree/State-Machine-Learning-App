using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler
{
    // Output
    private Vector2? tap;
    private Vector2? hold;
    private Vector2? drag;
    private Vector2? lastPos;
    // Getters
    public Vector2? Tap => tap;
    public Vector2? Hold => hold;
    public Vector2? Drag => drag;
    public Vector2? LastPos => lastPos;
    // Update : pls call every frame
    public void Update()
    {
        // Reset Output
        tap = null;
        hold = null;
        drag = null;
        lastPos = null;
        // Update Output
        UpdateTouch();
        UpdateMouse();
    }

    private void UpdateTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {

            }
        }
    }
    float mouseTime;
    Vector2 mousePosStart;
    Vector2 mousePosLast;
    private void UpdateMouse()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mousePosStart = Input.mousePosition;
            mouseTime = 0;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            mouseTime += Time.deltaTime;
            // Is Hold?
            if (mouseTime > 1f)
            {
                // Hold
                hold = Input.mousePosition;
                // Set to -inf to Trigger hold once
                mouseTime = float.NegativeInfinity;
            }
            // Move
            if (Vector2.Distance(mousePosStart, Input.mousePosition) >= 5f)
            {
                // Drag
                drag = (Vector2)Input.mousePosition - mousePosLast;
                // Not Hold
                mouseTime = float.NegativeInfinity;
            }
            // lastPos
            lastPos = mousePosLast;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            // Not Move
            if (Vector2.Distance(mousePosStart, Input.mousePosition) < 5f)
            {
                // Not Hold
                if (mouseTime >= 0)
                {
                    // Tap
                    tap = Input.mousePosition;
                }
            }
        }
        mousePosLast = Input.mousePosition;
    }
}
