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
        int tileSize = 48;

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

        //Player speed and angle
        float movementSpeed = 40f;
        float rotationSpeed = 0.05f;
        float playerAngle = 0f;

        //DeltaTime
        float lastTime = 0.0167f;
        float playerDeltaX = 0f;
        float playerDeltaY = 0f;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        //Raycasting variables


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
                float deltaRotationSpeed = rotationSpeed * deltaTime;

                //Movement and strife calculations
                var keyboard = Keyboard.GetState();

                //Forward
                if (keyboard.IsKeyDown(Key.W)) {
                    player.Position.X += playerDeltaX;
                    player.Position.Y += playerDeltaY;
                };

                //Strife left
                if (keyboard.IsKeyDown(Key.A)) {
                    playerAngle -= rotationSpeed;
                    if (playerAngle < 0) {
                        playerAngle += 2 * PI;
                    }
                    playerDeltaX = (float)Math.Cos(playerAngle) * 5;
                    playerDeltaY = (float)Math.Sin(playerAngle) * 5;
                };

                //Backward
                if (keyboard.IsKeyDown(Key.S)) {
                    player.Position.X -= playerDeltaX;
                    player.Position.Y -= playerDeltaY;
                };

                //Strife right
                if (keyboard.IsKeyDown(Key.D)) {
                    playerAngle += rotationSpeed;
                    if (playerAngle > 2 * PI) {
                        playerAngle -= 2 * PI;
                    }
                    playerDeltaX = (float)Math.Cos(playerAngle) * 5;
                    playerDeltaY = (float)Math.Sin(playerAngle) * 5;
                };

                bool isVerticalFound = false;
                //bool isHorizontalFound = false;

                //Casting rays until hitting a wall
                if (playerAngle > 0 && playerAngle <= (PI * 2)) {
                    
                };


                /*
                //View angle
                //Up
                if (playerAngle >= PI / 4 && playerAngle < (3 * PI) / 4) {
                
                };
                
                //Left
                if (playerAngle >= (3 * PI) / 4 && playerAngle < (5 * PI) / 4) {
                
                };

                //Down
                if (playerAngle >= (5 *PI) / 4 && playerAngle < (7 * PI) / 4) {
                
                };
                
                //Right
                if (playerAngle >= (7 * PI) / 4 && playerAngle < PI / 4) {
                
                };
                */

                int playerOnMapX = (int)Math.Floor(player.Position.X / tileSize);
                int playerOnMapY = (int)Math.Floor(player.Position.Y / tileSize);
                
                while (map[(playerOnMapX - 1), playerOnMapY] != 1) {
                    playerOnMapX += 1;
                };

                playerOnMapX -= ((int)Math.Floor(player.Position.X / tileSize) + 1);
                
                Console.WriteLine("playerOnMapX: " + (int)Math.Floor(player.Position.X / tileSize) + "\nExtendedX: " + (playerOnMapX * tileSize + player.Position.X));
                
                GL.ClearColor(0.6f, 0.6f, 0.6f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                
                Map.Draw(DebugScreen, map, tileSize);
                player.Draw(playerDeltaX, playerDeltaY);

                //Casting the lazerz (I'KNOW IT'S A RAY OKAY?...)
                for (int i = 0; i < 1; i++)
                {
                    GL.Color3(1f, 0f, 0f);
                    GL.LineWidth(1f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(player.Position.X + (player.Width / 2), player.Position.Y + (player.Width / 2));
                    GL.Vertex2(player.Position.X + (playerDeltaX * 100) + (player.Width / 2), player.Position.Y + (playerDeltaY * 100) + (player.Width / 2));
                    GL.End();
                }

                DebugScreen.SwapBuffers();
            };

            //Starting the debug screen
            DebugScreen.Run();
        }
    }
}