using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

class Player
{
    //Player data
    public Vector2 Position;        //Player center position
    public int PlayerWidth;         //Width
    public int PlayerHeight;        //Height
    public Player(int[,] map, int tileSize)
    {
        PlayerWidth = 10;
        PlayerHeight = 10;
        Position = new Vector2(
            tileSize + (tileSize / 2f) - (PlayerWidth / 2),
            (map.GetLength(0) * tileSize) - (tileSize + (tileSize / 2f) + (PlayerHeight / 2))
        );
    }

    public void Draw(float playerDeltaX, float playerDeltaY)
    {
        GL.Color3(0f, 0f, 1f);
        GL.Begin(PrimitiveType.Quads);
        GL.Vertex2(Position.X, Position.Y); //Left top 
        GL.Vertex2(Position.X + PlayerWidth, Position.Y); //Right top
        GL.Vertex2(Position.X + PlayerWidth, Position.Y + PlayerHeight); //Right bottom
        GL.Vertex2(Position.X, Position.Y + PlayerHeight); //Left bottom
        GL.End();

        GL.LineWidth(3f);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(Position.X + (PlayerWidth / 2), Position.Y + (PlayerWidth / 2));
        GL.Vertex2(Position.X + (playerDeltaX * 6.5) + (PlayerWidth / 2), Position.Y + (playerDeltaY * 6.5) + (PlayerWidth / 2));
        GL.End();

        GL.Color3(1f, 0f, 0f);
        GL.LineWidth(1f);
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(Position.X + (PlayerWidth / 2), Position.Y + (PlayerWidth / 2));
        GL.Vertex2(Position.X + (playerDeltaX * 100) + (PlayerWidth / 2), Position.Y + (playerDeltaY * 100) + (PlayerWidth / 2));
        GL.End();
    }
}