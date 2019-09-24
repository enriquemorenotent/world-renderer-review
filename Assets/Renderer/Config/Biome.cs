using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extinction.Utils;

namespace Extinction.Config
{
    [CreateAssetMenu(fileName = "Biome", menuName = "Extinction/Config/Biome", order = 2)]
    public class Biome : ScriptableObject
    {
        // Fields

        [Range(1f, 200.0f)] public float propDistributionScale = 10f;
        public List<Extinction.Data.Terrain> terrains;
        public List<Extinction.Data.WeightedProp> props;

        // Attributes

        Noise propDistribution;

        // Properties

        int TotalPropWeight => this.props.Aggregate(0, (accum, item) => accum + item.weight);

        // Methods

        GameObject GetPropForWeight(int index)
        {
            foreach (var prop in this.props)
            {
                if (prop.weight > index) return prop.prefab;
                index -= prop.weight;
            }

            return this.props[0].prefab;
        }

        public GameObject GetProp(float x, float z)
        {
            if (propDistribution == null)
                propDistribution = new Noise(propDistributionScale, TotalPropWeight - 1, 666);

            return GetPropForWeight(propDistribution.At(x, z));
        }
    }
}
