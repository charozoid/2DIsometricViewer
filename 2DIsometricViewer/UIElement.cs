using SFML.Graphics;
using SFML.System;
using SFML.Window;

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
