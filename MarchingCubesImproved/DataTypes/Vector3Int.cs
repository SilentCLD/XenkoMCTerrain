using System.Runtime.Serialization;
using Xenko.Core.Mathematics;

namespace MarchingCubesImproved
{
    public struct Vector3Int
    {
        [DataMember] public int X, Y, Z;

        public Vector3Int(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3Int Subtract(Vector3Int pos)
        {
            return new Vector3Int(X - pos.X, Y - pos.Y, Z - pos.Z);
        }

        public static Vector3Int operator -(Vector3Int pos1, Vector3Int pos2)
        {
            return pos1.Subtract(pos2);
        }

        public static implicit operator Vector3(Vector3Int pos)
        {
            return new Vector3(pos.X, pos.Y, pos.Z);
        }

        public static implicit operator Vector3Int(Vector3 pos)
        {
            Vector3Int vector3Int = new Vector3Int(
                MathHelpers.Round(pos.X),
                MathHelpers.Round(pos.Y),
                MathHelpers.Round(pos.Z)
            );

            return vector3Int;
        }
    }
}