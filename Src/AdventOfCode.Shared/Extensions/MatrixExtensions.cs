using System;
using System.Numerics;

namespace AdventOfCode.Shared.Extensions
{
    public static class MatrixExtensions
    {
        public static Vector3 Transform(this Matrix4x4 matrix, Vector3 vector)
        {
            float x = vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31 + matrix.M41;
            float y = vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32 + matrix.M42;
            float z = vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33 + matrix.M43;

            return new Vector3((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(z));
        }
    }
}