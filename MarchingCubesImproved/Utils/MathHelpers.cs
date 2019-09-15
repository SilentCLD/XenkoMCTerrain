using System;
using Xenko.Core.Mathematics;
using Xenko.Engine;
using Xenko.Graphics;
using Xenko.Physics;

namespace MarchingCubesImproved
{
    public static class MathHelpers
    {
        public static float Abs(this float n)
        {
            if (n < 0)
                return -n;
            else
                return n;
        }

        public static int Abs(this int n)
        {
            return (int) Abs((float) n);
        }

        public static int Floor(this float n)
        {
            return (int) Math.Floor(n);
        }

        public static int Ceil(this float n)
        {
            return Floor(n) + 1;
        }

        public static int Round(this float n)
        {
            int floored = Floor(n);

            float diff = n - floored;
            if (diff < 0.5f)
                return floored;
            else
                return floored + 1;
        }

        public static int RoundToNearestX(this float n, int x)
        {
            return Round(n / x) * x;
        }

        public static int FloorToNearestX(this float n, int x)
        {
            return Floor(n / x) * x;
        }

        public static int CeilToNearestX(this float n, int x)
        {
            return Ceil(n / x) * x;
        }

        public static Vector3 Floor(this Vector3 n)
        {
            int x = Floor(n.X);
            int y = Floor(n.Y);
            int z = Floor(n.Z);

            return new Vector3(x, y, z);
        }

        public static Vector3 Ceil(this Vector3 n)
        {
            int x = Ceil(n.X);
            int y = Ceil(n.Y);
            int z = Ceil(n.Z);

            return new Vector3(x, y, z);
        }

        public static Vector3 Round(this Vector3 n)
        {
            int x = Round(n.X);
            int y = Round(n.Y);
            int z = Round(n.Z);

            return new Vector3(x, y, z);
        }

        public static Vector3 Abs(this Vector3 n)
        {
            int x = Abs((int) n.X);
            int y = Abs((int) n.Y);
            int z = Abs((int) n.Z);

            return new Vector3(x, y, z);
        }

        public static Vector3 RoundToNearestX(this Vector3 n, int x)
        {
            int _x = RoundToNearestX(n.X, x);
            int _y = RoundToNearestX(n.Y, x);
            int _z = RoundToNearestX(n.Z, x);

            return new Vector3(_x, _y, _z);
        }

        public static Vector3 FloorToNearestX(this Vector3 n, int x)
        {
            int _x = FloorToNearestX(n.X, x);
            int _y = FloorToNearestX(n.Y, x);
            int _z = FloorToNearestX(n.Z, x);

            return new Vector3(_x, _y, _z);
        }

        public static Vector3Int FloorToNearestX(this Vector3Int n, int x)
        {
            int _x = FloorToNearestX(n.X, x);
            int _y = FloorToNearestX(n.Y, x);
            int _z = FloorToNearestX(n.Z, x);

            return new Vector3Int(_x, _y, _z);
        }

        public static Vector3 CeilToNearestX(this Vector3 n, int x)
        {
            int _x = CeilToNearestX(n.X, x);
            int _y = CeilToNearestX(n.Y, x);
            int _z = CeilToNearestX(n.Z, x);

            return new Vector3(_x, _y, _z);
        }

        public static Vector3 Mod(this Vector3 n, int x)
        {
            int _x = Mod((int) n.X, x);
            int _y = Mod((int) n.Y, x);
            int _z = Mod((int) n.Z, x);

            return new Vector3(_x, _y, _z);
        }

        public static Vector3Int Mod(this Vector3Int n, int x)
        {
            int _x = Mod(n.X, x);
            int _y = Mod(n.Y, x);
            int _z = Mod(n.Z, x);

            return new Vector3Int(_x, _y, _z);
        }

        public static bool IsBetween(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static float Pow(this int @base, int exponent)
        {
            bool exponentNegative = exponent < 0;

            if (exponentNegative)
                exponent = -exponent;

            int result = 1;

            for (int i = 0; i < exponent; i++)
            {
                result *= @base;
            }

            if (exponentNegative)
                return 1f / result;
            else
                return result;
        }

        public static int Clamp(this int number, int min, int max)
        {
            if (number < min)
                return min;
            else if (number > max)
                return max;
            else
                return number;
        }

        public static float Clamp(this float number, float min, float max)
        {
            if (number < min)
                return min;
            else if (number > max)
                return max;
            else
                return number;
        }

        public static float Clamp01(this float number)
        {
            return Clamp(number, 0, 1);
        }

        public static int Mod(this int n, int x)
        {
            return (n % x + x) % x;
        }

        public static float Max(this float a, float b)
        {
            return a > b ? a : b;
        }

        public static float Min(this float a, float b)
        {
            return a < b ? a : b;
        }

        public static float SqrDistance(this Vector3 p1, Vector3 p2)
        {
            Vector3 p3 = p1 - p2;

            float x = p3.X;
            float y = p3.Y;
            float z = p3.Z;

            float result = x * x + y * y + z * z;

            return result;
        }

        public static float SqrDistance(float x, float y, float z, Vector3 p1)
        {
            float _x = x - p1.X;
            float _y = y - p1.Y;
            float _z = z - p1.Z;

            float result = _x * _x + _y * _y + _z * _z;

            return result;
        }

        public static float Distance(this Vector3 p1, Vector3 p2)
        {
            float sqrD = SqrDistance(p1, p2);
            float result = (float) Math.Sqrt(sqrD);

            return result;
        }

        public static float Distance(float x, float y, float z, Vector3 p2)
        {
            float sqrD = SqrDistance(x, y, z, p2);
            float result = (float) Math.Sqrt(sqrD);

            return result;
        }

        public static readonly int[] PowerOf2s =
        {
            1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192,
        };

        public static float Map(this float value, float x1, float y1, float x2, float y2)
        {
            return (value - x1) / (y1 - x1) * (y2 - x2) + x2;
        }

        public static BoundingBox FromPoints(VertexPositionNormalTexture[] verts)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            for (int i = 0; i < verts.Length; ++i)
            {
                var v = verts[i];
                Vector3.Min(ref min, ref v.Position, out min);
                Vector3.Max(ref max, ref v.Position, out max);
            }

            return new BoundingBox(min, max);
        }

        public static HitResult ScreenPositionToWorldPositionRaycast(Vector2 screenPos, CameraComponent camera,
            Simulation simulation)
        {
            Matrix invViewProj = Matrix.Invert(camera.ViewProjectionMatrix);

            // Reconstruct the projection-space position in the (-1, +1) range.
            //    Don't forget that Y is down in screen coordinates, but up in projection space
            Vector3 sPos;
            sPos.X = screenPos.X * 2f - 1f;
            sPos.Y = 1f - screenPos.Y * 2f;

            // Compute the near (start) point for the raycast
            // It's assumed to have the same projection space (x,y) coordinates and z = 0 (lying on the near plane)
            // We need to unproject it to world space
            sPos.Z = 0f;
            var vectorNear = Vector3.Transform(sPos, invViewProj);
            vectorNear /= vectorNear.W;

            // Compute the far (end) point for the raycast
            // It's assumed to have the same projection space (x,y) coordinates and z = 1 (lying on the far plane)
            // We need to unproject it to world space
            sPos.Z = 1f;
            var vectorFar = Vector3.Transform(sPos, invViewProj);
            vectorFar /= vectorFar.W;

            // Raycast from the point on the near plane to the point on the far plane and get the collision result
            var result = simulation.Raycast(vectorNear.XYZ(), vectorFar.XYZ());

            return result;
        }
    }
}