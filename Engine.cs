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
        float movementSpeed = 100f;
        float playerAngle = 0f;
        float rotationSpeed = 2f;

        //DeltaTime
        float lastTime = 0.0167f;
        float playerDeltaMovementX = 0f;
        float playerDeltaMovementY = 0f;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        //Raycasting variables
        int rayCount = 3;
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
                    playerAngle -= rotationSpeed * deltaTime;
                    //If something is wrong in the future with rotation and wallchecking, change this to playerAngle -= 0.1f;
                    if (playerAngle < 0)
                    {
                        playerAngle += (2 * PI);
                    }
                    playerDeltaMovementX = (float)Math.Cos(playerAngle);
                    playerDeltaMovementY = (float)Math.Sin(playerAngle);
                };

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
                };

                //Forward
                if (keyboard.IsKeyDown(Key.W))
                {
                    player.Position.X += (float)Math.Cos(playerAngle) * deltaMovementSpeed;
                    player.Position.Y += (float)Math.Sin(playerAngle) * deltaMovementSpeed;
                };

                //Backward
                if (keyboard.IsKeyDown(Key.S))
                {
                    player.Position.X -= (float)Math.Cos(playerAngle) * deltaMovementSpeed;
                    player.Position.Y -= (float)Math.Sin(playerAngle) * deltaMovementSpeed;
                };

                GL.ClearColor(0.6f, 0.6f, 0.6f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                Map.Draw(DebugScreen, map, tileSize);
                player.Draw();

                float rayOffset = -((rayCount / 2) * 1f);
                for (int i = 0; i < rayCount; i++) {
                    rayOffset += 1f;
                    GL.Color3(1f, 0f, 0f);
                    GL.LineWidth(1f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(player.Position.X, player.Position.Y);
                    GL.Vertex2(player.Position.X + (float)Math.Cos(playerAngle + rayOffset) * 300, player.Position.Y + (float)Math.Sin(playerAngle + rayOffset) * 300);
                    GL.End();
                }

                /*
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

                DebugScreen.SwapBuffers();
            };

            //Starting the debug screen
            DebugScreen.Run();
        }
    }
}