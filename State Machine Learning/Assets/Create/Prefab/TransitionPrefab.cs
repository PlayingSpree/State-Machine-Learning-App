using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPrefab : MonoBehaviour
{
    // Ref
    public GameObject arrow;
    public LineRenderer lineRenderer;
    public TMPro.TMP_Text text;
    public Collider2D coll;
    public List<StateMachineData.Transition> transitions = new List<StateMachineData.Transition>();

    public GameObject normalTransitionArrow;
    public GameObject selfTransitionArrow;
    public Collider2D selfColl;
    public TMPro.TMP_Text selfText;
    public SpriteRenderer selfArrow;
    public SpriteRenderer selfLine;

    public void Set(Vector2 start, Vector2 end, string s, StateMachineData.Transition t)
    {
        transitions.Add(t);

        // Self Transition?
        if (t.from == t.to)
        {
            lineRenderer.enabled = false;
            selfTransitionArrow.SetActive(true);
            normalTransitionArrow.SetActive(false);

            transform.position = start;
            // set ref
            coll = selfColl;
            text = selfText;
        }
        else
        {
            // Line Pos
            Vector2 e = Vector2.MoveTowards(end, start, Appdata.circleSize + (Appdata.arrowSize / 2f));
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new Vector3[] { start, e });
            lineRenderer.sortingOrder = -10;

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
            ((BoxCollider2D)coll).size = new Vector2(Vector2.Distance(start, end) - (Appdata.circleSize * 2f), 0.35f);
            coll.offset = new Vector2(((BoxCollider2D)coll).size.x / 2f, 0f);

            // Coll Rot
            coll.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, textRot);
        }

        text.SetText(s);
    }
    public void AddText(string s, StateMachineData.Transition t)
    {
        transitions.Add(t);
        text.SetText(string.Join(",", text.text, s));
    }
    public void SetColor(Color c)
    {
        if (transitions[0].from == transitions[0].to)
        {
            selfArrow.color = c;
            selfLine.color = c;
            text.color = c;
        }
        else
        {
            arrow.GetComponent<SpriteRenderer>().color = c;
            text.color = c;
            Gradient g = new Gradient();
            g.SetKeys(new GradientColorKey[] { new GradientColorKey(c, 0f), new GradientColorKey(c, 1f) }, lineRenderer.colorGradient.alphaKeys);
            lineRenderer.colorGradient = g;
        }
    }
}
