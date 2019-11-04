/*
Function: 		All vertex data objects will implement this interface
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public interface IVertexData
    {
        void Draw(GameTime gameTime, Effect effect);
    }
}
