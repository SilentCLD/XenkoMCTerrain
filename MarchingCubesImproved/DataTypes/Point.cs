using Xenko.Core.Mathematics;

namespace MarchingCubesImproved
{
    public struct Point
    {
        public Vector3 localPosition;
        public float density;

        public Point(Vector3 localPosition, float density)
        {
            this.localPosition = localPosition;
            this.density = density;
        }
    }
}