using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPrefab : MonoBehaviour
{
    // Ref
    public GameObject arrow;
    public LineRenderer lineRenderer;
    public TMPro.TMP_Text text;
    public BoxCollider2D coll;
    public int from, to;

    public void set(Vector2 start, Vector2 end, string s, int from, int to)
    {
        this.from = from;
        this.to = to;

        text.SetText(s);

        // Line Pos
        Vector2 e = Vector2.MoveTowards(end, start, Appdata.circleSize + (Appdata.arrowSize / 2f));
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[] { start, e });

        // Arrow Pos
        e = Vector2.MoveTowards(e, start, Appdata.arrowSize / 2f);
        arrow.transform.position = e;

        // Arrow Rot
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, end - start));

        // Text Pos
        Vector2 ts = Vector2.MoveTowards(start, end, Appdata.circleSize);
        text.gameObject.transform.position = Vector2.Lerp(ts, e, 0.5f);

        // Text Rot
        float textRot = (arrow.transform.rotation.eulerAngles.z + 90f) % 360;
        if (textRot > 90f && textRot < 270f)
        {
            text.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, textRot + 180f);
            ((RectTransform)text.gameObject.transform).pivot = new Vector2(0.5f, 0.85f);
        }
        else
        {
            text.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, textRot);
        }

        // Coll Pos
        coll.gameObject.transform.position = ts;
        coll.size = new Vector2(Vector2.Distance(start, end) - (Appdata.circleSize * 2f), 0.35f);
        coll.offset = new Vector2(coll.size.x / 2f, 0f);

        // Coll Rot
        coll.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, textRot);
    }
    public void AddText(string s)
    {
        text.SetText(string.Join(",", text.text, s));
    }
}
