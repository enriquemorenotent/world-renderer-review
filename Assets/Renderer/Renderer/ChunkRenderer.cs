using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Extinction.Renderer
{
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        // Fields

        [SerializeField] bool isChunkRendered = false;

        // Components

        MeshCollider meshCollider;
        MeshFilter meshFilter;

        // Other

        List<GameObject> propsRendered;

        // Unity methods

        void Awake()
        {
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();
            propsRendered = new List<GameObject>();
        }

        void OnEnable()
        {
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.name = "Chunk mesh";
            meshCollider.sharedMesh = new Mesh();

            isChunkRendered = false;
        }

        void Update()
        {
            if (!isChunkRendered) TryRenderChunk();
        }

        // Other

        public bool IsRendered() => isChunkRendered;

        void TryRenderChunk()
        {
            ChunkData chunkData;
            if (WorldRenderer.GetChunkData().TryGetValue(transform.position, out chunkData))
            {
                RenderMesh(chunkData.meshData);
                if (WorldRenderer.singleton.renderProps)
                    RenderProps(chunkData);

                isChunkRendered = true;
            }
        }

        void RenderMesh(MeshData data)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Chunk";

            mesh.SetVertices(data.vertices);
            mesh.SetTriangles(data.triangles, 0);
            mesh.SetUVs(0, data.uvs);
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        void RenderProps(ChunkData chunkData)
        {
            foreach (PropData data in chunkData.propDataList)
            {
                GameObject instance = Instantiate(data.prefab, data.position, Quaternion.identity);
                instance.transform.SetParent(this.transform);
                this.propsRendered.Add(instance);
            }
        }

        public void ToPool()
        {
            foreach (GameObject prop in this.propsRendered) Destroy(prop);
            WorldRenderer.singleton.chunkPool.Return(this.gameObject);
        }
    }
}