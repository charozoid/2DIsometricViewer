using SFML.Graphics;
using SFML.System;
using SFML.Window;
class Program
{
    public const int WIDTH = 960;
    public const int HEIGHT = 600;
    public static VideoMode mode = new VideoMode(WIDTH, HEIGHT);
    public static RenderWindow window = new RenderWindow(mode, "Isometric");
    public static Texture cubeText = new Texture("../../Assets/spritesheet.png");
    public static Tile[,] tiles = new Tile[32,32];
    public static View view = new View(new FloatRect(0, 0, WIDTH, HEIGHT));
    public static float CameraSpeed = 10.0f;
    public static float ZoomFactor = 1.05f;
    public static float amplitude = 0.25f;
    public static float frequency = 2f;
    public static Random random = new Random();
    public static Clock clock = new Clock();
    public static void Main(string[] args)
    {

        window.SetVerticalSyncEnabled(true);
        window.Closed += (sender, args) => window.Close();

        while (window.IsOpen)
        {
            window.SetView(view);
            window.Clear();
            DrawGrid();
            KeyPressed();

            window.DispatchEvents();
            window.Display();
        }
    }
    public static void DrawGrid()
    {
        int offset = 512;
        Sprite sprite = new Sprite(cubeText);
        sprite.TextureRect = new IntRect(64, 0, 32, 32);
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                sprite.Position = TransformVector(i, j);
                window.Draw(sprite);
            }
        }
    }
    public static void KeyPressed()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
        {
            view.Center += new Vector2f(0, -CameraSpeed);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
        {
            view.Center += new Vector2f(0, CameraSpeed);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
        {
            view.Center += new Vector2f(-CameraSpeed, 0);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
        {
            view.Center += new Vector2f(CameraSpeed, 0);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.PageDown))
        {
            view.Zoom(ZoomFactor);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.PageUp))
        {
            view.Zoom(1.0f / ZoomFactor);
        }
    }

    public static Vector2f TransformVector(int x, int y)
    {
        float newx = (x * 16) + (y * -16);
        float newy = (x * 8) + (y * 8);

        return new Vector2f(newx + 496, newy);
    }
}

class Tile
{
    public int x = 0;
    public int y = 0;
    public float drawx = 0;
    public float drawy = 0;
    public Sprite sprite = new Sprite(Program.cubeText);

    public Vector2f AddSinOffsetToSprite(float offset)
    {
        float posy = sprite.Position.Y;
        return new Vector2f(sprite.Position.X, sprite.Position.Y + offset);
    }
    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
        sprite.TextureRect = new IntRect(0, 0, 32, 32);
    }
}