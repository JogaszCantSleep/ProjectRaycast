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
        float tempPositionX = 0;
        float tempPositionY = 0;
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

                //X ahonnan rajzolunk
                tempPlayerPositionX = player.Position.X;
                //Y ahonna rajzolunk
                tempPlayerPositionY = player.Position.Y;
                //Amennyivel eltoljuk az X-et
                tempPositionX = tileSize - (tempPlayerPositionX % tileSize);
                //Amennyivel eltoljuk az Y-t
                tempPositionY = (tileSize - (player.Position.X % tileSize)) * (float)Math.Tan((2 * PI) - playerViewAngle);

                //Casting the lazerz (I'KNOW IT'S A RAY OKAY?...)

                bool isVerticalFound = false;
                bool isHorizontalFound = false;

                //Casting rays until hitting a wall
                int mapCheckingRow = (int)(tempPlayerPositionY / tileSize);
                int mapCheckingCol = (int)(tempPlayerPositionX / tileSize) + 1;

                int tempIterator = 1;

                //If view angle is between 0 and PI/2 (1. quadrant)
                while (mapCheckingCol < map.GetLength(1) && !isVerticalFound)
                {
                    if (map[mapCheckingRow, mapCheckingCol] == 1)
                    {
                        GL.Color3(1f, 0f, 0f);
                        GL.LineWidth(1f);
                        GL.Begin(PrimitiveType.Triangles);
                        GL.Vertex2(tempPlayerPositionX, tempPlayerPositionY);
                        GL.Vertex2(tempPlayerPositionX + tempPositionX, tempPlayerPositionY);
                        GL.Vertex2(tempPlayerPositionX + tempPositionX, tempPlayerPositionY - tempPositionY);
                        GL.End();

                        GL.Color3(1f, 0f, 0f);
                        GL.LineWidth(1f);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex2(player.Position.X, player.Position.Y);
                        GL.Vertex2(player.Position.X + playerDeltaMovementX * 1000, player.Position.Y + playerDeltaMovementY * 1000);
                        GL.End();
                        isVerticalFound = true;
                    }
                    else
                    {
                        
                        GL.Color3(1f, 0f, 0f);
                        GL.LineWidth(1f);
                        GL.Begin(PrimitiveType.Triangles);
                        GL.Vertex2(tempPlayerPositionX, tempPlayerPositionY);
                        GL.Vertex2(tempPlayerPositionX + tempPositionX, tempPlayerPositionY);
                        GL.Vertex2(tempPlayerPositionX + tempPositionX, tempPlayerPositionY - tempPositionY);
                        GL.End();

                        Console.WriteLine("=================================");
                        Console.WriteLine("Kör: " + tempIterator);
                        Console.WriteLine("tempPlayerPositionX: " + tempPlayerPositionX);
                        Console.WriteLine("tempPlayerPositionY: " + tempPlayerPositionY);
                        Console.WriteLine("tempPositionX: " + tempPositionX);
                        Console.WriteLine("tempPositionY: " + tempPositionY);

                        tempIterator += 1;

                        //Amennyivel eltoljuk az X-et
                        tempPositionX = tileSize;
                        //Y ahonna rajzolunk
                        tempPlayerPositionY -= (tileSize * (float)Math.Tan((2 * PI) - playerViewAngle));
                        //X ahonnan rajzolunk
                        tempPlayerPositionX += tileSize - (tempPlayerPositionX % tileSize);
                        //Amennyivel eltoljuk az Y-t
                        tempPositionY += tileSize * (float)Math.Tan((2 * PI) - playerViewAngle);
                        mapCheckingCol += 1;
                    }
                };

                DebugScreen.SwapBuffers();
            };

            //Starting the debug screen
            DebugScreen.Run();
        }
    }
}