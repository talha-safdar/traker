namespace Traker.Events
{
    /// <summary>
    /// Used to signal the closing of a window, with the name of the window to be closed as a parameter.
    /// </summary>
    public class CloseWindow
    {
        public string WindowName { get; set; } = string.Empty;
    }
}