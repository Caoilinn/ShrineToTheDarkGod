/*
Function: 		Stores and organises the cameras available within the game (used single and split screen layouts) 
                WORK IN PROGRESS - at present this class is only a barebones class to be used by the ObjectManager 
Author: 		NMCG
Date Updated:	
Bugs:			None
Fixes:			Added IEnumberable
*/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary
{

    public class CameraManager : GameComponent
    {

        #region Fields
        private List<Camera3D> cameraList;
        private int activeCameraIndex = -1;
        #endregion

        #region Properties
        public Camera3D ActiveCamera
        {
            get
            {
                return this.cameraList[this.activeCameraIndex];
            }
        }
        #endregion

        public CameraManager(Game game) 
            : base(game)
        {
            
        }

        public int SetActiveCameraIndex(int index)
        {
            //if user wants to set a valid index then allow, otherwise set to 0.
            this.activeCameraIndex = (index >= 0 && index < this.cameraList.Count) ? index : 0;
            return this.activeCameraIndex;
        }
        public void Add(Camera3D camera)
        {
            //first time in ensures that we have a list
            if (this.cameraList == null)
            {
                this.cameraList = new List<Camera3D>();
                //set the first camera in the list to be active, until we call SetActiveCamera() later
                this.activeCameraIndex = 0;
            }
            this.cameraList.Add(camera);
        }

        public bool Remove(Predicate<Camera3D> predicate)
        {
            Camera3D foundCamera = this.cameraList.Find(predicate);
            if (foundCamera != null)
                return this.cameraList.Remove(foundCamera);

            return false;
        }

        public int RemoveAll(Predicate<Camera3D> predicate)
        {
            return this.cameraList.RemoveAll(predicate);
        }
      
        //to do - Add SetActiveCameraBy() and CycleCamera()
        public int CycleCamera()
        {
            this.activeCameraIndex++;
            this.activeCameraIndex %= this.cameraList.Count;
            return this.activeCameraIndex;
        }

        public int SetActiveCameraBy(int startIndex, Predicate<Camera3D> predicate)
        {
            int findIndex = this.cameraList.FindIndex(startIndex, predicate);

            if (findIndex != -1)
                this.activeCameraIndex = findIndex;
            
            return findIndex;
        }

        public override void Update(GameTime gameTime)
        {
            /* 
             * Update all the cameras in the list.
             * Remember that at the moment only 1 camera is visible so this foreach loop seems counter-intuitive.
             * Assuming each camera in the list had some form of automatic movement (e.g. like a security camera) then what would happen if we only updated the active camera?
             */
            foreach (Camera3D camera in this.cameraList)
                camera.Update(gameTime);
           
            base.Update(gameTime);
        }
    }
}
