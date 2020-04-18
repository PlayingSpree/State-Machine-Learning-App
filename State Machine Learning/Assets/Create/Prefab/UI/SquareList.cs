using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquareList : MonoBehaviour
{
    public GameObject squareListPrefab;
    public Transform listParrent;

    public List<GameObject> buttons = new List<GameObject>();

    public void SetButtons(List<SquareListOptions> list, UnityEngine.Events.UnityAction<object> action)
    {
        // Clear
        foreach (GameObject item in buttons)
        {
            Destroy(item);
        }
        buttons.Clear();

        // Create New
        foreach (SquareListOptions item in list)
        {
            GameObject g = Instantiate(squareListPrefab, listParrent);
            g.GetComponentInChildren<TMPro.TMP_Text>().SetText(item.name);
            g.GetComponentInChildren<Button>().onClick.AddListener(() => action(item.obj));
            buttons.Add(g);
        }
    }

    public class SquareListOptions
    {
        public string name;
        public object obj;

        public SquareListOptions(string name, object obj)
        {
            this.name = name;
            this.obj = obj;
        }
    }
}
