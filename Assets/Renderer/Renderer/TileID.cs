using System.Collections.Generic;

namespace Extinction.Renderer
{
    public class TileID
    {
        public TerrainID terrainID;
        public int nscode;

        public TileID(TerrainID _terrainID, int _nscode)
        {
            terrainID = _terrainID;
            nscode = _nscode;
        }

        public static List<TileID> CreateList(TerrainID t0, TerrainID t1, TerrainID t2, TerrainID t3)
        {
            List<TileID> list = new List<TileID>();
            TileID tt0 = new TileID(t0, 1);
            TileID tt1 = new TileID(t1, 2);
            TileID tt2 = new TileID(t2, 4);
            TileID tt3 = new TileID(t3, 8);
            bool added;


            list.Add(tt0);

            added = false;

            foreach (var item in list)
            {
                if (item.Add(tt1))
                {
                    added = true;
                }
            }

            if (!added) list.Add(tt1);




            added = false;

            foreach (var item in list)
            {
                if (item.Add(tt2))
                {
                    added = true;
                }
            }

            if (!added) list.Add(tt2);



            added = false;

            foreach (var item in list)
            {
                if (item.Add(tt3))
                {
                    added = true;
                }
            }

            if (!added) list.Add(tt3);

            return list;

        }

        public bool Add(TileID tt)
        {
            if (tt.terrainID == terrainID)
                nscode += tt.nscode;

            return (tt.terrainID == terrainID);
        }

        //
        // Overriding ==
        //

        public static bool operator ==(TileID obj1, TileID obj2)
        {
            return obj1.terrainID == obj2.terrainID && obj1.nscode == obj2.nscode;
        }

        public static bool operator !=(TileID obj1, TileID obj2)
        {
            return obj1.terrainID != obj2.terrainID || obj1.nscode == obj2.nscode;
        }

        public bool Equals(TileID other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return terrainID.Equals(other.terrainID)
                   && nscode.Equals(other.nscode);
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

            return obj.GetType() == GetType() && Equals((TileID)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = terrainID.GetHashCode();
                hashCode = (hashCode * 397) ^ nscode.GetHashCode();
                return hashCode;
            }
        }
    }
}
