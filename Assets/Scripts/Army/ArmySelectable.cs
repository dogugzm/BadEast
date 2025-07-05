using System;
using UnityEngine;

namespace Army
{
    public interface ISelectable
    {
        bool IsSelected { get; }
        void Select();
        void Deselect();
        Action<ISelectable> OnSelected { get; set; }
        Action<ISelectable> OnDeselected { get; set; }
    }

    public class ArmySelectable : MonoBehaviour, ISelectable
    {
        public bool IsSelected { get; private set; }

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

        public Action<ISelectable> OnSelected { get; set; }
        public Action<ISelectable> OnDeselected { get; set; }
    }
}