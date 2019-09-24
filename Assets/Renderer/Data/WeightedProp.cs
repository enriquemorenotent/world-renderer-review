using UnityEngine;

namespace Extinction.Data
{
    [System.Serializable]
    public class WeightedProp
    {
        public string name = "not-named";
        public GameObject prefab;
        public int weight = 100;
    }
}