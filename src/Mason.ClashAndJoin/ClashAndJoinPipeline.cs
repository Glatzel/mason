using Autodesk.Revit.DB;

using Mason.Geometry;

namespace Mason.ClashAndJoin;

internal sealed class ClashAndJoinPipeline
{
    public bool ContinueFlag;
    public Document Doc;
    public ProxyElement E1;
    public ProxyElement E2;

    public ClashAndJoinPipeline Init(Document doc, ProxyElement e1, ProxyElement e2)
    {
        ContinueFlag = true;
        Doc = doc;
        E1 = e1;
        E2 = e2;
        return this;
    }

    public ClashAndJoinPipeline IsIdenticalElement(bool continueIfIdentical)
    {
        if (ContinueFlag)
        {
#if REVIT2018 || REVIT2019 || REVIT2020 || REVIT2021 || REVIT2022 || REVIT2023
            ContinueFlag =
                E1.Id.IntegerValue == E2.Id.IntegerValue
                    ? continueIfIdentical
                    : !continueIfIdentical;
#endif
#if REVIT2024 || REVIT2025
            ContinueFlag = E1.Id.Value == E2.Id.Value ? continueIfIdentical : !continueIfIdentical;
#endif
        }
        return this;
    }

    public ClashAndJoinPipeline IsBoundingBoxIntersect(bool continueIfIntersect)
    {
        if (ContinueFlag)
        {
            ContinueFlag = BoundingBox.IsIntersect(ref E1.CachedBBox, ref E2.CachedBBox)
                ? continueIfIntersect
                : !continueIfIntersect;
        }
        return this;
    }

    public ClashAndJoinPipeline IsJoined(bool continueIfJoined)
    {
        if (ContinueFlag)
        {
            ContinueFlag = JoinGeometryUtils.AreElementsJoined(Doc, E1.E, E2.E)
                ? continueIfJoined
                : !continueIfJoined;
        }
        return this;
    }

    public void Join()
    {
        if (!ContinueFlag)
        {
            return;
        }
        try
        {
            JoinGeometryUtils.JoinGeometry(Doc, E1.E, E2.E);
        }
        catch { }
    }

    public void Unjoin()
    {
        if (!ContinueFlag)
        {
            return;
        }
        JoinGeometryUtils.UnjoinGeometry(Doc, E1.E, E2.E);
    }

    public void SwitchJoin()
    {
        if (!ContinueFlag)
        {
            return;
        }
        JoinGeometryUtils.SwitchJoinOrder(Doc, E1.E, E2.E);
    }

    public bool ClashDetection()
    {
        if (!ContinueFlag)
        {
            return false;
        }
        try
        {
            if (E1.Filter?.PassesFilter(E2.E) == true)
                return true;
        }
        catch
        {
            return false;
        }
        return false;
    }
}
