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
    public enum TriggerType : sbyte  //signed byte => 8 bits => 0-255
    {
        PlaySound,
        PlayAnimation,
        EndLevel,
        InitiateBattle,
        DisplayToast,
        PickupItem, //collidable maybe?
        ActivateTrap,
        ActivateEnemy,
    }
}
