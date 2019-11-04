/*
Function: 		Represents the parent interface for all game objects. 
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

//Base class from which all drawn, collidable, non-collidable, trigger volumes, and camera implement
namespace GDLibrary
{
    public interface IActor
    {
        string GetID();

        bool Remove();
        void Update(GameTime gameTime);

        void AttachController(IController controller);
        bool DetachController(IController controller);
        int DetachControllers(Predicate<IController> predicate);
    }
}