using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Extinction.Utils;

namespace Extinction.Renderer
{
    public class DataPreloader
    {
        // Attributes

        public Cache<Vector3, ChunkData> chunkDataCache;

        int loadRadius;
        int chunkSize;

        Vector3 dataPoint;

        // Constructor

        public DataPreloader(int _loadRadius, int _chunkSize, Vector3 position)
        {
            chunkSize = _chunkSize;
            loadRadius = _loadRadius;
            chunkDataCache = new Cache<Vector3, ChunkData>(LoadChunkData, CleanChunkData);
        }

        // Methods

        public ChunkData LoadChunkData(Vector3 position) => ChunkData.LoadDataAt(position, chunkSize);

        public void LoadAround(Vector3 position)
        {
            dataPoint = position;
            Task.Run(UpdateData);
        }

        void UpdateData()
        {
            chunkDataCache.CleanUp();
            CollectData();
        }

        bool CleanChunkData(Vector3 position)
        {
            bool toDelete =
                Mathf.Abs(position.x - dataPoint.x) > loadRadius * (chunkSize * 2 + 1) ||
                Mathf.Abs(position.z - dataPoint.z) > loadRadius * (chunkSize * 2 + 1);

            return toDelete;
        }

        void CollectData()
        {
            int diameter = chunkSize * 2 + 1;
            Vector3 position;

            for (int z = -loadRadius; z <= loadRadius; z++)
                for (int x = -loadRadius; x <= loadRadius; x++)
                {
                    position = new Vector3(dataPoint.x + x * diameter, 0, dataPoint.z + z * diameter);
                    chunkDataCache.At(position);
                }
        }
    }
}