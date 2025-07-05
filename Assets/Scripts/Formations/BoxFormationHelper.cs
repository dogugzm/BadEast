using UnityEngine;

namespace Formations
{
    public static class BoxFormationHelper
    {
        public static Vector3[] GetPositions(int count, Vector3 center, float spacing, float y)
        {
            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt((float)count / columns);

            Vector3 startPos = center -
                               new Vector3((columns - 1) * spacing / 2f, 0, (rows - 1) * spacing / 2f);

            Vector3[] positions = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int col = i % columns;
                positions[i] = startPos + new Vector3(col * spacing, y, row * spacing);
            }

            return positions;
        }
    }
}