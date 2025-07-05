using Army;
using Lean.Touch;
using UnityEngine;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class SelectionManager : IInitializable
    {
        public void Initialize()
        {
            LeanTouch.OnFingerTap += OnFingerTap;
        }

        private void OnFingerTap(LeanFinger finger)
        {
            if (finger.IsOverGui) return;

            var ray = Camera.main.ScreenPointToRay(finger.ScreenPosition);

            if (!Physics.Raycast(ray, out var hit)) return;
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