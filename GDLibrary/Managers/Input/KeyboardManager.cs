/*
Function: 		Provide keyboard input functions
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary
{
    public class KeyboardManager : GameComponent
    {
        #region Fields
        protected KeyboardState newState, oldState;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        public KeyboardManager(
            Game game
        ) : base(game) {
        }
        #endregion

        #region Methods
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Store the old keyboard state for later comparison
            this.oldState = this.newState; 

            //Get the current state in THIS update
            this.newState = Keyboard.GetState();
            base.Update(gameTime);
        }

        //Is any key pressed on the keyboard?
        public bool IsKeyPressed()
        {
            return (this.newState.GetPressedKeys().Length != 0);
        }

        //Is a key pressed?
        public bool IsKeyDown(Keys key)
        {
            return this.newState.IsKeyDown(key);
        }

        //Is a key pressed now that was not pressed in the last update?
        public bool IsFirstKeyPress(Keys key)
        {
            return this.newState.IsKeyDown(key) && this.oldState.IsKeyUp(key);
        }

        //Is any key pressed?
        public bool IsAnyKeyPressed()
        {
            return this.newState.GetPressedKeys().Length == 0 ? false : true;
        }

        //Has the keyboard state changed since the last update?
        public bool IsStateChanged()
        {
            return !this.newState.Equals(oldState); //False if no change, otherwise true
        }
        #endregion
    }
}