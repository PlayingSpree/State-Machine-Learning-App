using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler
{
    // Output
    private Vector2? tap;
    private Vector2? hold;
    // Getters
    public Vector2? Tap => tap;
    public Vector2? Hold => hold;
    // Update : pls call every frame
    public void Update()
    {
        // Reset Output
        tap = null;
        hold = null;
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
            // Hold
            if (mouseTime > 1f)
            {
                hold = Input.mousePosition;
                // Set to -inf to Trigger hold once
                mouseTime = float.NegativeInfinity;
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            // Not Move
            if (Vector2.Distance(mousePosStart, Input.mousePosition) < 10f)
            {
                // Not Hold
                if (mouseTime >= 0)
                {
                    // Tap
                    tap = Input.mousePosition;
                }
            }
        }
    }
}
