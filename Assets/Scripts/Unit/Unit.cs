using System;
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

    public enum CombatStatus
    {
        None = 0,
        InCombat = 1,
        OutOfCombat = 2
    }

    public class Unit : MonoBehaviour, IUnit
    {
        [field: SerializeField] public UnitSide Side { get; protected set; }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!TryGetComponent(out UnitSoldierController soldierController)) return;
            soldierController.Initialize();
        }
    }
}