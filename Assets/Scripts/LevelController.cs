using Core;
using Cysharp.Threading.Tasks;
using Unit;
using Unit.Archer;
using Unit.Spearman;
using Unit.Swordsman;
using UnityEngine;
using VContainer.Unity;

public class LevelController : IInitializable
{
    private readonly PoolManager _poolManager;
    private readonly GridManager _gridManager;

    public LevelController(PoolManager poolManager, GridManager gridManager)
    {
        _poolManager = poolManager;
        _gridManager = gridManager;
    }

    public async void Initialize()
    {
        await UniTask.WaitUntil(() => _gridManager.isReady);

        // Get a Swordsman from the pool manager
        var swordsmanUnit = _poolManager.Get<SwordsmanUnit>();
        swordsmanUnit.transform.position = _gridManager.GetRandomWalkableGridCell().worldPosition;
        swordsmanUnit.transform.rotation = Quaternion.identity;
        swordsmanUnit.Init(UnitSide.Enemy);

        // Get an Archer from the pool manager
        var archerUnit = _poolManager.Get<ArcherUnit>();
        archerUnit.transform.position = _gridManager.GetRandomWalkableGridCell().worldPosition;
        archerUnit.transform.rotation = Quaternion.identity;
        archerUnit.Init(UnitSide.Player);

        // Get a Spearman from the pool manager
        var spearmanUnit = _poolManager.Get<SpearmanUnit>();
        spearmanUnit.transform.position = _gridManager.GetRandomWalkableGridCell().worldPosition;
        spearmanUnit.transform.rotation = Quaternion.identity;
        spearmanUnit.Init(UnitSide.Player);

        // Example of how to return a unit to the pool later
        // _poolManager.Return(archerUnit);
    }
}