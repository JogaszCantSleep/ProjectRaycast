using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Security.Policy;

class Player
{
    //Declaring player variables and calculating center of the starting position
    public Vector2 Position;
    public int Width;
    public int Height;
    public Player(int[,] map, int tileSize)
    {
        Width = 10;
        Height = 10;
        Position = new Vector2(
            tileSize + (tileSize / 2f),
            (map.GetLength(0) * tileSize) - (tileSize + (tileSize / 2f))
        );
    }

    //Drawing player
    public void Draw(float playerDeltaX, float playerDeltaY)
    {
        GL.Color3(0f, 0f, 1f);
        GL.Begin(PrimitiveType.Quads);
        GL.Vertex2(Position.X - Width / 2f, Position.Y - Height / 2f); // Bal felső
        GL.Vertex2(Position.X + Width / 2f, Position.Y - Height / 2f); // Jobb felső
        GL.Vertex2(Position.X + Width / 2f, Position.Y + Height / 2f); // Jobb alsó
        GL.Vertex2(Position.X - Width / 2f, Position.Y + Height / 2f); // Bal alsó
        GL.End();
    }
}