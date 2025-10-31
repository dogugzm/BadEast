using Core;
using Unit.Spearman;

namespace Unit.Swordsman
{
    public class SwordsmanUnit : Unit, IProduct<SwordsmanUnit>
    {
        public void Initialize(ObjectPool<SwordsmanUnit> pool)
        {
        }

        public void Decommission()
        {
            gameObject.SetActive(false);
        }
    }
}