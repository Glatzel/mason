using Autodesk.Revit.DB;

namespace Mason.Geometry;

public static class Extension
{
    public static BoundingBox ToBoundingBox(this BoundingBoxXYZ bbox)
    {
        Vec3 min = bbox.Min.ToVec3();
        Vec3 max = bbox.Max.ToVec3();
        return new BoundingBox(ref min, ref max);
    }

    public static BoundingBoxXYZ ToBoundingBoxXYZ(this BoundingBox bbox)
    {
        return new()
        {
            Min = new XYZ(bbox.MinPt.X, bbox.MinPt.Y, bbox.MinPt.Z),
            Max = new XYZ(bbox.MaxPt.X, bbox.MaxPt.Y, bbox.MaxPt.Z),
        };
    }

    public static Vec3 ToVec3(this XYZ pt)
    {
        return new(pt.X, pt.Y, pt.Z);
    }

    public static XYZ ToXYZ(this Vec3 pt)
    {
        return new(pt.X, pt.Y, pt.Z);
    }
}
