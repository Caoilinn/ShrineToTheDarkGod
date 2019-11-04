/*
Function: 		Stores all objects to be drawn in the game and manually calls Update() and Draw() on same.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    //to do...
    public class ObjectManager : DrawableGameComponent
    {
        #region Fields
        private CameraManager cameraManager;
        private List<DrawnActor3D> drawList;
        #endregion

        #region Properties
        //to do...
        #endregion

        public ObjectManager(Game game, CameraManager cameraManager) : base(game)
        {
            this.cameraManager = cameraManager;
            this.drawList = new List<DrawnActor3D>();
        }

        public void Add(DrawnActor3D actor)
        {
            this.drawList.Add(actor);
        }

        public bool Remove(DrawnActor3D actor)
        {
            return this.drawList.Remove(actor);
        }

        public List<DrawnActor3D> FindAll(Predicate<DrawnActor3D> predicate)
        {
            return this.drawList.FindAll(predicate);
        }

        public override void Update(GameTime gameTime)
        {
            //here we manually call a draw on the actor
            foreach (DrawnActor3D actor in this.drawList)
                actor.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //here we manually call a draw on the actor
            foreach (DrawnActor3D actor in this.drawList)
                actor.Draw(gameTime, this.cameraManager.ActiveCamera);

            base.Draw(gameTime);
        }
    }
}