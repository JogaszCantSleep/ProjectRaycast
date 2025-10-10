using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

class Engine
{
    //Declaring variables
    const float PI = (float)Math.PI;
    float playerAngle = 0f;
    int DebugScreenWidth = Map.(map).GetLength(1) * tileSize;
    int DebugScreenHeight = map.GetLength(0) * tileSize;

    //DeltaTime
    float lastTime = 0.0167f;
    Stopwatch stopWatch = new Stopwatch();
    stopWatch.Start();

        //Raycasting variables
        //FOV
        const int FOV = 70;
    //Resolution
    int rayCount = 30;

    float radBetweenRay = ((float)(FOV * (PI / 180f)) / (rayCount - 1));
    float FOVStart = -((float)(FOV * (PI / 180f)) / 2);

    float tempOffsetX = 0;
    float tempOffsetY = 0;
    float tempPlayerPositionX = 0;
    float tempPlayerPositionY = 0;

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

                GL.ClearColor(0.6f, 0.6f, 0.6f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                Map.Draw(DebugScreen, map, tileSize);
                player.Move(PI, playerAngle);
                player.Draw();

                for (int i = 0; i < rayCount; i++) {
                    GL.Color3(1f, 0f, 0f);
                    GL.LineWidth(1f);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex2(player.Position.X, player.Position.Y);
                    GL.Vertex2(player.Position.X + (float)Math.Cos(playerAngle + FOVStart) * 300, player.Position.Y + (float)Math.Sin(playerAngle + FOVStart) * 300);
                    GL.End();
                    FOVStart += radBetweenRay;
                }
                FOVStart = -(float)((FOV * (PI / 180f)) / 2);

                GL.Color3(0f, 1f, 0f);
                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(player.Position.X, player.Position.Y);
                GL.Vertex2(player.Position.X + playerDeltaMovementX * 300, player.Position.Y + playerDeltaMovementY * 300);
                GL.End();

                DebugScreen.SwapBuffers();
            };

            //Starting the debug screen
            DebugScreen.Run();
        }
    }
}