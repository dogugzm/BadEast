using JetBrains.Annotations;
using Lean.Touch;
using Unit;
using UnityEngine;
using VContainer.Unity;

public class SelectionManager : IInitializable
{
    private readonly GridManager _gridManager;

    public SelectionManager(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    [CanBeNull] private IUnit SelectedUnit { get; set; }
    private const string GroundTagName = "Ground";

    public void Initialize()
    {
        LeanTouch.OnFingerTap += OnFingerTap;
    }

    private void OnFingerTap(LeanFinger finger)
    {
        if (finger.IsOverGui) return;

        var ray = Camera.main.ScreenPointToRay(finger.ScreenPosition);
        if (!Physics.Raycast(ray, out var hit)) return;

        if (hit.collider.CompareTag(GroundTagName) && SelectedUnit != null)
        {
            //TODO: move to grid

            var gridCell = _gridManager.GetNearestWalkableGridCell(hit.point);
            if (gridCell != null)
            {
                if (SelectedUnit.transform.TryGetComponent(out IUnitMovement unitMovement))
                {
                    unitMovement.SetTarget(gridCell.Value.worldPosition);
                    SelectedUnit = null;
                }
            }

            return;
        }


        if (!hit.collider.TryGetComponent(out ISelectable selectable)) return;

        if (hit.collider.TryGetComponent(out IUnit unit))
        {
            if (!selectable.IsSelected)
            {
                if (SelectedUnit is not null)
                {
                    SelectedUnit.transform.TryGetComponent(out ISelectable selectableUnit);
                    if (selectableUnit != null)
                    {
                        selectableUnit.Deselect();
                    }
                }

                SelectedUnit = unit;
                selectable.Select();
            }
            else
            {
                selectable.Deselect();
            }
        }
    }
}