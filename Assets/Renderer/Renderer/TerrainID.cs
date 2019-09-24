using System;

namespace Extinction.Renderer
{
    public class TerrainID : IEquatable<TerrainID>
    {
        public int biome, terrain;

        public TerrainID(int _biome, int _terrain)
        {
            biome = _biome;
            terrain = _terrain;
        }

        //
        // Overriding ==
        //

        public static bool operator ==(TerrainID obj1, TerrainID obj2)
        {
            return obj1.biome == obj2.biome && obj1.terrain == obj2.terrain;
        }

        public static bool operator !=(TerrainID obj1, TerrainID obj2)
        {
            return obj1.biome != obj2.biome || obj1.terrain == obj2.terrain;
        }

        public bool Equals(TerrainID other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return biome.Equals(other.biome)
                   && terrain.Equals(other.terrain);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((TerrainID)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = biome.GetHashCode();
                hashCode = (hashCode * 397) ^ terrain.GetHashCode();
                return hashCode;
            }
        }
    }
}