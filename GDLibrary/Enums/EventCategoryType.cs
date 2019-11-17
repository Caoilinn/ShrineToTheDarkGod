namespace GDLibrary
{
    //One category for each group of events in EventType
    public enum EventCategoryType : sbyte
    {
        MainMenu,
        UIMenu,
        Camera,
        Player,
        NonPlayer,
        Pickup,
        Door,
        Screen,
        Opacity,
        SystemAdd,      //Used to send add related events    e.g. add objects to oject manager, camera manager, ui manager
        SystemRemove,   //Used to send remove related events e.g. remove objects from oject manager, camera manager, ui manager
        Sound2D,
        Sound3D,
        Video,
        GlobalSound,
        Game,
        Debug,
        Menu,
        ObjectPicking,
        Combat,
        Inventory,
    }
}