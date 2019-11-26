/*
Function: 		Defines static methods to return common primitive geometry
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary
{
    public class VertexFactory
    {
        /// <summary>
        /// Returns a VertexPositionColorTexture array describing a quad (drawn using TriangleStrip) 
        /// </summary>
        /// <returns></returns>
        public static VertexPositionColorTexture[] GetVertexPositionColorTextureQuad()
        {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];

            vertices[0] = new VertexPositionColorTexture(
                new Vector3(-0.5f, -0.5f, 0), Color.White, new Vector2(0, 1)); //BL
            vertices[1] = new VertexPositionColorTexture(
                new Vector3(-0.5f, 0.5f, 0), Color.White, new Vector2(0, 0)); //TL
            vertices[2] = new VertexPositionColorTexture(
                new Vector3(0.5f, -0.5f, 0), Color.White, new Vector2(1, 1)); //BR
            vertices[3] = new VertexPositionColorTexture(
                new Vector3(0.5f, 0.5f, 0), Color.White, new Vector2(1, 0)); //TR

            return vertices;
        }
    }
}
