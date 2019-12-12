/*
Function: 		First person COLLIDABLE camera controller.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    /// <summary>
    /// A collidable camera has a body and collision skin from a player object but it has no modeldata or texture
    /// </summary>
    public class CollidableFirstPersonCameraController : FirstPersonCameraController
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public CollidableFirstPersonCameraController(
            string id,
            ControllerType controllerType,
            Keys[] moveKeys,
            float moveSpeed,
            float rotateSpeed,
            ManagerParameters managerParameters,
            Vector3 movementVector,
            Vector3 rotationVector
        ) : base(id, controllerType, moveKeys, moveSpeed, rotateSpeed, managerParameters, movementVector, rotationVector) {
        }
        #endregion
        
        #region Methods
        //Handle Keyboard Input
        public override void HandleKeyboardInput(GameTime gameTime, Actor3D parentActor)
        {
        }

        //Handle Movement
        public override void HandleMovement(Actor3D parentActor)
        {
        }

        //Update
        public override void Update(GameTime gameTime, IActor actor)
        {
            base.Update(gameTime, actor);
        }
        #endregion
    }
}