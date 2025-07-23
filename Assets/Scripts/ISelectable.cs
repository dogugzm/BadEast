using System;

public interface ISelectable
{
    bool IsSelected { get; }
    void Select();
    void Deselect();
    Action<ISelectable> OnSelected { get; set; }
    Action<ISelectable> OnDeselected { get; set; }
}