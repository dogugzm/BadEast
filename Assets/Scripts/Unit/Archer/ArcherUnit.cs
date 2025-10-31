using Core;

namespace Unit.Archer
{
    public class ArcherUnit : Unit, IProduct<ArcherUnit>
    {
        public void Initialize(ObjectPool<ArcherUnit> pool)
        {
        }

        public void Decommission()
        {
            gameObject.SetActive(false);
        }
    }
}