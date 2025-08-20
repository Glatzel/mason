using System;
using Autodesk.Revit.DB;
using Mason.Core;
using Mason.Geometry;
using Pyxis;

namespace Mason.ClashAndJoin;

internal sealed class ProxyElement : IDisposable, IEquatable<ProxyElement>
{
    public Element E;
    public BoundingBox CachedBBox;
    public ElementId Id;
    public ElementIntersectsElementFilter Filter;

    public BoundingBox BBox
    {
        get { return E.get_BoundingBox(RevitContext.ActiveView).ToBoundingBox(); }
    }

#if REVIT2018 || REVIT2019 || REVIT2020 || REVIT2021 || REVIT2022 || REVIT2023

    public int IntId
    {
        get { return E.Id.IntegerValue; }
    }

#endif
#if REVIT2024 || REVIT2025

    public long IntId
    {
        get { return E.Id.Value; }
    }
#endif

    public ProxyElement(ElementId id, bool cacheBBox)
    {
        E = RevitContext.Doc.GetElement(id);
        Id = id;
        if (cacheBBox)
            CacheBBox();
    }

    public void CacheBBox()
    {
        CachedBBox = BBox;
    }

    public void InitFilter()
    {
        try
        {
            Filter = new ElementIntersectsElementFilter(E);
        }
        catch { }
    }

    public void Dispose()
    {
        Filter.Dispose();
    }

    public override int GetHashCode()
    {
        return IntId.GetHashCode();
    }

    public bool Equals(ProxyElement other)
    {
        return IntId == other.IntId;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ProxyElement);
    }
}
