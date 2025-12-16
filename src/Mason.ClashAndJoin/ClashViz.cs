namespace Mason.ClashAndJoin;

public class OverrideGraphics()
{
    //    protected ProxyElement _e1 = item.E1;
    //    protected ProxyElement _e2 = item.E2;
    //    protected VizSetting _setting = setting;

    //    public override void Cook()
    //    {
    //        FillPatternElement solidFillPattern = new FilteredElementCollector(Doc)
    //            .OfType<FillPatternElement>()
    //            .First(a => a.GetFillPattern().IsSolidFill);
    //        View3D view = (View3D)UIDoc.ActiveView;
    //        //var defaultsetting = new OverrideGraphicSettings();
    //        OverrideGraphicSettings othersetting = new();
    //        OverrideGraphicSettings oversetting1 = new();
    //        OverrideGraphicSettings oversetting2 = new();
    //        othersetting.SetSurfaceTransparency(_setting.Transparency);

    //#if  REVIT2018
    //        othersetting.SetCutFillPatternVisible(false);
    //        othersetting.SetProjectionFillPatternVisible(false);
    //        oversetting1.SetProjectionFillColor(_setting.Cd1);
    //        oversetting1.SetCutFillColor(_setting.Cd1);
    //        oversetting1.SetCutFillPatternId(solidFillPattern.Id);
    //        oversetting1.SetProjectionFillPatternId(solidFillPattern.Id);
    //        oversetting2.SetProjectionFillColor(_setting.Cd2);
    //        oversetting2.SetCutFillColor(_setting.Cd2);
    //        oversetting2.SetCutFillPatternId(solidFillPattern.Id);
    //        oversetting2.SetProjectionFillPatternId(solidFillPattern.Id);
    //#endif
    //#if REVIT2019 || REVIT2020 || REVIT2021 || REVIT2022 ||REVIT2023||REVIT2024||REVIT2025
    //        oversetting1.SetSurfaceForegroundPatternColor(_setting.Cd1);
    //        oversetting1.SetSurfaceForegroundPatternId(solidFillPattern.Id);
    //        oversetting1.SetCutForegroundPatternColor(_setting.Cd1);
    //        oversetting1.SetCutForegroundPatternId(solidFillPattern.Id);

    //        oversetting2.SetSurfaceForegroundPatternColor(_setting.Cd2);
    //        oversetting2.SetSurfaceForegroundPatternId(solidFillPattern.Id);
    //        oversetting2.SetCutForegroundPatternColor(_setting.Cd2);
    //        oversetting2.SetCutForegroundPatternId(solidFillPattern.Id);
    //#endif
    //        foreach (
    //            Element i in new FilteredElementCollector(Doc, UIDoc.ActiveView.Id)
    //                .Cast<Element>()
    //                .ToList()
    //        )
    //        {
    //            view.SetElementOverrides(i.Id, othersetting);
    //        }
    //        view.SetElementOverrides(_e1.Id, oversetting1);
    //        view.SetElementOverrides(_e2.Id, oversetting2);
    //        view.SetSectionBox(
    //            BoundingBox
    //                .Union(_e1.CachedBBox, _e2.CachedBBox)
    //                .Offset(_setting.offset)
    //                .ToBoundingBoxXYZ()
    //        );
    //        view.IsSectionBoxActive = true;
    //    }
}
