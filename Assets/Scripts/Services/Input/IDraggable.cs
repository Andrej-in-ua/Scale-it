namespace Services.Input
{
    public interface IDraggable
    {
        public const string DraggableViewSortingLayerName= "DraggableView";
        public const string DraggableUISortingLayerName= "DraggableUI";

        public int Priority => 1;
    }
}