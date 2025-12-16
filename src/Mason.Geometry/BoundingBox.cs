using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

using ZLinq;

namespace Mason.Geometry;

public struct BoundingBox : IEquatable<BoundingBox>, IFormattable
{
    private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

    public BoundingBox()
    {
        MinPt = new Vec3(0, 0, 0);
        MaxPt = new Vec3(1, 1, 1);
    }

    public BoundingBox(double minPt, double maxPt)
    {
        MinPt = new Vec3(minPt);
        MaxPt = new Vec3(maxPt);
    }

    public BoundingBox(ref readonly Vec3 minPt, ref readonly Vec3 maxPt)
    {
        MinPt = minPt;
        MaxPt = maxPt;
    }

    public BoundingBox(double[] minPt, double[] maxPt)
    {
        MinPt = new Vec3(minPt);
        MaxPt = new Vec3(maxPt);
    }

    public Vec3 MaxPt { readonly get; set; }
    public Vec3 MinPt { readonly get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Intersect(params BoundingBox[] bboxs)
    {
        Vec3 minPt = new();
        Vec3 maxPt = new();
        List<BoundingBox> listBBox = [.. bboxs];
        minPt.X = listBBox.AsValueEnumerable().Min(p => p.MaxPt.X);
        minPt.Y = listBBox.AsValueEnumerable().Min(p => p.MaxPt.Y);
        minPt.Z = listBBox.AsValueEnumerable().Min(p => p.MaxPt.Z);

        maxPt.X = listBBox.AsValueEnumerable().Max(p => p.MinPt.X);
        maxPt.Y = listBBox.AsValueEnumerable().Max(p => p.MinPt.Y);
        maxPt.Z = listBBox.AsValueEnumerable().Max(p => p.MinPt.Z);

        BoundingBox outbbox = new(in maxPt, in minPt);
        outbbox.Check();
        return outbbox;
    }

    //https://developer.mozilla.org/en-US/docs/Games/Techniques/3D_collision_detection#aabb_vs._aabb
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIntersect(ref readonly BoundingBox bbox1, ref readonly BoundingBox bbox2)
    {
        return bbox1.MinPt.X <= bbox2.MaxPt.X
            && bbox1.MaxPt.X >= bbox2.MinPt.X
            && bbox1.MinPt.Y <= bbox2.MaxPt.Y
            && bbox1.MaxPt.Y >= bbox2.MinPt.Y
            && bbox1.MinPt.Z <= bbox2.MaxPt.Z
            && bbox1.MaxPt.Z >= bbox2.MinPt.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Offset(ref readonly BoundingBox bbox, double offset)
    {
        return Offset(bbox, new Vec3(-offset / 2.0), new Vec3(offset / 2.0));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Offset(BoundingBox bbox, Vec3 MinOffset, Vec3 MaxOffset)
    {
        Vec3 minPt = bbox.MinPt.Add(in MinOffset);
        Vec3 maxPt = bbox.MaxPt.Add(in MaxOffset);
        return new(in minPt, in maxPt);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BoundingBox left, BoundingBox right)
    {
        return !(left == right);
    }

    public static bool operator ==(BoundingBox left, BoundingBox right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Scale(ref readonly BoundingBox bbox, double scale)
    {
        Vec3 scale_vec = new(scale);
        Vec3 center = bbox.Center();
        return Scale(in bbox, scale_vec, in center);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Scale(
        ref readonly BoundingBox bbox,
        double scale,
        ref readonly Vec3 origin
    )
    {
        Vec3 scale_vec = new(scale);
        return Scale(in bbox, scale_vec, in origin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Scale(ref readonly BoundingBox bbox, ref readonly Vec3 scale)
    {
        Vec3 center = bbox.Center();
        return Scale(in bbox, scale, in center);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Scale(
        ref readonly BoundingBox bbox,
        Vec3 scale,
        ref readonly Vec3 origin
    )
    {
        scale.Subtract(1);
        Vec3 minPt = new(
            bbox.MinPt.X + ((bbox.MinPt.X - origin.X) * scale.X),
            bbox.MinPt.Y + ((bbox.MinPt.Y - origin.Y) * scale.Y),
            bbox.MinPt.Z + ((bbox.MinPt.Z - origin.Z) * scale.Z)
        );
        Vec3 maxPt = new(
            bbox.MaxPt.X + ((bbox.MaxPt.X - origin.X) * scale.X),
            bbox.MaxPt.Y + ((bbox.MaxPt.Y - origin.Y) * scale.Y),
            bbox.MaxPt.Z + ((bbox.MaxPt.Z - origin.Z) * scale.Z)
        );
        return new(in minPt, in maxPt);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Union(params BoundingBox[] bboxs)
    {
        Vec3 maxpt = new();
        Vec3 Minpt = new();
        List<BoundingBox> listBBox = [.. bboxs];
        maxpt.X = listBBox.AsValueEnumerable().Max(p => p.MaxPt.X);
        maxpt.Y = listBBox.AsValueEnumerable().Max(p => p.MaxPt.Y);
        maxpt.Z = listBBox.AsValueEnumerable().Max(p => p.MaxPt.Z);

        Minpt.X = listBBox.AsValueEnumerable().Min(p => p.MinPt.X);
        Minpt.Y = listBBox.AsValueEnumerable().Min(p => p.MinPt.Y);
        Minpt.Z = listBBox.AsValueEnumerable().Min(p => p.MinPt.Z);

        return new BoundingBox(in Minpt, in maxpt);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vec3 Center()
    {
        return new(MidX(), MidY(), MidZ());
    }

    public readonly void Check()
    {
        if (MaxPt.X < MinPt.X || MaxPt.Y < MinPt.Y || MaxPt.Z < MinPt.Z)
        {
            string msg =
                $"MaxPt({MaxPt.X}, {MaxPt.Y}, {MaxPt.Z})< MinPt({MinPt.X}, {MinPt.Y}, {MinPt.Z})";
            Log.Error(msg);
            throw new ArithmeticException(msg);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(BoundingBox other)
    {
        return MinPt == other.MinPt && MaxPt == other.MaxPt;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object obj)
    {
        return obj is BoundingBox boundingBox && Equals(boundingBox);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(MinPt.GetHashCode(), MaxPt.GetHashCode());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double LengthX()
    {
        return MaxPt.X - MinPt.X;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double LengthY()
    {
        return MaxPt.Y - MinPt.Y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double LengthZ()
    {
        return MaxPt.Z - MinPt.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Axis MaxAxis()
    {
        return LengthX() >= LengthY() && LengthX() >= LengthZ() ? Axis.X
            : LengthY() >= LengthZ() ? Axis.Y
            : Axis.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Axis MinAxis()
    {
        return LengthX() <= LengthY() && LengthX() <= LengthZ() ? Axis.X
            : LengthY() <= LengthZ() ? Axis.Y
            : Axis.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double MidX()
    {
        return (MaxPt.X + MinPt.X) / 2.0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double MidY()
    {
        return (MaxPt.Y + MinPt.Y) / 2.0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double MidZ()
    {
        return (MaxPt.Z + MinPt.Z) / 2.0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoundingBox Offset(double offset)
    {
        Offset(new Vec3(-offset / 2.0), new Vec3(offset / 2.0));
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoundingBox Offset(Vec3 MinOffset, Vec3 MaxOffset)
    {
        MinPt = MinPt.Add(in MinOffset);
        MaxPt = MaxPt.Add(in MaxOffset);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoundingBox Scale(double scale)
    {
        Vec3 scale_vec = new(scale);
        Vec3 center = Center();
        Scale(scale_vec, in center);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoundingBox Scale(double scale, ref readonly Vec3 origin)
    {
        Vec3 scale_vec = new(scale);
        Scale(scale_vec, in origin);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoundingBox Scale(ref readonly Vec3 scale)
    {
        Vec3 center = Center();
        Scale(scale, in center);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BoundingBox Scale(Vec3 scale, ref readonly Vec3 origin)
    {
        scale.Subtract(1);
        MinPt = new Vec3(
            MinPt.X + ((MinPt.X - origin.X) * scale.X),
            MinPt.Y + ((MinPt.Y - origin.Y) * scale.Y),
            MinPt.Z + ((MinPt.Z - origin.Z) * scale.Z)
        );
        MaxPt = new Vec3(
            MaxPt.X + ((MaxPt.X - origin.X) * scale.X),
            MaxPt.Y + ((MaxPt.Y - origin.Y) * scale.Y),
            MaxPt.Z + ((MaxPt.Z - origin.Z) * scale.Z)
        );
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double Volume()
    {
        return LengthX() * LengthY() * LengthZ();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString()
    {
        return ToString("G", CultureInfo.CurrentCulture);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string ToString(string format, IFormatProvider formatProvider)
    {
        return $"BoundingBox(Min: {MinPt.ToString(format, formatProvider)}, Max: {MaxPt.ToString(format, formatProvider)})";
    }
}
