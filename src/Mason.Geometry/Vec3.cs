using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Mason.Geometry;

/// <summary>
/// Represents a 3D vector with double precision.
/// Provides optimized operations with ref/out variants and overloads for arithmetic.
/// </summary>
public struct Vec3 : IEquatable<Vec3>, IFormattable
{
    /// <summary>
    /// Logger for error reporting and diagnostics.
    /// </summary>
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    /// <summary> X component of the vector </summary>
    public double X { get; set; }

    /// <summary> Y component of the vector </summary>
    public double Y { get; set; }

    /// <summary> Z component of the vector </summary>
    public double Z { get; set; }

    #region Constructors

    /// <summary>
    /// Initializes a zero vector.
    /// </summary>
    public Vec3()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    /// <summary>
    /// Initializes a vector with the same value for all components.
    /// </summary>
    public Vec3(double scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
    }

    /// <summary>
    /// Initializes a vector with specified components.
    /// </summary>
    public Vec3(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Initializes from an array of length 3.
    /// Throws <see cref="ArgumentException"/> if the array length is not 3.
    /// </summary>
    public Vec3(double[] v)
    {
        if (v.Length != 3)
        {
            string msg = $"Length of input is not 3, get {v.Length}.";
            Log.Error(msg);
            throw new ArgumentException(msg, nameof(v));
        }
        X = v[0];
        Y = v[1];
        Z = v[2];
    }

    #endregion

    #region Static Properties

    public static Vec3 One => new(1, 1, 1);
    public static Vec3 UnitX => new(1, 0, 0);
    public static Vec3 UnitY => new(0, 1, 0);
    public static Vec3 UnitZ => new(0, 0, 1);
    public static Vec3 Zero => new(0, 0, 0);

    #endregion

    #region Indexer

    /// <summary>
    /// Access components by index (0:X, 1:Y, 2:Z)
    /// </summary>
    public double this[int idx]
    {
        readonly get =>
            idx switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new ArgumentException($"Unknown index: {idx}"),
            };
        set
        {
            if (idx == 0)
                X = value;
            else if (idx == 1)
                Y = value;
            else if (idx == 2)
                Z = value;
            else
                throw new ArgumentException($"Unknown index: {idx}");
        }
    }

    #endregion

