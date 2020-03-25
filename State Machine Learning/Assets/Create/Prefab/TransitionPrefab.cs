﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPrefab : MonoBehaviour
{
    // Size const
    const float circleSize = 0.5f; // Half
    const float arrowSize = 0.225f; // (0.3 * 3/4)

    // Ref
    public GameObject arrow;
    public LineRenderer lineRenderer;
    public TMPro.TMP_Text text;

    public void set(Vector2 start, Vector2 end, string s)
    {
        text.SetText(s);

        // Line Pos
        Vector2 e = Vector2.MoveTowards(end, start, circleSize + (arrowSize / 2f));
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[] { start, e });

        // Arrow Pos
        e = Vector2.MoveTowards(e, start, arrowSize / 2f);
        arrow.transform.position = e;

        // Arrow Rot
        arrow.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, end - start));
    }
}