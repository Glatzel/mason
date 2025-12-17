using Autodesk.Revit.DB;
using static System.Math;

namespace Mason.ClashAndJoin.Command.Bench.Misc;

public static class BoundingBoxXYZUtils
{
    public static BoundingBoxXYZ Union(params BoundingBoxXYZ[] bboxs)
    {
        double maxX = double.MinValue;
        double maxY = double.MinValue;
        double maxZ = double.MinValue;

        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double minZ = double.MaxValue;
        foreach (BoundingBoxXYZ bbox in bboxs)
        {
            maxX = Max(maxX, bbox.Max.X);
            maxY = Max(maxY, bbox.Max.Y);
            maxZ = Max(maxZ, bbox.Max.Z);

            minX = Min(minX, bbox.Min.X);
            minY = Min(minY, bbox.Min.Y);
            minZ = Min(minZ, bbox.Min.Z);
        }
        return new() { Max = new XYZ(maxX, maxY, maxZ), Min = new XYZ(minX, minY, minZ) };
    }

    public static bool IsIntersect(BoundingBoxXYZ bbox1, BoundingBoxXYZ bbox2)
    {
        return bbox1.Min.X <= bbox2.Max.X
            && bbox1.Max.X >= bbox2.Min.X
            && bbox1.Min.Y <= bbox2.Max.Y
            && bbox1.Max.Y >= bbox2.Min.Y
            && bbox1.Min.Z <= bbox2.Max.Z
            && bbox1.Max.Z >= bbox2.Min.Z;
    }

    public static XYZ Center(BoundingBoxXYZ bbox)
    {
        double cX = (bbox.Max.X + bbox.Min.X) / 2.0;
        double cY = (bbox.Max.Y + bbox.Min.Y) / 2.0;
        double cZ = (bbox.Max.Z + bbox.Min.Z) / 2.0;
        return new(cX, cY, cZ);
    }

    public static BoundingBoxXYZ Offset(BoundingBoxXYZ bbox, double offset)
    {
        return new()
        {
            Min = new XYZ(bbox.Min.X - offset, bbox.Min.Y - offset, bbox.Min.Z - offset),
            Max = new XYZ(bbox.Max.X + offset, bbox.Max.Y + offset, bbox.Max.Z + offset),
        };
    }

    public static BoundingBoxXYZ Scale(BoundingBoxXYZ bbox, double scale)
    {
        BoundingBoxXYZ newBBox = new();
        scale--;

        XYZ center = Center(bbox);
        newBBox.Min = new XYZ(
            bbox.Min.X + ((bbox.Min.X - center.X) * scale),
            bbox.Min.Y + ((bbox.Min.Y - center.Y) * scale),
            bbox.Min.Z + ((bbox.Min.Z - center.Z) * scale)
        );
        newBBox.Max = new XYZ(
            bbox.Max.X + ((bbox.Max.X - center.X) * scale),
            bbox.Max.Y + ((bbox.Max.Y - center.Y) * scale),
            bbox.Max.Z + ((bbox.Max.Z - center.Z) * scale)
        );

        return newBBox;
    }
}
