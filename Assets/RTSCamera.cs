using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
public class RTSCamera : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 15f;
    [SerializeField] private float deadband = 8f;
    [SerializeField] private MarqueeSelection marqueeSelection;
    [SerializeField] private Vector3 marqueeSelectionOffset = Vector3.up;
    [SerializeField] private SpawnManager spawnManager;

    public bool MouseMovementEnabled = false;

    private bool selecting;
    private Camera camera;
    private Vector3 selectionStartScreen;
    private Vector3 selectionStartWorld;
    private List<SelectableObject> activeSelection;
    private ClickMarker clickMarker;

    private readonly UnitFormation[] formations = new[]
    {
        new GridUnitFormation()
    };

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCameraByKeyboard();
        MoveCameraByMouse();

        HandleMouseSelection();
        HandleMouseActions();
    }

    private void HandleMouseActions()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MoveUnits();
        }
    }

    private void MoveUnits()
    {
        if (activeSelection == null || activeSelection.Count == 0)
        {
            return;
        }

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 200))
        {
            var target = hit.collider.GetComponent<SelectableObject>();
            var isMoving = true;
            var position = hit.point;
            var points = formations[0].CalculateFormationPoints(position, activeSelection.Count).ToList();

            for (int i = 0; i < activeSelection.Count; i++)
            {
                var selection = activeSelection[i];
                var targetPoint = points.OrderBy(x => (x - selection.transform.position).sqrMagnitude).FirstOrDefault();
                points.Remove(targetPoint);

                if (selection.Unit)
                {
                    if (target)
                    {
                        isMoving = false;

                        if (target.Resource && selection.Unit is WorkerUnit worker)
                        {
                            worker.GatherResources(target.Resource);
                        }


                        // do something based on object type.
                    }
                    else
                    {
                        if (selection.Unit is WorkerUnit worker)
                        {
                            worker.GatherResources(null);
                        }

                        selection.Unit.MoveTo(targetPoint);
                    }
                }
            }

            if (isMoving)
            {
                DisplayMouseMarker(position);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DisplayMouseMarker(Vector3 position)
    {
        if (!clickMarker) clickMarker = spawnManager.SpawnClickMarker();
        clickMarker.Show(position);
    }

    private void HandleMouseSelection()
    {
        if (selecting)
        {
            DrawSelection();
        }

        if (Input.GetMouseButtonDown(0))
        {
            BeginSelection();
        }

        if (selecting && Input.GetMouseButtonUp(0))
        {
            EndSelection();
        }
    }

    private void DrawSelection()
    {
        if (!marqueeSelection)
        {
            return;
        }

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 1000))
        {
            return;
        }

        var currentWorldPoint = hit.point;
        var delta = currentWorldPoint - selectionStartWorld;
        var center = Vector3.Lerp(selectionStartWorld, currentWorldPoint, 0.5F);


        marqueeSelection.transform.position = center + marqueeSelectionOffset;
        marqueeSelection.transform.localScale =
            new Vector3(Mathf.Abs(delta.x), Mathf.Max(0.1f, Mathf.Abs(delta.y)), Mathf.Abs(delta.z));
    }

    private void BeginSelection()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 1000))
        {
            Debug.Log("Bitch lasagna you didnt hit anything");
            return;
        }

        if (activeSelection != null && activeSelection.Count > 0)
        {
            activeSelection.ForEach(x => x.RingVisible = false);
        }

        selectionStartScreen = Input.mousePosition;
        selectionStartWorld = hit.point;
        selecting = true;
        marqueeSelection.gameObject.SetActive(true);

        Debug.Log("Begin selection");
    }

    private void EndSelection()
    {
        var selection = new List<SelectableObject>();
        try
        {
            var dist = (selectionStartScreen - Input.mousePosition).sqrMagnitude;
            if (dist < 100)
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);

                foreach (var hit in Physics.RaycastAll(ray, 1000))
                {
                    var selectableObject = hit.transform.GetComponent<SelectableObject>();
                    if (selectableObject)
                    {
                        TrySelectObject(selection, selectableObject);
                        return;
                    }
                }
            }

            foreach (var selectedObject in marqueeSelection.SelectedObjects)
            {
                TrySelectObject(selection, selectedObject);
            }
        }
        finally
        {
            activeSelection = selection;
            marqueeSelection.gameObject.SetActive(false);
            marqueeSelection.transform.localScale = Vector3.zero;
            selecting = false;
        }
    }

    private static void TrySelectObject(List<SelectableObject> selection, SelectableObject selectedObject)
    {
        if (selectedObject && selectedObject.Owner.IsPlayerControlled)
        {
            Debug.Log("Selected: " + selectedObject.name);
            selectedObject.RingVisible = true;
            selection.Add(selectedObject);
        }
    }

    private void MoveCameraByMouse()
    {
        if (!MouseMovementEnabled) return;

        var mousePosition = Input.mousePosition;
        if (mousePosition.x <= deadband)
            transform.position += Vector3.left * movementSpeed * Time.deltaTime;

        if (mousePosition.x >= Screen.width - deadband)
            transform.position += Vector3.right * movementSpeed * Time.deltaTime;

        if (mousePosition.y <= deadband)
            transform.position += Vector3.back * movementSpeed * Time.deltaTime;

        if (mousePosition.y >= Screen.height - deadband)
            transform.position += Vector3.forward * movementSpeed * Time.deltaTime;
    }

    private void MoveCameraByKeyboard()
    {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        transform.position += direction * movementSpeed * Time.deltaTime;
    }
}

public class GridUnitFormation : UnitFormation
{
    public override Vector3[] CalculateFormationPoints(Vector3 origin, int count)
    {
        var points = new Vector3[count];
        var gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));

        //var halfWidth = -((gridSize * Vector3.right * 3) / 2f);
        //var halfHeight = -((gridSize * Vector3.forward * 3) / 2f);

        for (var i = 0; i < count; ++i)
        {
            var x = i % gridSize;
            var z = i / gridSize;

            var xOffset = x * Vector3.right * 3;
            var zOffset = z * Vector3.forward * 3;

            points[i] = origin + xOffset + zOffset;
        }

        return points;
    }
}

public abstract class UnitFormation
{
    public abstract Vector3[] CalculateFormationPoints(Vector3 origin, int count);
}
