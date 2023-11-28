using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Text.Json;
using Newtonsoft.Json;

class Program
{
    public static int chosenElevation = 0;
    public static Random random = new Random();
    public static Dictionary<Block.Type, IntRect> spritePos = new Dictionary<Block.Type, IntRect>();
    const string mapPath = @"../../Assets/maps.json";
    public static bool menuOpened = false;
    public static void Main(string[] args)
    {
        Clock keyPressedClock = new Clock();
        spritePos[Block.Type.Block] = new IntRect(0, 0, 32, 32);
        spritePos[Block.Type.Water] = new IntRect(0, 32, 32, 32);
        Graphics.window.SetVerticalSyncEnabled(true);
        Graphics.window.Closed += (sender, args) => Graphics.window.Close();

        while (Graphics.window.IsOpen)
        {


            Graphics.window.SetView(Graphics.view);
            Graphics.window.Clear();
            Graphics.DrawGrid();
            Graphics.DrawBlocks();
            Graphics.DrawBlockOnCursor();

            if (keyPressedClock.ElapsedTime.AsSeconds() >= 0.2f)
            {
                KeyPressedDelay();
                keyPressedClock.Restart();

            }
            KeyPressed();
            Ui.WriteElevation();
            Ui.WriteGridCoords();
            Ui.WriteRealCoords();
            Graphics.window.DispatchEvents();
            Graphics.window.Display();
        }
    }
    public static void KeyPressed()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
        {
            Graphics.view.Center += new Vector2f(0, -Graphics.CameraSpeed);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
        {
            Graphics.view.Center += new Vector2f(0, Graphics.CameraSpeed);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
        {
            Graphics.view.Center += new Vector2f(-Graphics.CameraSpeed, 0);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
        {
            Graphics.view.Center += new Vector2f(Graphics.CameraSpeed, 0);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.PageDown))
        {
            Graphics.view.Zoom(Graphics.ZoomFactor);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.PageUp))
        {
            Graphics.view.Zoom(1.0f / Graphics.ZoomFactor);
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Enter))
        {
            SaveMap();
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.L))
        {
            LoadMap();
        }
        if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
        {
            Ui.DrawBlockSelectionMenu();
            menuOpened = true;
        }
        else
        {
            menuOpened = false;
        }
        if (Mouse.IsButtonPressed(Mouse.Button.Left))
        {
            if (menuOpened)
                return;
            Vector2i mousePos = Mouse.GetPosition(Graphics.window);
            Vector2f worldCoords = Graphics.window.MapPixelToCoords(mousePos, Graphics.view);
            Graphics.CreateBlock(LinearTransformations.GetMouseGrid(worldCoords, Graphics.view), Block.Type.Block);
        }
        if (Mouse.IsButtonPressed(Mouse.Button.Right))
        {
            Vector2i mousePos = Mouse.GetPosition(Graphics.window);
            Vector2f worldCoords = Graphics.window.MapPixelToCoords(mousePos, Graphics.view);

            Graphics.RemoveBlock(LinearTransformations.GetMouseGrid(worldCoords, Graphics.view));
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

    public static void SaveMap()
    {
        string jsonData = JsonConvert.SerializeObject(Graphics.blocks);
        var mapFile = File.Create(mapPath);
        mapFile.Close();
        File.WriteAllText(mapPath, jsonData);
    }
    public static void LoadMap()
    {
        string jsonData = File.ReadAllText(mapPath);
        Graphics.blocks = JsonConvert.DeserializeObject<Block[,,]>(jsonData);
    }
}
class Graphics
{
    public const int WIDTH = 960;
    public const int HEIGHT = 600;
    public static VideoMode mode = new VideoMode(WIDTH, HEIGHT);
    public static RenderWindow window = new RenderWindow(mode, "Isometric");

    public static View view = new View(new FloatRect(0, 0, WIDTH, HEIGHT));
    public static View ui = new View(new FloatRect(0, 0, WIDTH, HEIGHT));

    public static Font font = new Font("../../Assets/Fonts/arialbd.ttf");
    public static Texture cubeText = new Texture("../../Assets/spritesheet.png");

    public static Block[,,] blocks = new Block[32, 32, 8];

    public static float CameraSpeed = 2.0f;
    public static float ZoomFactor = 1.01f;
    public static void DrawGrid()
    {
        Sprite sprite = new Sprite(cubeText);
        sprite.TextureRect = new IntRect(128, 32, 32, 32);
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                sprite.Position = LinearTransformations.TransformVectorToIsometric(new Vector2f(i, j));
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
                        DrawBlockOnCursor();

                    }
                }
            }
        }

    }
    public static void DrawBlockOnCursor()
    {
        if (Program.menuOpened)
        {
            return;
        }

        Vector2f gridCoords = MousePositionToGrid();
        Vector2f menuPos = Ui.selectionMenu.selectedPos;
        IntRect intRect = new IntRect((int)menuPos.X / 2, (int)menuPos.Y / 2, 32, 32);
        Block block = new Block((int)gridCoords.X, (int)gridCoords.Y, Program.chosenElevation, intRect);
        window.Draw(block.sprite);
    }
    public static void CreateBlock(Vector2f position, Block.Type type)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        if (x < 0 || y < 0 || x > 31 || y > 31)
        {
            return;
        }
        Vector2f menuPos = Ui.selectionMenu.selectedPos;
        IntRect intRect = new IntRect((int)menuPos.X / 2, (int)menuPos.Y / 2, 32, 32);
        blocks[x, y, Program.chosenElevation] = new Block(x, y, Program.chosenElevation, intRect);
    }
    public static void RemoveBlock(Vector2f position)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        if (x < 0 || y < 0 || x > 31 || y > 31)
        {
            return;
        }
        blocks[x, y, Program.chosenElevation] = null;
    }
    public static Vector2f MousePositionToGrid()
    {
        Vector2i mousePos = Mouse.GetPosition(window);
        Vector2f worldCoords = window.MapPixelToCoords(mousePos, view);
        return LinearTransformations.GetMouseGrid(worldCoords, view);
    }
    public static IntRect GridToIntRect(Vector2f grid)
    {
        return new IntRect((int)grid.X * 32, (int)grid.Y * 32, 32, 32);
    }
}
class LinearTransformations
{
    public static Vector2f TransformVectorToIsometric(Vector2f vector)
    {
        float x = vector.X;
        float y = vector.Y;
        float newx = ((x * 16) + (y * -16)) + Graphics.WIDTH / 2 - 16;
        float newy = (x * 8) + (y * 8);

        return new Vector2f(newx, newy);
    }
    public static Vector2f GetMouseGrid(Vector2f inputVector, View view)
    {
        float x = inputVector.X - Graphics.WIDTH / 2;
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
    public int realx = 0;
    public int realy = 0;
    public float drawx = 0;
    public float drawy = 0;
    [JsonIgnore]
    public Sprite sprite;
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
    public void CreateSprite()
    {
        sprite = new Sprite(Graphics.cubeText);
    }
    public Block(int x, int y, int z, IntRect intRect)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        realx = x - z;
        realy = y - z;
        sprite = new Sprite(Graphics.cubeText);
        this.intRect = intRect;
        sprite.TextureRect = intRect;
        sprite.Position = LinearTransformations.TransformVectorToIsometric(new Vector2f(x, y));
    }
}