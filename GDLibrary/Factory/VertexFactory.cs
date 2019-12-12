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

        //returns the vertices for a billboard which has a custom vertex declaration
        public static VertexBillboard[] GetVertexBillboard(int sidelength, out PrimitiveType primitiveType, out int primitiveCount)
        {
            primitiveType = PrimitiveType.TriangleStrip;
            primitiveCount = 2;

            VertexBillboard[] vertices = new VertexBillboard[4];
            float halfSideLength = sidelength / 2.0f;

            Vector2 uvTopLeft = new Vector2(0, 0);
            Vector2 uvTopRight = new Vector2(1, 0);
            Vector2 uvBottomLeft = new Vector2(0, 1);
            Vector2 uvBottomRight = new Vector2(1, 1);

            //quad coplanar with the XY-plane (i.e. forward facing normal along UnitZ)
            vertices[0] = new VertexBillboard(Vector3.Zero, new Vector4(uvTopLeft, -halfSideLength, halfSideLength));
            vertices[1] = new VertexBillboard(Vector3.Zero, new Vector4(uvTopRight, halfSideLength, halfSideLength));
            vertices[2] = new VertexBillboard(Vector3.Zero, new Vector4(uvBottomLeft, -halfSideLength, -halfSideLength));
            vertices[3] = new VertexBillboard(Vector3.Zero, new Vector4(uvBottomRight, halfSideLength, -halfSideLength));

            return vertices;
        }

        //TriangleStrip
        public static VertexPositionColorTexture[] GetTextureQuadVertices(out PrimitiveType primitiveType, out int primitiveCount)
        {
            float halfLength = 0.5f;

            Vector3 topLeft = new Vector3(-halfLength, halfLength, 0);
            Vector3 topRight = new Vector3(halfLength, halfLength, 0);
            Vector3 bottomLeft = new Vector3(-halfLength, -halfLength, 0);
            Vector3 bottomRight = new Vector3(halfLength, -halfLength, 0);

            //quad coplanar with the XY-plane (i.e. forward facing normal along UnitZ)
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
            vertices[0] = new VertexPositionColorTexture(topLeft, Color.White, Vector2.Zero);
            vertices[1] = new VertexPositionColorTexture(topRight, Color.White, Vector2.UnitX);
            vertices[2] = new VertexPositionColorTexture(bottomLeft, Color.White, Vector2.UnitY);
            vertices[3] = new VertexPositionColorTexture(bottomRight, Color.White, Vector2.One);

            primitiveType = PrimitiveType.TriangleStrip;
            primitiveCount = 2;

            return vertices;
        }
    }
}