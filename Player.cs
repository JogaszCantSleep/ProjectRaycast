using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
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
    public void Draw()
    {
        GL.Color3(0f, 0f, 1f);
        GL.Begin(PrimitiveType.Quads);
        GL.Vertex2(Position.X - Width / 2f, Position.Y - Height / 2f); // Bal felső
        GL.Vertex2(Position.X + Width / 2f, Position.Y - Height / 2f); // Jobb felső
        GL.Vertex2(Position.X + Width / 2f, Position.Y + Height / 2f); // Jobb alsó
        GL.Vertex2(Position.X - Width / 2f, Position.Y + Height / 2f); // Bal alsó
        GL.End();
    }

    public void Move(float PI, float playerAngle) {
        //Player movement, speed and angle
        float movementSpeed = 100f;
        float rotationSpeed = 2f;
        float playerDeltaMovementX = (float)Math.Cos(playerAngle);
        float playerDeltaMovementY = (float)Math.Sin(playerAngle);

        //Movement and strife calculations
        var keyboard = Keyboard.GetState();

        //Strife left
        if (keyboard.IsKeyDown(Key.A))
        {
            playerAngle -= rotationSpeed * deltaTime;
            //If something is wrong in the future with rotation and wallchecking, change this to playerAngle -= 0.1f;
            if (playerAngle < 0)
            {
                playerAngle += (2 * PI);
            }
            playerDeltaMovementX = (float)Math.Cos(playerAngle);
            playerDeltaMovementY = (float)Math.Sin(playerAngle);
        }
        ;

        //Strife right
        if (keyboard.IsKeyDown(Key.D))
        {
            playerAngle += rotationSpeed * deltaTime;
            //If something is wrong in the future with rotation and wallchecking, change this to playerAngle += 0.1f;
            if (playerAngle > (2 * PI))
            {
                playerAngle -= (2 * PI);
            }
            playerDeltaMovementX = (float)Math.Cos(playerAngle);
            playerDeltaMovementY = (float)Math.Sin(playerAngle);
        }
        ;

        //Forward
        if (keyboard.IsKeyDown(Key.W))
        {
            Position.X += (float)Math.Cos(playerAngle) * deltaMovementSpeed;
            Position.Y += (float)Math.Sin(playerAngle) * deltaMovementSpeed;
        }
        ;

        //Backward
        if (keyboard.IsKeyDown(Key.S))
        {
            Position.X -= (float)Math.Cos(playerAngle) * deltaMovementSpeed;
            Position.Y -= (float)Math.Sin(playerAngle) * deltaMovementSpeed;
        }
        ;
    }
}

/*
Trianlge-mesh engine

                //Casting the rays
                bool isVerticalFound = false;
                bool isHorizontalFound = false;

                tempPlayerPositionX = player.Position.X;
                tempPlayerPositionY = player.Position.Y;
                tempOffsetX = tileSize - (tempPlayerPositionX % tileSize);
                tempOffsetY = (tileSize - (player.Position.X % tileSize)) * (float)Math.Tan((2 * PI) - playerAngle);

                int mapCheckingRow = (int)(player.Position.Y / tileSize);
                int mapCheckingCol = (int)(player.Position.X / tileSize);


                if (playerAngle > 0f && ((2 * PI) - playerAngle) < (PI / 2)) {
                    while (!isVerticalFound)
                    {
                        if (mapCheckingCol < map.GetLength(1) && mapCheckingRow > 0) {
                            if (map[mapCheckingRow, mapCheckingCol] == 1)
                            {
                                GL.Color3(1f, 0f, 0f);
                                GL.LineWidth(1f);
                                GL.Begin(PrimitiveType.Lines);
                                GL.Vertex2(player.Position.X, player.Position.Y);
                                GL.Vertex2(tempPlayerPositionX + tempOffsetX, tempPlayerPositionY - tempOffsetY);
                                GL.End();
                                isVerticalFound = true;
                            }
                            tempOffsetX = tileSize;
                            tempPlayerPositionX += tileSize - (tempPlayerPositionX % tileSize);
                            tempPlayerPositionY -= tempOffsetY;
                            tempOffsetY = tempOffsetX * (float)Math.Tan((2 * PI) - playerAngle);
                            mapCheckingRow = (int)((tempPlayerPositionY - tempOffsetY) / tileSize);
                            mapCheckingCol = (int)((tempPlayerPositionX + tempOffsetX) / tileSize);
                        }
                    };
                }*/