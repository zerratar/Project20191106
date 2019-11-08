using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject highlightRingPrefab;
    [SerializeField] private GameObject clickMarkerPrefab;

    private ClickMarker clickMarker;

    public GameObject SpawnHighlightRing(Transform parent)
    {
        return Instantiate(highlightRingPrefab, parent);
    }

    public ClickMarker SpawnClickMarker()
    {
        if (!clickMarker) clickMarker = Instantiate(clickMarkerPrefab).GetComponent<ClickMarker>();        
        return clickMarker;
    }
}
