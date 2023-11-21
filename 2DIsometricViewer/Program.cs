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
    public static Block[,,] blocks = new Block[32, 32, 8];
    public static View view = new View(new FloatRect(0, 0, WIDTH, HEIGHT));
    public static View ui = new View(new FloatRect(0, 0, WIDTH, HEIGHT));
    public static Font font = new Font("../../Assets/Fonts/arialbd.ttf");
    public static float CameraSpeed = 2.0f;
    public static float ZoomFactor = 1.01f;
    public static float amplitude = 0.25f;
    public static float frequency = 2f;
    public static int chosenElevation = 0;
    public static Random random = new Random();
    public static Clock clock = new Clock();
    public static List<Sprite> sprites = new List<Sprite>();
    public static Dictionary<Block.Type, IntRect> spritePos = new Dictionary<Block.Type, IntRect>();
    public static void Main(string[] args)
    {
        Clock keyPressedClock = new Clock();
        spritePos[Block.Type.Block] = new IntRect(0, 0, 32, 32);
        spritePos[Block.Type.Water] = new IntRect(0, 32, 32, 32);
        window.SetVerticalSyncEnabled(true);
        window.Closed += (sender, args) => window.Close();



        while (window.IsOpen)
        {


            window.SetView(view);
            window.Clear();
            DrawBlockOnCursor();
            DrawGrid();
            DrawBlocks();
            foreach (Sprite sprite in sprites)
            {
                window.Draw(sprite);
            }
            window.SetView(ui);
            Vector2i mousePos = Mouse.GetPosition(window);
            Vector2f worldCoords = window.MapPixelToCoords(mousePos, view);
            Vector2f gridCoords = GetMouseGrid(worldCoords, view);
            Text text = new Text($"X: {gridCoords.X} Y: {gridCoords.Y}", font);
            text.Position = new Vector2f(0, 0);
            window.Draw(text);
            Text elevationText = new Text($"Elevation: {chosenElevation}", font);
            elevationText.Position = new Vector2f(0, 20);
            window.Draw(elevationText);
            if (keyPressedClock.ElapsedTime.AsSeconds() >= 0.2f)
            {
                KeyPressedDelay();
                keyPressedClock.Restart();

            }
            KeyPressed();
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
                sprite.Position = TransformVectorToIsometric(new Vector2f(i, j));
                window.Draw(sprite);
            }
        }
    }
    public static void DrawBlocks()
    {
        for (int k = 0; k < 8; k++)
        {
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    Block block = blocks[i, j, k];
                    if (blocks[i, j, k] != null && block.z == k)
                    {
                        window.Draw(blocks[i, j, k].sprite);
                    }
                }
            }
        }

    }
    public static void DrawBlockOnCursor()
    {
        Sprite sprite = new Sprite(cubeText);
        sprite.TextureRect = new IntRect(0, 0, 32, 32);
        Vector2i mousePos = Mouse.GetPosition(window);
        Vector2f worldCoords = window.MapPixelToCoords(mousePos, view);
        Vector2f gridCoords = GetMouseGrid(worldCoords, view);
        sprite.Position = TransformVectorToIsometric(gridCoords);
        window.Draw(sprite);
    }
    public static void CreateBlock(Vector2f position, Block.Type type)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        if (x < 0 || y < 0 || x > 31 || y > 31)
        {
            return;
        }
        blocks[x, y, chosenElevation] = new Block(x, y, chosenElevation);
        Block block = blocks[x, y, chosenElevation];
        block.sprite = new Sprite(cubeText);
        block.sprite.TextureRect = spritePos[type];
        block.sprite.Position = TransformVectorToIsometric(position);
    }
    public static void RemoveBlock(Vector2f position)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        if (x < 0 || y < 0 || x > 31 || y > 31)
        {
            return;
        }
        blocks[x, y, chosenElevation] = null;
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
            CreateBlock(GetMouseGrid(worldCoords, view), Block.Type.Block);
        }
        if (Mouse.IsButtonPressed(Mouse.Button.Right))
        {
            Vector2i mousePos = Mouse.GetPosition(window);
            Vector2f worldCoords = window.MapPixelToCoords(mousePos, view);

            RemoveBlock(GetMouseGrid(worldCoords, view));
        }
    }
    public static void KeyPressedDelay()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.Add))
        {
            if (chosenElevation < 7)
            {
                chosenElevation++;
            }
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Subtract))
        {
            if (chosenElevation > 0)
            {
                chosenElevation--;
            }
        }
    }
    public static Vector2f TransformVectorToIsometric(Vector2f vector)
    {
        float x = vector.X;
        float y = vector.Y;
        float newx = ((x * 16) + (y * -16)) + WIDTH / 2 - 16;
        float newy = (x * 8) + (y * 8);

        return new Vector2f(newx, newy);
    }
    public static Vector2f GetMouseGrid(Vector2f inputVector, View view)
    {
        float x = inputVector.X - WIDTH / 2;
        float y = inputVector.Y;
        float newx = ((x / 32) + (y / 16));
        float newy = (y / 16 - x / 32);
        float gridx = (float)Math.Floor(newx);
        float gridy = (float)Math.Floor(newy);
        return new Vector2f(gridx, gridy);
    }
}

class Block
{
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public float drawx = 0;
    public float drawy = 0;
    public Sprite sprite = new Sprite(Program.cubeText);
    public enum Type
    {
        Block,
        Water
    }

    public Vector2f AddSinOffsetToSprite(float offset)
    {
        float posy = sprite.Position.Y;
        return new Vector2f(sprite.Position.X, sprite.Position.Y + offset);
    }
    public Block(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        sprite.TextureRect = new IntRect(0, 0, 32, 32);
    }
}