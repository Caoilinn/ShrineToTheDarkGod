/*
Function: 		Flight controllers allows movement in any XYZ direction 
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    
    public class FlightCameraController : UserInputController
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        public FlightCameraController(string id, ControllerType controllerType, Keys[] moveKeys, float moveSpeed, float strafeSpeed, float rotationSpeed, 
            ManagerParameters managerParameters)
            : base(id, controllerType, moveKeys, moveSpeed, strafeSpeed, rotationSpeed, managerParameters)
        {

        }

        public override void HandleMouseInput(GameTime gameTime, Actor3D parentActor)
        {
            //to do...
        }

        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
            //to do...
        }

        //Add Equals, Clone, ToString, GetHashCode...
    }
}