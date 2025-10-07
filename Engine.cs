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
        float tempX = 0;
        float tempY = 0;
        float tempPlayerX = 0;
        float tempPlayerY = 0;


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

                GL.ClearColor(0.6f, 0.6f, 0.6f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                
                Map.Draw(DebugScreen, map, tileSize);
                player.Draw(playerDeltaX, playerDeltaY);

                //Casting the lazerz (I'KNOW IT'S A RAY OKAY?...)
                /*GL.Color3(1f, 0f, 0f);
                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.Triangles);
                GL.Vertex2(player.Position.X, player.Position.Y);
                GL.Vertex2((player.Position.X + (player.Position.X % tileSize)), player.Position.Y - Math.Tan(2 * PI - playerAngle) * (player.Position.X % tileSize));
                GL.Vertex2((player.Position.X + (player.Position.X % tileSize)), player.Position.Y);
                GL.End();*/

                bool isVerticalFound = false;
                bool isHorizontalFound = false;

                //X ahonnan rajzolunk
                tempPlayerX = player.Position.X;
                //Y ahonna rajzolunk
                tempPlayerY = player.Position.Y;
                //Amennyivel eltoljuk az X-et
                tempX = tempPlayerX % tileSize;
                //Amennyivel eltoljuk az Y-t
                tempY = (float)Math.Tan(2 * PI - playerAngle) * tempX;

                Console.WriteLine("tempPlayerX: " + tempPlayerX + "\ntempPlayerY: " + tempPlayerY + "\ntempX: " + tempX + "\ntempY: " + tempY);

                //Casting rays until hitting a wall
                /*if (playerAngle > 0 && playerAngle < (PI / 2))
                {
                    while (isVerticalFound == false)
                    {
                        GL.Color3(1f, 0f, 0f);
                        GL.LineWidth(1f);
                        GL.Begin(PrimitiveType.Triangles);
                        GL.Vertex2(tempPlayerX, tempPlayerY);
                        GL.Vertex2(tempPlayerX + tempX, tempPlayerY + tempY);
                        GL.Vertex2(tempPlayerX + tempX, tempPlayerY);
                        GL.End();
                        if (map[(int)(tempPlayerX / tileSize), (int)(tempPlayerY /  tileSize)] == 1)
                        {
                            GL.Color3(1f, 0f, 0f);
                            GL.LineWidth(1f);
                            GL.Begin(PrimitiveType.Lines);
                            GL.Vertex2(player.Position.X, player.Position.Y);
                            GL.Vertex2(tempPlayerX + tempX, tempPlayerY + tempY);
                            GL.End();
                            isVerticalFound = true;
                        }
                        else
                        {
                            //X ahonnan rajzolunk
                            tempPlayerX += tempX;
                            //Y ahonna rajzolunk
                            tempPlayerY += tempY;
                            //Amennyivel eltoljuk az X-et
                            tempX = tileSize;
                            //Amennyivel eltoljuk az Y-t
                            tempY = (float)Math.Tan(2 * PI - playerAngle) * tempX;
                        }
                    }
                };*/

                for (int i = 0; i < 3; i++) {
                    GL.Color3(1f, 0f, 0f);
                    GL.LineWidth(1f);
                    GL.Begin(PrimitiveType.Triangles);
                    GL.Vertex2(tempPlayerX, tempPlayerY);
                    GL.Vertex2(tempPlayerX + tempX, tempPlayerY + tempY);
                    GL.Vertex2(tempPlayerX + tempX, tempPlayerY);
                    GL.End();
                    Console.WriteLine("tempPlayerX: " + tempPlayerX + "\ntempPlayerY: " + tempPlayerY + "\ntempX: " + tempX + "\ntempY: " + tempY);
                    if (map[(int)(tempPlayerX / tileSize), (int)(tempPlayerY / tileSize)] == 1)
                    {
                        GL.Color3(1f, 0f, 0f);
                        GL.LineWidth(1f);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex2(player.Position.X, player.Position.Y);
                        GL.Vertex2(tempPlayerX + tempX, tempPlayerY + tempY);
                        GL.End();
                        isVerticalFound = true;
                    }
                    else
                    {
                        //X ahonnan rajzolunk
                        tempPlayerX += tempX;
                        //Y ahonna rajzolunk
                        tempPlayerY += tempY;
                        //Amennyivel eltoljuk az X-et
                        tempX = tileSize;
                        //Amennyivel eltoljuk az Y-t
                        tempY = (float)Math.Tan(2 * PI - playerAngle) * tempX;
                    }
                }

                DebugScreen.SwapBuffers();
            };

            //Starting the debug screen
            DebugScreen.Run();
        }
    }
}