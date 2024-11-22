using Godot;

namespace GoDogKit
{
    public static class Archive
    {
        /// <summary>
        /// The path to the user's My Documents folder.
        /// </summary>
        public readonly static string MyDocuments;

        /// <summary>
        /// The path to the Streaming Assets folder.
        /// </summary>
        public readonly static string Streaming;

        static Archive()
        {
            MyDocuments
            = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            if (OS.HasFeature("editor"))
            {
                Streaming = "Streaming";
            }
            else
            {
                Streaming = OS.GetExecutablePath().GetBaseDir();
            }
        }
    }

}
