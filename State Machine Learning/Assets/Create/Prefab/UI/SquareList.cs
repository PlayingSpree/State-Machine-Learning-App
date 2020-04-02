using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquareList : MonoBehaviour
{
    public GameObject squareListPrefab;
    public Transform listParrent;

    public Dictionary<GameObject, object> buttons = new Dictionary<GameObject, object>();

    public void SetButtons(Dictionary<string, object> list, UnityEngine.Events.UnityAction action)
    {
        // Clear
        foreach (KeyValuePair<GameObject, object> item in buttons)
        {
            Destroy(item.Key);
        }
        buttons.Clear();

        // Create New
        foreach (KeyValuePair<string, object> item in list)
        {
            GameObject g = Instantiate(squareListPrefab, listParrent);
            g.GetComponentInChildren<TMPro.TMP_Text>().SetText(item.Key);
            g.GetComponentInChildren<Button>().onClick.AddListener(action);
            buttons.Add(g, item.Value);
        }
    }
}
