using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Vector3 selectionRingOffset;
    [SerializeField] private Vector3 selectionRingSize;
    [SerializeField] private MeshRenderer meshRenderer;

    private GameObject selectionRing;
    private Vector3 originalRingOffset;
    private Material selectionMaterial;

    private bool highlightVisible;
    private bool ringVisible;

    private UnitController unitController;
    private PlayerController playerController;
    private BuildingController buildingController;
    private ResourceController resourceController;

    public UnitController Unit => unitController;
    public BuildingController Building => buildingController;
    public ResourceController Resource => resourceController;
    public PlayerController Owner => playerController;

    public bool IsMine => Owner.IsPlayerControlled;
    public bool RingVisible
    {
        get => ringVisible;
        set
        {
            ringVisible = value;
            UpdateSelectionRing(value);
        }
    }

    public bool HighlightVisible
    {
        get => highlightVisible;
        set
        {
            highlightVisible = value;
            UpdateSelectionHighlight(value);
        }
    }

    private void Awake()
    {
        if (!playerController) playerController = transform.root.GetComponent<PlayerController>();
        if (!unitController) unitController = GetComponent<UnitController>();
        if (!resourceController) resourceController = GetComponent<ResourceController>();
        if (!buildingController) buildingController = GetComponent<BuildingController>();
        // if (!buildingController) 
        if (!spawnManager) spawnManager = FindObjectOfType<SpawnManager>();
        if (!meshRenderer) meshRenderer = GetComponent<MeshRenderer>();
        var materials = meshRenderer.materials;
        if (materials.Length > 0)
        {
            selectionMaterial = materials[materials.Length - 1];
        }
    }
    private void UpdateSelectionRing(bool value)
    {
        if (!EnsureHighlightRing()) return;
        selectionRing.transform.localScale = selectionRingSize != Vector3.zero ? selectionRingSize : selectionRing.transform.localScale;
        selectionRing.transform.localPosition = originalRingOffset + selectionRingOffset;
        selectionRing.SetActive(value);
    }

    private bool EnsureHighlightRing()
    {
        if (selectionRing) return true;
        selectionRing = spawnManager.SpawnHighlightRing(transform);
        originalRingOffset = selectionRing.transform.localPosition;
        return selectionRing;
    }

    private void UpdateSelectionHighlight(bool value)
    {
        if (!selectionMaterial) return;
        selectionMaterial.SetFloat("VisibilityValue", Owner.IsPlayerControlled && value ? 1 : 0);

        //if (!Owner.IsPlayerControlled)
        //{
        //    selectionMaterial.SetColor("Color_1053FFD8", Color.red);
        //}
    }
}
