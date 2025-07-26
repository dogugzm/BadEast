using System;
using UnityEngine;

namespace Unit
{
    public class UnitSelectionController : MonoBehaviour, ISelectable
    {
        public bool IsSelected { get; private set; }
        public Action<ISelectable> OnSelected { get; set; }
        public Action<ISelectable> OnDeselected { get; set; }

        public void Select()
        {
            IsSelected = true;
            Debug.Log(gameObject.name + " selected.");
            OnSelected?.Invoke(this);
        }

        public void Deselect()
        {
            IsSelected = false;
            Debug.Log(gameObject.name + " deselected.");
            OnDeselected?.Invoke(this);
        }

    }
}