/*
Function: 		Used by Controller to define what action the controller applies to its attached Actor. For example, a FirstPersonCamera has
                an attached controller of type FirstPerson.
Author: 		NMCG
Version:		1.1
Bugs:			None
Fixes:			None
*/

namespace GDLibrary
{
    public enum ControllerType : sbyte
    {
        Drive,          //applied to 3D model
        FirstPerson,    //applied to camera
        ThirdPerson,    //applied to camera
        Flight,         //applied to camera
        Rail,           //applied to camera or model
        Curve,          //applied to camera or model
        Security,       //applied to camera
        Track,
        CollidableCamera,
        Spin,
    }
}
