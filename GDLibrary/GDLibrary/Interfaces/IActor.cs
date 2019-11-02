/*
Function: 		Represents the parent interface for all game objects. 
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/
using Microsoft.Xna.Framework;

//base class from which all drawn, collidable, non-collidable, trigger volumes, and camera implement
namespace GDLibrary
{
    public interface IActor
    {
        void Update(GameTime gameTime);
        string GetID();
        //to do...
    }
}
