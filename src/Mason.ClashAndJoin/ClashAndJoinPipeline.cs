using System;
using Autodesk.Revit.DB;
using Mason.Geometry;

namespace Mason.ClashAndJoin;

/// <summary>
/// Provides a fluent pipeline for checking clashes and joining/unjoining elements in Revit.
/// </summary>
internal sealed class ClashAndJoinPipeline
{
    /// <summary>
    /// Flag indicating whether the pipeline should continue processing.
    /// </summary>
    public bool ContinueFlag { get; private set; }

    /// <summary>
    /// The Revit document.
    /// </summary>
    public Document Doc { get; private set; } = null!;

    /// <summary>
    /// The first proxy element.
    /// </summary>
    public ProxyElement E1 { get; private set; } = null!;

    /// <summary>
    /// The second proxy element.
    /// </summary>
    public ProxyElement E2 { get; private set; } = null!;

    /// <summary>
    /// Initializes the pipeline with the document and two elements.
    /// </summary>
    public ClashAndJoinPipeline Init(Document doc, ProxyElement e1, ProxyElement e2)
    {
        Doc = doc ?? throw new ArgumentNullException(nameof(doc));
        E1 = e1 ?? throw new ArgumentNullException(nameof(e1));
        E2 = e2 ?? throw new ArgumentNullException(nameof(e2));
        ContinueFlag = true;
        return this;
    }

    /// <summary>
    /// Determines if the elements are identical and sets ContinueFlag accordingly.
    /// </summary>
    public ClashAndJoinPipeline IsIdenticalElement(bool continueIfIdentical)
    {
        if (!ContinueFlag)
            return this;

#if REVIT2018 || REVIT2019 || REVIT2020 || REVIT2021 || REVIT2022 || REVIT2023
        ContinueFlag =
            E1.Id.IntegerValue == E2.Id.IntegerValue ? continueIfIdentical : !continueIfIdentical;
#endif
#if REVIT2024 || REVIT2025
        ContinueFlag = E1.Id.Value == E2.Id.Value ? continueIfIdentical : !continueIfIdentical;
#endif
        return this;
    }

    /// <summary>
    /// Checks if bounding boxes intersect and sets ContinueFlag accordingly.
    /// </summary>
    public ClashAndJoinPipeline IsBoundingBoxIntersect(bool continueIfIntersect)
    {
        if (!ContinueFlag)
            return this;

        ContinueFlag = BoundingBox.IsIntersect(ref E1.CachedBBox, ref E2.CachedBBox)
            ? continueIfIntersect
            : !continueIfIntersect;
        return this;
    }

    /// <summary>
    /// Checks if elements are joined and sets ContinueFlag accordingly.
    /// </summary>
    public ClashAndJoinPipeline IsJoined(bool continueIfJoined)
    {
        if (!ContinueFlag)
            return this;

        ContinueFlag = JoinGeometryUtils.AreElementsJoined(Doc, E1.E, E2.E)
            ? continueIfJoined
            : !continueIfJoined;
        return this;
    }

    /// <summary>
    /// Joins the two elements if ContinueFlag is true.
    /// </summary>
    public void Join()
    {
        if (!ContinueFlag)
            return;

        try
        {
            JoinGeometryUtils.JoinGeometry(Doc, E1.E, E2.E);
        }
        catch
        {
            // ignore exceptions to allow fluent chaining
        }
    }

    /// <summary>
    /// Unjoins the two elements if ContinueFlag is true.
    /// </summary>
    public void Unjoin()
    {
        if (!ContinueFlag)
            return;

        try
        {
            JoinGeometryUtils.UnjoinGeometry(Doc, E1.E, E2.E);
        }
        catch
        {
            // ignore exceptions
        }
    }

    /// <summary>
    /// Switches the join order of the two elements if ContinueFlag is true.
    /// </summary>
    public void SwitchJoin()
    {
        if (!ContinueFlag)
            return;

 
            JoinGeometryUtils.SwitchJoinOrder(Doc, E1.E, E2.E);
 
    }

    /// <summary>
    /// Performs clash detection between the two elements using the filter.
    /// Returns true if a clash is detected.
    /// </summary>
    public bool ClashDetection()
    {
        if (!ContinueFlag)
            return false;

        try
        {
            return E1.Filter?.PassesFilter(E2.E) == true;
        }
        catch
        {
            return false;
        }
    }
}
