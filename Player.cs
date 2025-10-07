using OpenTK;
using OpenTK.Graphics.OpenGL;

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
            tileSize + (tileSize / 2f) - (Width / 2),
            (map.GetLength(0) * tileSize) - (tileSize + (tileSize / 2f) + (Height / 2))
        );
    }

    //Drawing player
    public void Draw(float playerDeltaX, float playerDeltaY)
    {
        GL.Color3(0f, 0f, 1f);
        GL.Begin(PrimitiveType.Quads);
        //Left top
        GL.Vertex2(Position.X, Position.Y);
        //Right top
        GL.Vertex2(Position.X + Width, Position.Y);
        //Right bottom
        GL.Vertex2(Position.X + Width, Position.Y + Height);
        //Left bottom
        GL.Vertex2(Position.X, Position.Y + Height);
        GL.End();
    }
}