﻿/*
Function: 		Represents the parent interface for all object controllers. A controller can be attached to an actor and modify its state e.g.
                a FirstPersonCameraController attached to a Camera3D actor will handle keyboard and mouse input and translate into changes in the actors Transform3D properties.
Author: 		NMCG
Version:		1.1
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public interface IController : ICloneable
    {
        //Update the actor controller by this controller
        void Update(GameTime gameTime, IActor actor);

        //Reset the actor controller by this controller
        void SetActor(IActor actor);

        //Used when we want to interrogate a controller and see if it is "the one" that we want to enable/disable, based on ID.
        string GetID();

        //Allows us to play, pause, reset, stop a controller
        void SetPlayStatus(PlayStatusType playStatusType);
        PlayStatusType GetPlayStatus();
        ControllerType GetControllerType();
    }
}