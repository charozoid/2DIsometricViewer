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
    public static Font font = new Font("../../Assets/Fonts/arialbd.ttf");
    public static float CameraSpeed = 10.0f;
    public static float ZoomFactor = 1.05f;
    public static float amplitude = 0.25f;
    public static float frequency = 2f;
    public static Random random = new Random();
    public static Clock clock = new Clock();
    public static List<Sprite> sprites = new List<Sprite>();
    public static void Main(string[] args)
    {

        window.SetVerticalSyncEnabled(true);
        window.Closed += (sender, args) => window.Close();

        while (window.IsOpen)
        {
            window.SetView(view);
            window.Clear();
            DrawGrid();

            Sprite sprite = new Sprite(cubeText);
            sprite.TextureRect = new IntRect(0, 0, 32, 32);
            Vector2i mousePos = Mouse.GetPosition(window);
            Vector2f worldCoords = window.MapPixelToCoords(mousePos, view);
            Vector2f gridCoords = GetMouseGrid(worldCoords, view, GetZoomFactor(view));
            //Vector2f mousePosF = GetMouseGrid(new Vector2f(mousePos.X, mousePos.Y), view, GetZoomFactor(view));
            sprite.Position = TransformVector(gridCoords);

            Text text = new Text($"X: {gridCoords.X} Y: {gridCoords.Y}", font);
            text.Position = new Vector2f(15, 50);
            window.Draw(text);
            KeyPressed();
            foreach (Sprite tile in sprites)
            {
                window.Draw(tile);
            }
            window.Draw(sprite);
            window.DispatchEvents();
            window.Display();
        }
    }
    public static void DrawGrid()
    {
        Sprite sprite = new Sprite(cubeText);
        sprite.TextureRect = new IntRect(128, 32, 32, 32);
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                sprite.Position = TransformVector(new Vector2f(i + 32, j));                
                window.Draw(sprite);
            }
        }
    }
    public static void CreateBlock(Vector2f position)
    {
        Sprite sprite = new Sprite(cubeText);
        sprite.TextureRect = new IntRect(0, 0, 32, 32);
        sprite.Position = TransformVector(position);
        sprites.Add(sprite);
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
        if (Mouse.IsButtonPressed(Mouse.Button.Left))
        {
            Vector2i mousePos = Mouse.GetPosition(window);
            Vector2f worldCoords = window.MapPixelToCoords(mousePos, view);
            CreateBlock(GetMouseGrid(worldCoords, view, GetZoomFactor(view)));
        }
    }
    static float GetZoomFactor(View view)
    {
        Vector2f size = view.Size;
        float zoomFactor = WIDTH / size.X;

        return zoomFactor;
    }
    public static Vector2f TransformVector(Vector2f vector)
    {
        float x = vector.X;
        float y = vector.Y;
        float newx = (x * 16) + (y * -16);
        float newy = (x * 8) + (y * 8);

        return new Vector2f(newx, newy);
    }
    public static Vector2f GetMouseGrid(Vector2f inputVector, View view, float zoom)
    {
        float x = inputVector.X / zoom;
        float y = inputVector.Y / zoom;
        //float x = inputVector.X;
        //float y = inputVector.Y;
        float newx = ((x / 32) + (y / 16));
        float newy = (y / 16 - x / 32);
        newx -= view.Center.X / 32;
        newy -= view.Center.Y / 32;
        float gridx = (float)Math.Floor(newx + 0.5f);
        float gridy = (float)Math.Floor(newy + 0.5f);
        return new Vector2f(gridx, gridy);
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