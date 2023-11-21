using SFML.Graphics;
using SFML.System;
using SFML.Window;
class Program
{
    public static int chosenElevation = 0;
    public static Random random = new Random();
    public static Dictionary<Block.Type, IntRect> spritePos = new Dictionary<Block.Type, IntRect>();
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


    public static void  KeyPressed()
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
                        CalculateShadow(block);
                        window.Draw(blocks[i, j, k].sprite);
                        DrawBlockOnCursor();

                    }
                }
            }
        }

    }
    public static void CalculateShadow(Block block)
    {

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
        CalculateBlockShadow();
    }
    public static void CalculateBlockShadow()
    {
        for (int i = Program.chosenElevation; i > 0; i--)
        {

        }
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

class UIElement
{
    public Vector2f position { get; set; }
    public Vector2f size { get; set; }
    public Vector2f selectedPos = new Vector2f(0, 0);
    public RectangleShape shape = new RectangleShape(new Vector2f(64, 64));
    public UIElement(Vector2f position, Vector2f size)
    {
        this.position = position;
        this.size = size;
    }
    public virtual void Draw()
    {
    }
    public virtual Vector2f GetMousePosToGrid()
    {
        return new Vector2f(0, 0);
    }
    public virtual void KeyPressed()
    {

    }
}
class SelectedElement : UIElement
{
    public RectangleShape shape = new RectangleShape(new Vector2f(64, 64));
    public SelectedElement(Vector2f position, Vector2f size) : base(position, size)
    {
        shape.FillColor = Color.White;
    }
}
class SelectionMenu : UIElement
{
    private List<UIElement> children = new List<UIElement>();
    private SelectedElement mouseHover;
    private SelectedElement selectedBlock;
    public Vector2f selectedPos = new Vector2f(0, 0);
    public SelectionMenu(Vector2f position, Vector2f size) : base(position, size)
    {
        Graphics.window.SetView(Graphics.ui);
        mouseHover = new SelectedElement(position, new Vector2f(64, 64));
        selectedBlock = new SelectedElement(position, new Vector2f(64, 64));
        children.Add(mouseHover);
    }

    public override Vector2f GetMousePosToGrid()
    {
        Vector2i mousePos = Mouse.GetPosition(Graphics.window);
        int offsetx = (mousePos.X - ((Graphics.WIDTH / 2) - 256));
        int offsety = (mousePos.Y - ((Graphics.HEIGHT / 2)) + 256);
        Vector2i pos = new Vector2i(offsetx / 64, offsety / 64);
        return new Vector2f(pos.X, pos.Y);
    }
    public void AddChildren(UIElement child)
    {
        children.Add(child);
    }
    public override void KeyPressed()
    {
        if (Mouse.IsButtonPressed(Mouse.Button.Left))
        {
            Vector2f gridPos = GetMousePosToGrid();
            selectedPos = new Vector2f(gridPos.X * 64, gridPos.Y * 64);
        }
    }
    public override void Draw()
    {
        Graphics.window.SetView(Graphics.ui);
        Vector2f view = Graphics.ui.Center;
        Sprite sprite = new Sprite(Graphics.cubeText);
        sprite.Color = new Color(255, 255, 255, 255);
        sprite.Scale = new Vector2f(2.0f, 2.0f);
        sprite.Position = new Vector2f(view.X - 256, view.Y - 256);

        RectangleShape shape = new RectangleShape(size);
        shape.FillColor = new Color(32, 32, 32, 200);
        shape.Position = position;

        Vector2f mousePos = GetMousePosToGrid();
        mouseHover.position = position + new Vector2f(mousePos.X * 64, mousePos.Y * 64);
        mouseHover.shape.Position = position + new Vector2f(mousePos.X * 64, mousePos.Y * 64);
        mouseHover.Draw();

        KeyPressed();
        selectedBlock.position = position + selectedPos;
        selectedBlock.shape.Position = position + selectedPos;
        selectedBlock.Draw();
        


        Graphics.window.Draw(selectedBlock.shape);
        Graphics.window.Draw(mouseHover.shape);
        Graphics.window.Draw(shape);
        Graphics.window.Draw(sprite);

    }

}


class Ui
{
    public static View ui = Graphics.ui;
    public static Vector2f view = ui.Center;
    public static SelectionMenu selectionMenu = new SelectionMenu(new Vector2f(view.X - 256, view.Y - 256), new Vector2f(512, 512));
    public Ui()
    {
        View ui = Graphics.ui;
        Graphics.window.SetView(ui);
        Vector2f view = ui.Center;
        SelectedElement selectedElement = new SelectedElement(new Vector2f(0, 0), new Vector2f(64, 64));
        selectionMenu.AddChildren(selectedElement);
    }
    public Vector2f chosenGrid = new Vector2f(0, 0);
    public static void WriteElevation()
    {
        Graphics.window.SetView(Graphics.ui);
        Text elevationText = new Text($"Elevation: {Program.chosenElevation}", Graphics.font);
        elevationText.Position = new Vector2f(0, 20);
        Graphics.window.Draw(elevationText);
    }
    public static void WriteGridCoords()
    {
        Graphics.window.SetView(Graphics.ui);
        Vector2f gridCoords = Graphics.MousePositionToGrid();
        Text text = new Text($"X: {gridCoords.X} Y: {gridCoords.Y}", Graphics.font);
        text.Position = new Vector2f(0, 0);
        Graphics.window.Draw(text);
    }
    public static void DrawBlockSelectionMenu()
    {
        selectionMenu.Draw();
    }
    public static void WriteRealCoords()
    {
        Graphics.window.SetView(Graphics.ui);
        Vector2f gridCoords = Graphics.MousePositionToGrid();
        Text text = new Text($"X: {gridCoords.X + Program.chosenElevation} Y: {gridCoords.Y + Program.chosenElevation}, Z: {Program.chosenElevation}", Graphics.font);
        text.Position = new Vector2f(0, 50);
        Graphics.window.Draw(text);
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
    public Sprite sprite = new Sprite(Graphics.cubeText);
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
    public Block(int x, int y, int z, IntRect intRect)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        realx = x - z;
        realy = y - z;
        sprite = new Sprite(Graphics.cubeText);
        sprite.TextureRect = intRect;
        sprite.Position = LinearTransformations.TransformVectorToIsometric(new Vector2f(x, y));
    }
}