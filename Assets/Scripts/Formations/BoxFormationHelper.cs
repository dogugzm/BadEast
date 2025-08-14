using UnityEngine;

namespace Formations
{
    public static class BoxFormationHelper
    {
        public static Vector3[] GetPositions(int count, float spacing)
        {
            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt((float)count / columns);

            // Calculate start position relative to (0, 0, 0)
            Vector3 startPos = new Vector3(-((columns - 1) * spacing / 2f), 0, -((rows - 1) * spacing / 2f));

            Vector3[] positions = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int col = i % columns;
                positions[i] = startPos + new Vector3(col * spacing, 0, row * spacing);
            }

            return positions;
        }
    }
}