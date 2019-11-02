/*
Function: 		Used by Actor to help us distunguish one type of actor from another when we perform CD/CR or when we want to enable/disable certain game entities
                e.g. hide all the pickups.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

namespace GDLibrary
{
    public enum ActorType : sbyte  //signed byte => 8 bits => 0-255
    {
        Player,
        Decorator, //i.e.  architecture
        Billboard, //i.e. an imposter for a 3D object e.g. distant tree or facade of a building
        Interactable, //i.e. something that can be moved/lifted e.g. a health pickup, a door
        Camera,
        Zone, //i.e. invisible and triggers events e.g. walk through a bounding volume and trigger game end or camera change
        Helper, //i.e.. a wireframe visualisation for an entitiy e.g. camera, camera path, bounding box of a pickip
        
        Primitive //procedurally created surface i.e. user-defined vertices, color, texture etc
    }
}
