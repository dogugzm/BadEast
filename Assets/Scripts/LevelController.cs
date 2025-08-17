using System.Numerics;
using Cysharp.Threading.Tasks;
using Unit;
using Unit.Archer;
using Unit.Spearman;
using Unit.Swordsman;
using VContainer;
using VContainer.Unity;
using Quaternion = UnityEngine.Quaternion;

public class LevelController : IInitializable
{
    private readonly ArcherUnit _archerUnitPrefab;
    private readonly SwordsmanUnit _swordsmanUnitPrefab;
    private readonly SpearmanUnit _spearmanUnitPrefab;
    private readonly IObjectResolver _objectResolver;
    private readonly GridManager _gridManager;

    public LevelController(ArcherUnit archerUnitPrefab, SwordsmanUnit swordsmanUnitPrefab,
        IObjectResolver objectResolver, GridManager gridManager, SpearmanUnit spearmanUnitPrefab)
    {
        _archerUnitPrefab = archerUnitPrefab;
        _swordsmanUnitPrefab = swordsmanUnitPrefab;
        _objectResolver = objectResolver;
        _gridManager = gridManager;
        _spearmanUnitPrefab = spearmanUnitPrefab;
    }

    public async void Initialize()
    {
        await UniTask.WaitUntil(() => _gridManager.isReady);

        var swordsmanUnit = _objectResolver.Instantiate(_swordsmanUnitPrefab.gameObject,
            _gridManager.GetRandomWalkableGridCell().worldPosition, Quaternion.identity);
        if (swordsmanUnit.TryGetComponent(out IUnit unit))
        {
            unit.Init(UnitSide.Enemy);
        }

        var archerUnit = _objectResolver.Instantiate(_archerUnitPrefab.gameObject,
            _gridManager.GetRandomWalkableGridCell().worldPosition, Quaternion.identity);
        if (archerUnit.TryGetComponent(out IUnit archerUnitComponent))
        {
            archerUnitComponent.Init(UnitSide.Player);
        }

        var spearmanUnit = _objectResolver.Instantiate(_spearmanUnitPrefab.gameObject,
            _gridManager.GetRandomWalkableGridCell().worldPosition, Quaternion.identity);
        if (spearmanUnit.TryGetComponent(out IUnit spearmanUnitComponent))
        {
            spearmanUnitComponent.Init(UnitSide.Player);
        }
    }
}