    #region Ref/Out Optimized Static Operations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Add(ref readonly Vec3 v1, ref readonly Vec3 v2) =>
        new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Add(ref readonly Vec3 v1, double t) => new(v1.X + t, v1.Y + t, v1.Z + t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Add(ref readonly Vec3 u, double t, ref Vec3 outv)
    {
        outv.X = u.X + t;
        outv.Y = u.Y + t;
        outv.Z = u.Z + t;
        return outv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Add(ref readonly Vec3 u, ref readonly Vec3 v, ref Vec3 outv)
    {
        outv.X = u.X + v.X;
        outv.Y = u.Y + v.Y;
        outv.Z = u.Z + v.Z;
        return outv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Subtract(ref readonly Vec3 v1, ref readonly Vec3 v2) =>
        new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Subtract(ref readonly Vec3 v1, double t) =>
        new(v1.X - t, v1.Y - t, v1.Z - t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Subtract(ref readonly Vec3 u, double t, ref Vec3 outv)
    {
        outv.X = u.X - t;
        outv.Y = u.Y - t;
        outv.Z = u.Z - t;
        return outv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Subtract(ref readonly Vec3 u, ref readonly Vec3 v, ref Vec3 outv)
    {
        outv.X = u.X - v.X;
        outv.Y = u.Y - v.Y;
        outv.Z = u.Z - v.Z;
        return outv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Multiply(ref readonly Vec3 v, double t, ref Vec3 outv)
    {
        outv.X = v.X * t;
        outv.Y = v.Y * t;
        outv.Z = v.Z * t;
        return outv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Multiply(ref readonly Vec3 v, double t) =>
        new(v.X * t, v.Y * t, v.Z * t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Multiply(ref readonly Vec3 u, ref readonly Vec3 v, ref Vec3 outv)
    {
        outv.X = u.X * v.X;
        outv.Y = u.Y * v.Y;
        outv.Z = u.Z * v.Z;
        return outv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Multiply(ref readonly Vec3 u, ref readonly Vec3 v) =>
        new(u.X * v.X, u.Y * v.Y, u.Z * v.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Divide(ref readonly Vec3 v, double t, ref Vec3 outv)
    {
        outv.X = v.X / t;
        outv.Y = v.Y / t;
        outv.Z = v.Z / t;
        return outv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Divide(ref readonly Vec3 v, double t) => new(v.X / t, v.Y / t, v.Z / t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Divide(ref readonly Vec3 v, ref readonly Vec3 t, ref Vec3 outv)
    {
        outv.X = v.X / t.X;
        outv.Y = v.Y / t.Y;
        outv.Z = v.Z / t.Z;
        return outv;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Divide(ref readonly Vec3 v, ref readonly Vec3 t) =>
        new(v.X / t.X, v.Y / t.Y, v.Z / t.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Dot(ref readonly Vec3 u, ref readonly Vec3 v) =>
        (u.X * v.X) + (u.Y * v.Y) + (u.Z * v.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Cross(ref readonly Vec3 u, ref readonly Vec3 v) =>
        new((u.Y * v.Z) - (u.Z * v.Y), (u.Z * v.X) - (u.X * v.Z), (u.X * v.Y) - (u.Y * v.X));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Normalize(ref readonly Vec3 v)
    {
        double length = v.Length();
        return new Vec3(v.X / length, v.Y / length, v.Z / length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Length(ref readonly Vec3 v) =>
        Math.Sqrt((v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Length2(ref readonly Vec3 v) =>
        (v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z);

    #endregion

    #region Operator Overloads

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator -(Vec3 v) => new(-v.X, -v.Y, -v.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator +(Vec3 left, Vec3 right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator +(Vec3 left, double right) =>
        new(left.X + right, left.Y + right, left.Z + right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator +(double left, Vec3 right) =>
        new(left + right.X, left + right.Y, left + right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator -(Vec3 left, Vec3 right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator -(Vec3 left, double right) =>
        new(left.X - right, left.Y - right, left.Z - right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator -(double left, Vec3 right) =>
        new(left - right.X, left - right.Y, left - right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator *(Vec3 left, Vec3 right) =>
        new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator *(Vec3 left, double right) =>
        new(left.X * right, left.Y * right, left.Z * right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator *(double left, Vec3 right) =>
        new(left * right.X, left * right.Y, left * right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator /(Vec3 left, Vec3 right) =>
        new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator /(Vec3 left, double right) =>
        new(left.X / right, left.Y / right, left.Z / right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator /(double left, Vec3 right) =>
        new(left / right.X, left / right.Y, left / right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vec3 left, Vec3 right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vec3 left, Vec3 right) => !(left == right);

    #endregion

    #region Instance Operations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Add(double v)
    {
        X += v;
        Y += v;
        Z += v;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Add(ref readonly Vec3 v)
    {
        X += v.X;
        Y += v.Y;
        Z += v.Z;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Subtract(double v)
    {
        X -= v;
        Y -= v;
        Z -= v;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Subtract(ref readonly Vec3 v)
    {
        X -= v.X;
        Y -= v.Y;
        Z -= v.Z;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Multiply(double t)
    {
        X *= t;
        Y *= t;
        Z *= t;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Multiply(ref readonly Vec3 v)
    {
        X *= v.X;
        Y *= v.Y;
        Z *= v.Z;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Divide(double t)
    {
        X /= t;
        Y /= t;
        Z /= t;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Divide(ref readonly Vec3 v)
    {
        X /= v.X;
        Y /= v.Y;
        Z /= v.Z;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vec3 Normalize()
    {
        double length = Length();
        X /= length;
        Y /= length;
        Z /= length;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double Length()
    {
        return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double Length2()
    {
        return (X * X) + (Y * Y) + (Z * Z);
    }
    #endregion

    #region Overrides & Interface Implementations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Vec3 other) => X == other.X && Y == other.Y && Z == other.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object obj) => obj is Vec3 v && Equals(v);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode() => HashCode.Combine(X, Y, Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString() => ToString("G", CultureInfo.CurrentCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string format) =>
        ToString(format, CultureInfo.CurrentCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string format, IFormatProvider formatProvider)
    {
        string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
        return $"Vec3<{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}{separator} {Z.ToString(format, formatProvider)}>";
    }

    #endregion
}
