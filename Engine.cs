using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

class Engine
{
    static void Main()
    {
        //Reading map from file, checking for errors and giving it to a 2D array
        var (map, errors) = Map.Read();
        int tileSize = 50;

        //If there were no errors, continue running the program
        if (errors.Count > 0) {
            foreach (string error in errors)
            {
                Console.WriteLine(error);
            }
            map = null;
            return;
        };

        //Declaring variables
        const float PI = (float)Math.PI;
        int DebugScreenWidth = map.GetLength(1) * tileSize;
        int DebugScreenHeight = map.GetLength(0) * tileSize;

        //Player movement, speed and angle
        float playerDeltaX = 1f;
        float playerDeltaY = 0f;
        float movementSpeed = 100f;
        float playerViewAngle = 0f;
        float rotationSpeed = 2f;

        //DeltaTime
        float lastTime = 0.0167f;
        float playerDeltaMovementX = 0f;
        float playerDeltaMovementY = 0f;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        //Raycasting variables
        float tempOffsetX = 0;
        float tempOffsetY = 0;
        float tempPlayerPositionX = 0;
        float tempPlayerPositionY = 0;


        using (GameWindow DebugScreen = new GameWindow(DebugScreenWidth, DebugScreenHeight, GraphicsMode.Default, "Debug Screen"))
        {
            Player player = new Player(map, tileSize);

            //Setting up pixel coordinates, frame rendering and window resizing handlers
            Renderer.SetupPixelCoordinates(DebugScreen);

            //Rendering frames and drawing the map
            DebugScreen.RenderFrame += (sender, e) =>
            {
                //DeltaTime calculation
                float currentTime = (float)stopWatch.Elapsed.TotalSeconds;
                float deltaTime = currentTime - lastTime;
                lastTime = currentTime;
                float deltaMovementSpeed = movementSpeed * deltaTime;

                //Movement and strife calculations
                var keyboard = Keyboard.GetState();

                //Strife left
                if (keyboard.IsKeyDown(Key.A))
                {
                    playerViewAngle -= rotationSpeed * deltaTime;
                    //If something is wrong in the future with rotation and wallchecking, change this to playerViewAngle -= 0.1f;
                    if (playerViewAngle < 0)
                    {
                        playerViewAngle += (2 * PI);
                    }
                    playerDeltaMovementX = (float)Math.Cos(playerViewAngle);
                    playerDeltaMovementY = (float)Math.Sin(playerViewAngle);
                };

                //Strife right
                if (keyboard.IsKeyDown(Key.D))
                {
                    playerViewAngle += rotationSpeed * deltaTime;
                    //If something is wrong in the future with rotation and wallchecking, change this to playerViewAngle += 0.1f;
                    if (playerViewAngle > (2 * PI))
                    {
                        playerViewAngle -= (2 * PI);
                    }
                    playerDeltaMovementX = (float)Math.Cos(playerViewAngle);
                    playerDeltaMovementY = (float)Math.Sin(playerViewAngle);
                };

                //Forward
                if (keyboard.IsKeyDown(Key.W))
                {
                    player.Position.X += (float)Math.Cos(playerViewAngle) * deltaMovementSpeed;
                    player.Position.Y += (float)Math.Sin(playerViewAngle) * deltaMovementSpeed;
                };

                //Backward
                if (keyboard.IsKeyDown(Key.S))
                {
                    player.Position.X -= (float)Math.Cos(playerViewAngle) * deltaMovementSpeed;
                    player.Position.Y -= (float)Math.Sin(playerViewAngle) * deltaMovementSpeed;
                };

                GL.ClearColor(0.6f, 0.6f, 0.6f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                Map.Draw(DebugScreen, map, tileSize);
                player.Draw();

                //Casting the rays
                bool isVerticalFound = false;
                bool isHorizontalFound = false;

                tempPlayerPositionX = player.Position.X;
                tempPlayerPositionY = player.Position.Y;
                tempOffsetX = tileSize - (tempPlayerPositionX % tileSize);
                tempOffsetY = (tileSize - (player.Position.X % tileSize)) * (float)Math.Tan((2 * PI) - playerViewAngle);

                int mapCheckingRow = (int)(player.Position.Y / tileSize);
                int mapCheckingCol = (int)(player.Position.X / tileSize);

                //If view angle is between 0 and PI/2 (1. quadrant)
                while (mapCheckingCol < map.GetLength(1) && !isVerticalFound)
                {
                    mapCheckingRow = (int)((tempPlayerPositionY - tempOffsetY) / tileSize);
                    mapCheckingCol = (int)((tempPlayerPositionX + tempOffsetX) / tileSize);
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
                    tempOffsetY = tempOffsetX * (float)Math.Tan((2 * PI) - playerViewAngle);
                };

                DebugScreen.SwapBuffers();
            };

            //Starting the debug screen
            DebugScreen.Run();
        }
    }
}