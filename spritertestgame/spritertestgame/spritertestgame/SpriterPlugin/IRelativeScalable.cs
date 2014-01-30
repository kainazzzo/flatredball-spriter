namespace FlatRedBallExtensions
{
    public interface IRelativeScalable
    {
        float RelativeScaleX { get; set; }
        float RelativeScaleY { get; set; }
        float RelativeScaleZ { get; set; }
        float ScaleX { get; set; }
        float ScaleY { get; set; }
        float ScaleZ { get; set; }
        bool ParentScaleChangesPosition { get; set; }
    }
}