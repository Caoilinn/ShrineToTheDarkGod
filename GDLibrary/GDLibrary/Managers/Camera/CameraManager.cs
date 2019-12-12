/*
Function: 		Stores and organises the cameras available within the game (used single and split screen layouts) 
                WORK IN PROGRESS - at present this class is only a barebones class to be used by the ObjectManager 
Author: 		NMCG
Date Updated:	
Bugs:			None
Fixes:			Added IEnumberable
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class CameraManager : PausableGameComponent, IEnumerable<Camera3D>
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

        public int ActiveCameraIndex
        {
            get
            {
                return this.activeCameraIndex;
            }
            set
            {
                this.activeCameraIndex = (value >= 0 && value < this.cameraList.Count) ? value : 0;
            }
        }

        public Camera3D this[int index]
        {
            get
            {
                if (this.cameraList[index] == null)
                    throw new Exception("Camera " + index + " does not exist.");

                return this.cameraList[index];
            }
        }
        #endregion

        #region Constructors
        public CameraManager(
            Game game, 
            int initialSize, 
            EventDispatcher eventDispatcher,
            StatusType statusType
        ) : base(game, eventDispatcher, statusType) {
            this.cameraList = new List<Camera3D>(initialSize);
        }
        #endregion

        #region Methods
        public void Add(Camera3D camera)
        {
            //First time in ensures that we have a list
            if (this.cameraList.Count == 0)
                this.activeCameraIndex = 0;

            this.cameraList.Add(camera);

            this.cameraList.Sort((a, b) => (a.DrawDepth <= b.DrawDepth ? 1 : -1));
        }

        public Camera3D GetCameraByID(string ID)
        {
            return this.cameraList.Find(x => x.ID.Equals(ID));
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

        public void CycleActiveCamera()
        {
            this.ActiveCameraIndex = this.activeCameraIndex + 1;
        }

        public bool SetActiveCamera(Predicate<Camera3D> predicate)
        {
            int index = this.cameraList.FindIndex(predicate);
            this.ActiveCameraIndex = index;
            return (index != -1) ? true : false;
        }

        //Sorts cameras by Camera3D::drawDepth - used for PIP screen layout - see ScreenManager
        public void SortByDepth(SortDirectionType sortDirectionType)
        {
            this.cameraList.Sort(new CameraDepthComparer(sortDirectionType));
        }

        protected override void ApplyUpdate(GameTime gameTime)
        {
            /* 
             * Update all the cameras in the list.
             * Remember that at the moment only 1 camera is visible so this foreach loop seems counter-intuitive.
             * Assuming each camera in the list had some form of automatic movement (e.g. like a security camera) then what would happen if we only updated the active camera?
             */
            foreach (Camera3D camera in this.cameraList)
                camera.Update(gameTime);
           
            base.ApplyUpdate(gameTime);
        }

        public IEnumerator<Camera3D> GetEnumerator()
        {
            return this.cameraList.GetEnumerator();
        }

        //This method is called by any external foreach() loop with a handle to the camera manager
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); //calls IEnumerator<Camera3D> GetEnumerator() above
        }
        #endregion
    }
}