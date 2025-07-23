using UnityEngine;

namespace Unit
{
    public enum UnitSide
    {
        Player = 0,
        Enemy = 1
    }

    public interface IUnit
    {
        Transform transform { get; }
        UnitSide Side { get; }
    }

    public class Unit : MonoBehaviour, IUnit
    {
        public UnitSide Side { get; set; } = UnitSide.Player;
        
        
    }
}