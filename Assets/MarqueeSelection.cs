using System.Collections.Generic;
using UnityEngine;

public class MarqueeSelection : MonoBehaviour
{
    private readonly List<SelectableObject> selectedObjects = new List<SelectableObject>();
    public IReadOnlyList<SelectableObject> SelectedObjects => selectedObjects;

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<SelectableObject>();
        if (!obj)
        {
            return;
        }

        obj.HighlightVisible = true;
        selectedObjects.Add(obj);
        //Debug.Log("Marquee selection enter: " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<SelectableObject>();
        if (!obj)
        {
            return;
        }

        obj.HighlightVisible = false;
        selectedObjects.Remove(obj);
        //Debug.Log("Marquee selection exit: " + other.name);
    }

    private void OnDisable()
    {
        selectedObjects.ForEach(x => x.HighlightVisible = false);
        selectedObjects.Clear();
    }
}
