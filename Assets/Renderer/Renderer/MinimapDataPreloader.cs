using System.Threading.Tasks;
using UnityEngine;

namespace Extinction.Renderer
{
    public class MinimapDataPreloader
    {
        int mapSize = 2000;

        Vector3 centerPoint;

        public byte[] data;

        public bool loading = false;

        object lockObject = new object();

        int PixelSize () => mapSize * mapSize;

        int DataSize () => PixelSize() * 4;

        public MinimapDataPreloader()
        {
            // LookAround(Vector3.zero);
        }

        public void LookAround(Vector3 _centerPoint)
        {
            loading = true;
            centerPoint = _centerPoint;
            Task.Run(UpdateData);
        }

        public void UpdateData()
        {
            Debug.Log("Updating minimap data");

            int index;

            lock (lockObject)
            {
                byte[] _data = new byte[DataSize()];

                for (int i = 0; i < PixelSize(); i++)
                {
                    _data[i * 4 + 3] = 255;
                }

                foreach (Vector3 position in WorldRenderer.singleton.visitedChunks)
                {
                    // Debug.Log(position);
                    Vector2 drawCenterPoint =
                        Vector2.one * (mapSize / 2)
                        - new Vector2(centerPoint.x, centerPoint.z)
                        + new Vector2(position.x, position.z);

                    int size = WorldRenderer.singleton.chunkSize;

                    for (float x = -size; x <= size; x++)
                    for (float y = -size; y <= size; y++)
                    {
                        Color color = WorldRenderer.Config().GetTerrainColor(position.x + x, position.z + y);
                        index = mapSize * (int) (drawCenterPoint.y + y);
                        index += (int) (drawCenterPoint.x + x);
                        index *= 4;
                        _data[index + 0] =  (byte) (color.r *  255);
                        _data[index + 1] =  (byte) (color.g *  255);
                        _data[index + 2] =  (byte) (color.b *  255);
                    }
                }

                data = _data;

                loading = false;
            }
            Debug.Log("minimap data UPDATED");
       }
    }
}