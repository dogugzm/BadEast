using System;
using UnityEngine;

namespace Unit.Soldier

{
    public interface ISoldier
    {
        Transform transform { get; }
        UnitSide Side { get; }
        bool IsInitialized { get; }
    }

    public class Soldier : MonoBehaviour, ISoldier
    {
        public UnitSide Side { get; private set; }
        public bool IsInitialized { get; private set; }

        public void Init(UnitSide side)
        {
            Side = side;
            IsInitialized = true;
        }
    }
}