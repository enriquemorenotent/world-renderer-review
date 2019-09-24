using System.Collections.Generic;
using UnityEngine;

namespace Extinction.Utils
{
    public class Pool : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        Queue<GameObject> queue = new Queue<GameObject>();

        public int instancesCreated = 0;
        public int instancesDelivered = 0;
        public int instancesReturned = 0;

        void Start() => GrowPool();

        void GrowPool()
        {
            for (int i = 0; i < 10; i++)
            {
                var instance = Instantiate(prefab);
                instance.transform.SetParent(transform);
                Return(instance);
            }
            instancesCreated += 10;
        }

        public void Return(GameObject instance)
        {
            instance.SetActive(false);
            queue.Enqueue(instance);
            instancesReturned++;
        }

        public GameObject Deliver()
        {
            if (queue.Count == 0) GrowPool();

            instancesDelivered++;
            GameObject instance = queue.Dequeue();
            instance.SetActive(true);
            return instance;
        }
    }
}
