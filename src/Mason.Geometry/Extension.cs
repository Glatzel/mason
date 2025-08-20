using Autodesk.Revit.DB;
using Glatzel.Algorithm;

namespace Mason.Geometry;

public static class Extension
{
    public static BoundingBox ToBoundingBox(this BoundingBoxXYZ bbox)
    {
        return new(bbox.Min.ToVec3(), bbox.Max.ToVec3());
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
