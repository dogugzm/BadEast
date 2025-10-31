using Core;

namespace Unit.Spearman
{
    public class SpearmanUnit : Unit, IProduct<SpearmanUnit>
    {
        public void Initialize(ObjectPool<SpearmanUnit> pool)
        {
        }

        public void Decommission()
        {
            gameObject.SetActive(false);
        }
    }
}