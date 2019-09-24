using System.Collections.Generic;
using UnityEngine;

namespace Extinction.Renderer
{
    public class ChunkData
    {
        public MeshData meshData;
        public List<PropData> propDataList;

        public static ChunkData LoadDataAt(Vector3 position, int chunkSize)
        {
            ChunkData data = new ChunkData();

            data.meshData = MeshData.LoadDataAt(position, chunkSize);
            data.propDataList = PropData.LoadDataAt(position, chunkSize);

            return data;
        }
    }
}