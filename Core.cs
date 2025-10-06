using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

class Core
{
    static void Main()
    {
        //Reading map from file, checking for errors and giving it to a 2D array
        var (map, errors) = Map.Read();
        int tileSize = 64;

        if (errors.Count > 0) return;

        int DebugScreenWidth = map.GetLength(1) * tileSize;
        int DebugScreenHeight = map.GetLength(0) * tileSize;

        //Player movement speed
        float speed = 40f;

        //Delta elapsed time variable
        float lastTime = 0.0167f;

        //Starting Stopwatch for delta time calculation
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        const float PI = (float)Math.PI;
        float playerAngle = 0f;
        float playerDeltaX = 0f;
        float playerDeltaY = 0f;

        using (GameWindow DebugScreen = new GameWindow(DebugScreenWidth, DebugScreenHeight, GraphicsMode.Default, "Debug Screen"))
        {
            Player player = new Player(map, tileSize);

            //Setting up pixel coordinates, frame rendering and window resizing handlers
            Renderer.SetupPixelCoordinates(DebugScreen);

            //Rendering frames and drawing the map
            DebugScreen.RenderFrame += (sender, e) =>
            {
                //Current time
                float currentTime = (float)stopWatch.Elapsed.TotalSeconds;

                //Delta time calculation
                float deltaTime = currentTime - lastTime;

                lastTime = currentTime;

                float movementSpeed = speed * deltaTime;

                var keyboard = Keyboard.GetState();

                if (keyboard.IsKeyDown(Key.A)) { playerAngle -= 0.1f; if (playerAngle < 0) { playerAngle += 2 * PI; } playerDeltaX = (float)Math.Cos(playerAngle) * 5; playerDeltaY = (float)Math.Sin(playerAngle) * 5; }
                ;
                if (keyboard.IsKeyDown(Key.W)) player.Position.Y -= movementSpeed;
                if (keyboard.IsKeyDown(Key.S)) player.Position.Y += movementSpeed;
                if (keyboard.IsKeyDown(Key.D)) player.Position.X += movementSpeed;

                GL.ClearColor(0.6f, 0.6f, 0.6f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                Console.WriteLine("playerAngle: " + playerAngle + "\nplayerDeltaX: " + playerDeltaX + "\nplayerDeltaY: " + playerDeltaY);

                Map.Draw(DebugScreen, map, tileSize);
                player.Draw(playerDeltaX, playerDeltaY);

                DebugScreen.SwapBuffers();
            };

            //Running the debug screen
            DebugScreen.Run();

        }
    }
}