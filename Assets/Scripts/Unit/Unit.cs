using System;
using UnityEngine;
using UnityEngine.Serialization;

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
        void Init(UnitSide side);
    }

    public enum CombatStatus
    {
        None = 0,
        InCombat = 1,
        OutOfCombat = 2
    }

    public class Unit : MonoBehaviour, IUnit
    {
        [Serializable]
        public class UnitData
        {
            public UnitData(int unitSize)
            {
                UnitSize = unitSize;
            }

            [field: SerializeField] public int UnitSize { get; set; }
        }

        [SerializeField] private UnitData unitData;
        public UnitSide Side { get; private set; }

        public void Init(UnitSide side)
        {
            Side = side;
            if (!TryGetComponent(out UnitSoldierController soldierController)) return;
            soldierController.Initialize(unitData.UnitSize, side);
        }
    }
}