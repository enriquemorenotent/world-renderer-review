using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Extinction.Config;
using Extinction.Utils;

namespace Extinction.Renderer
{
    [AddComponentMenu("Extinction/World renderer")]
    [RequireComponent(typeof(DistanceDetector))]
    public class WorldRenderer : MonoBehaviour
    {
        // Fields

        [SerializeField] public Pool chunkPool;
        [SerializeField] public Pool navMeshLinkPool;

        [SerializeField] public World config;

        [Range(2, 20)] public int radius = 2;

        [Range(2, 30)] public int chunkSize = 10;

        [Range(2, 20)] public int cacheRadius = 5;

        [Range(300, 2000)] public int visitedChunkBufferRange = 500;

        public bool renderProps = true;

        public bool renderNavMesh = true;

        // Components

        DistanceDetector detector;
        [SerializeField] NavMeshSurface navMeshSurface;

        // Other

        DataPreloader dataPreloader;

        public MinimapDataPreloader minimapDataPreloader;

        public Dictionary<Vector3, GameObject> renderedChunks = new Dictionary<Vector3, GameObject>();

        public List<Vector3> visitedChunks = new List<Vector3>();

        public int ChunkDiameter { get { return chunkSize * 2 + 1; } }

        bool navMeshDirty;

        // Singleton

        public static WorldRenderer singleton { get; private set; }

        public static World Config() => singleton.config;

        public static Cache<Vector3, ChunkData> GetChunkData() => singleton.dataPreloader.chunkDataCache;

        // Unity methods

        void Awake()
        {
            singleton = this;
            config.Setup();
            dataPreloader = new DataPreloader(radius + cacheRadius, chunkSize, Vector3.zero);
            minimapDataPreloader = new MinimapDataPreloader();
            UpdateRenderPoint(Vector3.zero);

            detector = GetComponent<DistanceDetector>();
        }

        void Update()
        {
            if (detector.IsTargetTooFar())
                UpdateRenderPoint(detector.TargetPosition());

            if (renderNavMesh && navMeshDirty && AreAllChunksRendered())
            {
                SetNavMeshLinks();
                navMeshSurface.BuildNavMeshAsync();
                navMeshDirty = false;
            }
        }

        void SetNavMeshLinks()
        {
            var chunkDiameter = chunkSize * 2 + 1;
            var mapRadius = radius * ChunkDiameter + chunkSize + 1;

            for (float z = -mapRadius + 0.5f; z < mapRadius - 0.5f; z++)
                for (float x = -mapRadius + 0.5f; x < mapRadius - 0.5f; x++)
                {
                    if (config.GetHeight(x, z) != config.GetHeight(x + 1, z))
                    {
                        GameObject linkGameObject = navMeshLinkPool.Deliver();
                        linkGameObject.name = $"NavMeshLink - ({x}, {z})";

                        NavMeshLink link = linkGameObject.GetComponent<NavMeshLink>();
                        link.startPoint = new Vector3(x + 0.5f, config.GetHeight(x, z), z + 0.5f);
                        link.endPoint = new Vector3(x + 1.5f, config.GetHeight(x + 1, z), z + 0.5f);
                    }

                    if (config.GetHeight(x, z) != config.GetHeight(x, z + 1))
                    {
                        GameObject linkGameObject = navMeshLinkPool.Deliver();
                        linkGameObject.name = $"NavMeshLink - ({x}, {z})";

                        NavMeshLink link = linkGameObject.GetComponent<NavMeshLink>();
                        link.startPoint = new Vector3(x + 0.5f, config.GetHeight(x, z), z + 0.5f);
                        link.endPoint = new Vector3(x + 0.5f, config.GetHeight(x, z + 1), z + 1.5f);
                    }
                }
        }

        // Helpers

        Vector3 ClosestRenderPoint(Vector3 position) => new Vector3Int(
            (int)(position.x / ChunkDiameter) * ChunkDiameter,
            0,
            (int)(position.z / ChunkDiameter) * ChunkDiameter
        );

        bool AreAllChunksRendered()
        {
            return renderedChunks.All(pair => pair.Value.GetComponent<ChunkRenderer>().IsRendered());
        }

        //
        // Chunk control
        //

        void DeleteDistantChunks()
        {
            var distantChunks = renderedChunks.Where(pair =>
                Mathf.Abs(pair.Key.x - transform.position.x) / ChunkDiameter > radius ||
                Mathf.Abs(pair.Key.z - transform.position.z) / ChunkDiameter > radius
            ).ToList();

            foreach (var pair in distantChunks)
            {
                pair.Value.GetComponent<ChunkRenderer>().ToPool();
                renderedChunks.Remove(pair.Key);
            }

            visitedChunks.RemoveAll(position => Vector3.Distance(transform.position, position) > visitedChunkBufferRange);
        }

        bool isChunkRenderedAt(Vector3 position) => renderedChunks.ContainsKey(position);

        void TrackChunk(GameObject chunkInstance, Vector3 position)
        {
            renderedChunks.Add(position, chunkInstance);
            if (!visitedChunks.Contains(position)) visitedChunks.Add(position);
        }

        void InstantiateChunk(int x, int z)
        {
            Vector3 position = new Vector3(transform.position.x + x * ChunkDiameter, 0, transform.position.z + z * ChunkDiameter);

            if (isChunkRenderedAt(position)) return;

            GameObject chunkInstance = chunkPool.Deliver();
            chunkInstance.transform.position = position;
            chunkInstance.name = string.Format("Chunk {0}, {1}", position.x, position.z);

            // Do NOT do this. It will mess the positions of the chunks when moving around
            // chunkInstance.transform.SetParent(this.transform);

            TrackChunk(chunkInstance, position);
        }

        void UpdateRenderPoint(Vector3 point)
        {
            Vector3 drawPoint = ClosestRenderPoint(point);
            transform.position = drawPoint;
            dataPreloader.LoadAround(transform.position);
            // minimapDataPreloader.LookAround(transform.position);
            onRenderPointUpdated.Invoke();
            DeleteDistantChunks();

            for (int z = -radius; z <= radius; z++)
                for (int x = -radius; x <= radius; x++)
                    InstantiateChunk(x, z);

            navMeshDirty = true;
        }

        //
        // Events
        //

        static UnityEvent onRenderPointUpdated = new UnityEvent();

        public static void OnRenderPointUpdated(UnityAction action)
        {
            onRenderPointUpdated.AddListener(action);
        }
    }
}
