using System;
using Autodesk.Revit.DB;
using Mason.Core;
using Mason.Geometry;

namespace Mason.ClashAndJoin;

/// <summary>
/// Represents a Revit element proxy for clash detection with cached bounding box and filter support.
/// </summary>
internal sealed class ProxyElement : IDisposable, IEquatable<ProxyElement>
{
    /// <summary>
    /// The underlying Revit element.
    /// </summary>
    public Element E { get; }

    /// <summary>
    /// Cached bounding box in the active view.
    /// </summary>
    public BoundingBox CachedBBox;

    /// <summary>
    /// Element ID.
    /// </summary>
    public ElementId Id { get; }

    /// <summary>
    /// Filter used for intersection tests.
    /// </summary>
    public ElementIntersectsElementFilter Filter { get; set; }

    /// <summary>
    /// Returns the bounding box of the element in the active view.
    /// </summary>
    public BoundingBox BBox => E.get_BoundingBox(RevitContext.ActiveView).ToBoundingBox();

#if REVIT2018 || REVIT2019 || REVIT2020 || REVIT2021 || REVIT2022 || REVIT2023
    /// <summary>
    /// Integer-based element ID for older Revit versions.
    /// </summary>
    public int IntId => E.Id.IntegerValue;
#endif

#if REVIT2024 || REVIT2025
    /// <summary>
    /// Long-based element ID for newer Revit versions.
    /// </summary>
    public long IntId => E.Id.Value;
#endif

    /// <summary>
    /// Initializes a new proxy element.
    /// </summary>
    /// <param name="id">Element ID.</param>
    /// <param name="cacheBBox">Whether to cache the bounding box immediately.</param>
    /// <exception cref="ArgumentNullException">Thrown if the element cannot be found.</exception>
    public ProxyElement(ElementId id, bool cacheBBox)
    {
        E =
            RevitContext.Doc.GetElement(id)
            ?? throw new ArgumentNullException(nameof(id), $"Element with ID {id} not found.");

        Id = id;

        if (cacheBBox)
        {
            CacheBBox();
        }
    }

    /// <summary>
    /// Caches the bounding box for the element.
    /// </summary>
    public void CacheBBox()
    {
        CachedBBox = BBox;
    }

    /// <summary>
    /// Initializes the intersection filter for clash detection.
    /// </summary>
    public void InitFilter()
    {
        try
        {
            Filter = new ElementIntersectsElementFilter(E);
        }
        catch
        {
            Filter = null;
        }
    }

    /// <summary>
    /// Releases resources used by the filter.
    /// </summary>
    public void Dispose()
    {
        Filter?.Dispose();
        Filter = null;
    }

    public override int GetHashCode() => IntId.GetHashCode();

    public bool Equals(ProxyElement other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return IntId == other.IntId;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ProxyElement);
    }

    public static bool operator ==(ProxyElement left, ProxyElement right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(ProxyElement left, ProxyElement right)
    {
        return !(left == right);
    }
}
