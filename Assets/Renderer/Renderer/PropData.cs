using UnityEngine;
using System.Collections.Generic;
using Extinction.Config;

namespace Extinction.Renderer
{
    public class PropData
    {
        public Vector3 position;
        public GameObject prefab;

        public static List<PropData> LoadDataAt(Vector3 chunkPosition, int chunkSize)
        {
            List<PropData> dataList = new List<PropData>();
            World config = WorldRenderer.Config();

            System.Random random = new System.Random();

            for (float z = -chunkSize - 0.5f; z <= chunkSize - 0.5f; z++)
                for (float x = -chunkSize - 0.5f; x <= chunkSize - 0.5f; x++)
                {
                    Vector3 position = chunkPosition + new Vector3(x, 20, z);

                    if (config.HasPropAt(position.x, position.z))
                    {
                        PropData propData = new PropData();
                        propData.position = position;
                        propData.position.y = config.GetHeight(position.x, position.z) + config.propVerticalOffset;
                        propData.position.x += 0.5f;
                        propData.position.z += 0.5f + (float)random.Next(0, 100) / 1000;
                        propData.prefab = config.GetProp(position.x, position.z);
                        dataList.Add(propData);
                    }
                }

            return dataList;
        }
    }
}