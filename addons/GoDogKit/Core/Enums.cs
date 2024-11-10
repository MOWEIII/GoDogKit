namespace GoDogKit
{
    public enum CoroutineProcessMode
    {
        /// <summary>
        /// Response to Process.
        /// </summary>
        Idle,
        /// <summary>
        /// Response to PhysicsProcess.
        /// </summary>
        Physics,
    }

    public enum InputMode
    {
        KeyboardAndMouse,
        Gamepad,
        Screen
    }   

}