using Xenko.Core.Mathematics;

namespace MarchingCubesImproved
{
    public struct Point
    {
        public Vector3 LocalPosition;
        public float Density;

        public Point(Vector3 localPosition, float density)
        {
            this.LocalPosition = localPosition;
            this.Density = density;
        }
    }
}