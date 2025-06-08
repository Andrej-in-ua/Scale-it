public class Constants
{
    // UI
    public const string InventoryPath = "Inventory";
    public const string UICardPreviewPath = "UICardPreviewPrefab";
    public const string CardSpawnerPath = "CardSpawner";
    
    // View
    public const string CardViewPath = "GameTable/CardViewPrefab";
    public const string GridPath = "GameTable/GridPrefab";
    
    public const string ConnectionViewPath = "GameTable/ConnectionViewPrefab";
    public const string ConnectionsContainerPath = "GameTable/ConnectionsContainerPrefab";

    // Services
    public const string InputServicePath = "InputService";

    // Game Data
    public const string DefaultDeckName = "default";
    
    public class CameraSettings
    {
        public const float MoveMinSpeed = 32f;
        public const float MoveMaxSpeed = 512f;
        public const float MoveAccelerationDuration = 0.2f;
        public const float MoveDecelerationDuration = 0.1f;

        public const float ZoomDuration = 0.4f;
        public const float ZoomStep = 8f;
        public const float ZoomMin = 16f;
        public const float ZoomMax = 128f;
    }
}
