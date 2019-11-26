/*
Function: 		Used by PickingManager to specify the behaviour required on picking (i.e. place or remove)
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/
namespace GDLibrary
{
    public enum PickingBehaviourType : sbyte
    {
        PickOnly, //used for mouse over info
        PickAndPlace,  //lift and place
        PickAndRemove //remove 
    }
}