using SFML.Graphics;
using SFML.System;
using SFML.Window;
class Program
{
    public const int WIDTH = 960;
    public const int HEIGHT = 600;
    public static VideoMode mode = new VideoMode(WIDTH, HEIGHT);
    public static RenderWindow window = new RenderWindow(mode, "Isometric");
    public static Texture cubeText = new Texture("../../Assets/cube.png");
    public static Tile[,] tiles = new Tile[32,32];
    public static View view = new View(new FloatRect(0, 0, WIDTH, HEIGHT));
    public static float CameraSpeed = 10.0f;
    public static float ZoomFactor = 1.05f;
    public static float amplitude = 0.5f;
    public static float frequency = 2f;
    public static Random random = new Random();
    public static Clock clock = new Clock();
    public static void Main(string[] args)
    {

        window.SetVerticalSyncEnabled(true);
        window.Closed += (sender, args) => window.Close();

        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                tiles[i, j] = new Tile(i, j);
                tiles[i, j].sprite.Position = TransformVector(i, j);
            }
        }

        //tiles[10, 10].sprite.Position = new Vector2f(tiles[10, 10].sprite.Position.X, tiles[10, 10].sprite.Position.Y - 25);
        //tiles[11, 10].sprite.Position = new Vector2f(tiles[11, 10].sprite.Position.X, tiles[11, 10].sprite.Position.Y - 50);
        while (window.IsOpen)
        {
            float deltaTime = clock.Restart().AsSeconds();
            window.SetView(view);
            window.Clear();

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {

                    double seconds = DateTime.Now.Second;
                    double phaseShift = frequency * (0.3 * i + 0.3 * j);
                    double shiftedSeconds = seconds - phaseShift;
                    double sinValue = Math.Sin(shiftedSeconds);
                    Tile tile = tiles[i, j];
                    Vector2f pos = tile.sprite.Position;
                    float baseLine = pos.Y;
                    pos = new Vector2f(pos.X, baseLine + amplitude * (float)sinValue);
                    tile.sprite.Position = pos;
                    window.Draw(tiles[i,j].sprite);

                }
            }
            KeyPressed();

            window.DispatchEvents();
            window.Display();
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
        float newx = (x * 64) + (y * -64);
        float newy = (x * 32) + (y * 32);

        return new Vector2f(newx, newy);
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
    }
}