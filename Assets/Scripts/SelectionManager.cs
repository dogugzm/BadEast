using System;
using Army;
using JetBrains.Annotations;
using Lean.Touch;
using Unit;
using UnityEngine;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class SelectionManager : IInitializable
    {
        [CanBeNull] public IUnit SelectedUnit { get; private set; }
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


            if (hit.collider.TryGetComponent(out IUnit unit))
            {
                SelectedUnit = unit;
            }


            if (hit.collider.CompareTag(GroundTagName) && SelectedUnit != null)
            {
                if (SelectedUnit.transform.TryGetComponent(out UnitMovement unitMovement))
                {
                    unitMovement.SetTarget(hit.point);
                    SelectedUnit = null;
                }
            }


            if (!hit.collider.TryGetComponent(out ISelectable selectable)) return;

            if (selectable.IsSelected)
            {
                selectable.Deselect();
            }
            else
            {
                selectable.Select();
            }
        }
    }
}