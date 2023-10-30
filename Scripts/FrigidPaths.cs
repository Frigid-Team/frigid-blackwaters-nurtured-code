namespace FrigidBlackwaters
{
    public static class FrigidPaths
    {
#if UNITY_EDITOR
        public static class MenuItem
        {
            public const string Window = "Window/";
            public const string Jobs = "Jobs/";
            public const string EditMode = "Edit Mode/";
            public const string Help = "Help/";
        }
#endif

#if UNITY_EDITOR
        public static class ProjectFolder
        {
            public const string Assets = "Assets/";

            public const string Prefabs = "Prefabs/";

            public const string ScriptableObjects = "Scriptable_Objects/";
        }
#endif

        public static class CreateAssetMenu
        {
            public const string Core = "Core/";

            public const string Primitives = "Primitives/";
            public const string Unity = "Unity/";
            public const string Rendering = "Rendering/";

            public const string Game = "Game/";

            public const string Animation = "Animation/";
            public const string Tiles = "Tiles/";
            public const string Damage = "Damage/";
            public const string DungeonGeneration = "Dungeon Generation/";
            public const string Mobs = "Mobs/";
            public const string Items = "Items/";
            public const string Menus = "Menus/";
            public const string SceneChange = "Scene Change/";
            public const string Expeditions = "Expeditions/";
            public const string Boons = "Boons/";
        }

#if UNITY_EDITOR
        public static class Scenes
        {
            public const string Preloaded = "Assets/Scenes/Build_Scenes/Preloaded.unity";
            public const string Dungeon = "Assets/Scenes/Build_Scenes/Dungeon.unity";
            public const string PortCity = "Assets/Scenes/Build_Scenes/Port_City.unity";
        }
#endif
    }
}
