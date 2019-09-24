using UnityEngine;

namespace Extinction.Utils
{
    public class Noise
    {
        public float scale, index, seed;

        Cache<Vector2, int> cache;

        public Noise(float scale, float index, float seed)
        {
            this.cache = new Cache<Vector2, int>(this.GenerateValue);
            this.scale = scale;
            this.index = index;
            this.seed = seed;
        }

        int GenerateValue(Vector2 position)
        {
            return Noise.GetValue(position.x, position.y, scale, index, seed);
        }

        public int At(float x, float z)
        {
            return this.cache.At(new Vector2(x, z));
        }

        #region Static methods

        public static int GetValue(Vector2 position, float scale, float index, float seed)
        {
            return GetValue(position.x, position.y, scale, index, seed);
        }

        public static int GetValue(float x, float z, float scale, float index, float seed)
        {
            float _x = seed + x / scale;
            float _z = seed + z / scale;
            float value = Mathf.PerlinNoise(_x, _z);
            float min = 0;
            float max = index + 0.9999f;
            value = Mathf.Lerp(min, max, value);
            return (int)(value);
        }

        #endregion
    }
}