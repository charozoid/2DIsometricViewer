using SFML.Graphics;
using SFML.System;

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
