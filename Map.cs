using System.Collections.Generic;
using System.IO;
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

class Map
{
    //Reading map from file
    public static (int[,], HashSet<string>) Read()
    {
        int[,] map = null;
        string[] lines = File.ReadAllLines(@"source\map.txt");

        //Collection d’erreurs possibles
        HashSet<string> errors = new HashSet<string>();
        int firstLineLength = lines[0].Length;
        string error01 = "Error01: Every line must have the same length!";
        string error02 = "Error02: The map must be at least 3x3 in size!";
        string error03 = "Error03: The map needs to be a max of 20x20!";
        string error04 = "Error04: Player spawn blocked! Make sure the 2nd element of the 2nd-to-last row is 0.";

        //Scanning for errors
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length != firstLineLength)
            {
                errors.Add(error01);
            }
            if (lines[i].Length < 3 || lines.Length < 3)
            {
                errors.Add(error02);
            }
            if (lines[i].Length > 20)
            {
                errors.Add(error03);
            }
            if (lines[lines.Length - 2][1] != '0')
            {
                errors.Add(error04);
            }
        }

        //Reading map into variable if no errors were found
        map = new int[lines.Length, firstLineLength];

        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                map[i, j] = lines[i][j] - '0';
            }
        }

        return (map, errors);
    }

    //Drawing map
    public static void Draw(GameWindow DebugScreen, int[,] map, int tileSize)
    {
        int mapX = map.GetLength(0);
        int mapY = map.GetLength(1);

        for (int x = 0; x < map.GetLength(1); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                int xo = x * tileSize;
                int yo = y * tileSize;
                if (map[y, x] == 0) { GL.Color3(1f, 1f, 1f); }
                else { GL.Color3(0f, 0f, 0f); }
                GL.Begin(PrimitiveType.Quads);
                //Left top
                GL.Vertex2(tileSize * x + 1, tileSize * y + 1);
                //Right top
                GL.Vertex2(tileSize * x + tileSize - 1, tileSize * y + 1);
                //Right bottom
                GL.Vertex2(tileSize * x + tileSize - 1, tileSize * y + tileSize - 1);
                //Left bottom
                GL.Vertex2(tileSize * x + 1, tileSize * y + tileSize - 1);
                GL.End();
            }
        }
    }
}