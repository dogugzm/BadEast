using Army;
using UnityEngine;

namespace Unit
{
    public interface IUnit
    {
        Transform transform { get; }
        // Define any unit-specific methods or properties here
    }

    public class Unit : MonoBehaviour, IUnit
    {
    }
}