using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extinction.Config;

namespace Extinction.Renderer
{
    public class MeshData
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> triangles = new List<int>();
        public List<Vector2> uvs = new List<Vector2>();

        public static MeshData LoadDataAt(Vector3 position, int chunkSize)
        {
            MeshData data = new MeshData();
            World config = WorldRenderer.singleton.config;

            int offset = 0;
            for (float z = -chunkSize - 0.5f; z <= chunkSize - 0.5f; z++)
                for (float x = -chunkSize - 0.5f; x <= chunkSize - 0.5f; x++)
                {
                    var height = config.GetHeight(position.x + x, position.z + z);
                    var upperHeight = config.GetHeight(position.x + x, position.z + z + 1);
                    var rightHeight = config.GetHeight(position.x + x + 1, position.z + z);

                    List<TileID> tileIdList = config
                        .GetTileIDs(position.x + x, position.z + z)
                        .OrderBy(tile => tile.terrainID.biome * 10 + tile.terrainID.terrain).ToList();

                    foreach (TileID tileID in tileIdList)
                    {
                        data.vertices.Add(new Vector3(x + 0, height, z + 0));
                        data.vertices.Add(new Vector3(x + 1, height, z + 0));
                        data.vertices.Add(new Vector3(x + 0, height, z + 1));
                        data.vertices.Add(new Vector3(x + 1, height, z + 1));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 3);
                        offset += 4;

                        List<Vector2> uvs = config.GetUVsFor(tileID);
                        data.uvs.AddRange(uvs);
                    }

                    while (upperHeight > height)
                    {
                        data.vertices.Add(new Vector3(x + 0, upperHeight - 1, z + 1));
                        data.vertices.Add(new Vector3(x + 1, upperHeight - 1, z + 1));
                        data.vertices.Add(new Vector3(x + 0, upperHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 1, upperHeight + 0, z + 1));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 3);
                        offset += 4;

                        data.uvs.AddRange(config.GetUVsFor(0, 28));

                        upperHeight--;
                    }

                    while (upperHeight < height)
                    {
                        data.vertices.Add(new Vector3(x + 0, upperHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 1, upperHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 0, upperHeight + 1, z + 1));
                        data.vertices.Add(new Vector3(x + 1, upperHeight + 1, z + 1));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 3);
                        data.triangles.Add(offset + 2);
                        offset += 4;

                        data.uvs.AddRange(config.GetUVsFor(0, 28));

                        upperHeight++;
                    }

                    while (rightHeight > height)
                    {
                        data.vertices.Add(new Vector3(x + 1, rightHeight - 1, z + 1));
                        data.vertices.Add(new Vector3(x + 1, rightHeight - 1, z + 0));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 0, z + 0));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 3);
                        offset += 4;

                        data.uvs.AddRange(config.GetUVsFor(0, 28));

                        rightHeight--;
                    }

                    while (rightHeight < height)
                    {
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 0, z + 0));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 0, z + 1));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 1, z + 0));
                        data.vertices.Add(new Vector3(x + 1, rightHeight + 1, z + 1));

                        data.triangles.Add(offset + 0);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 1);
                        data.triangles.Add(offset + 2);
                        data.triangles.Add(offset + 3);
                        offset += 4;

                        data.uvs.AddRange(config.GetUVsFor(0, 28));

                        rightHeight++;
                    }
                }

            return data;
        }
    }
}