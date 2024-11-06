using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

namespace GoDogKit.SaveSystem
{

    #region ISaveable

    /// <summary>
    /// Fundemental interface for all saveable objects.
    /// Contains basical information for saving and loading, such as file name, directory, 
    /// and the save subsystem which own this.
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// The file name without extension on save.
        /// </summary>        
        public string FileName { get; set; }
        /// <summary>
        /// The file name extension on save.
        /// </summary>        
        public string FileNameExtension { get; set; }
        /// <summary>
        /// The directory where the file is saved.
        /// </summary>
        public DirectoryInfo Directory { get; set; }
        /// <summary>
        /// The save subsystem which own this.
        /// </summary>
        public SaveSubsystem SaveSubsystem { get; set; }

    }

    public class JsonSaveable : ISaveable
    {
        [JsonIgnore] public string FileName { get; set; }
        [JsonIgnore] public string FileNameExtension { get; set; }
        [JsonIgnore] public DirectoryInfo Directory { get; set; }
        [JsonIgnore] public SaveSubsystem SaveSubsystem { get; set; }

        // /// <summary>
        // /// The JsonSerializerContext used to serialize and deserialize this object.
        // /// </summary>
        // [JsonIgnore] public JsonSerializerContext SerializerContext { get; set; }
    }

    public class BinarySaveable : ISaveable
    {
        public string FileName { get; set; }
        public string FileNameExtension { get; set; }
        public DirectoryInfo Directory { get; set; }
        public SaveSubsystem SaveSubsystem { get; set; }
    }

    #endregion

    #region System

    public static class SaveSystem
    {
        public static DirectoryInfo DefaultSaveDirectory { get; set; }
        public static string DefaultSaveFileName { get; set; } = "sg";
        public static string DefaultSaveFileExtension { get; set; } = ".data";
        public static EncryptionMethod DefaultEncryptionMethod { get; set; } = EncryptionMethod.None;
        static SaveSystem()
        {
            if (OS.HasFeature("editor"))
            {
                // If current save action happens in editor, 
                // append with "_Editor" in project folder root.
                DefaultSaveDirectory = new DirectoryInfo("Save_Editor");
            }
            else
            {
                // Else, use the "Save" folder to store the save file,
                // at the same path with the game executable in default.
                DefaultSaveDirectory = new DirectoryInfo("Save");
            }

            if (!DefaultSaveDirectory.Exists)
            {
                DefaultSaveDirectory.Create();
            }
        }

        public static string Encrypt(string data, EncryptionMethod method)
        {
            return method switch
            {
                EncryptionMethod.None => data,
                EncryptionMethod.Negation => SaveEncryption.EncryInNegation(data),
                _ => null,
            };
        }

        public static string Decrypt(string data, EncryptionMethod method)
        {
            return method switch
            {
                EncryptionMethod.None => data,
                EncryptionMethod.Negation => SaveEncryption.DecryInNegation(data),
                _ => null,
            };
        }

        public static bool Exists(ISaveable saveable)
        {
            return File.Exists(GetFullPath(saveable));
        }

        public static string GetFullPath(ISaveable saveable)
        {
            return Path.Combine(saveable.Directory.FullName, saveable.FileName + saveable.FileNameExtension);
        }

        public static void Delete(ISaveable saveable)
        {
            if (Exists(saveable))
            {
                File.Delete(GetFullPath(saveable));
            }
        }
    }

    /// <summary>
    /// Base abstract class for all save subsystems.
    /// </summary>
    public abstract class SaveSubsystem
    {
        public DirectoryInfo SaveDirectory { get; set; } = SaveSystem.DefaultSaveDirectory;
        public string SaveFileName { get; set; } = SaveSystem.DefaultSaveFileName;
        public string SaveFileExtension { get; set; } = SaveSystem.DefaultSaveFileExtension;
        public EncryptionMethod EncryptionMethod { get; set; } = SaveSystem.DefaultEncryptionMethod;

        public abstract string Serialize(ISaveable saveable);

        public abstract ISaveable Deserialize(string data, ISaveable saveable);

        public virtual void Save(ISaveable saveable)
        {
            string data = Serialize(saveable);

            string encryptedData = SaveSystem.Encrypt(data, EncryptionMethod);

            File.WriteAllText(SaveSystem.GetFullPath(saveable), encryptedData);
        }

        public virtual ISaveable Load(ISaveable saveable)
        {
            if (!SaveSystem.Exists(saveable)) throw new FileNotFoundException("Save file not found!");

            string data = File.ReadAllText(SaveSystem.GetFullPath(saveable));

            string decryptedData = SaveSystem.Decrypt(data, EncryptionMethod);

            return Deserialize(decryptedData, saveable);
        }
    }

    /// <summary>
    /// Abstract class for all functional save subsystems.
    /// Restricts the type of ISaveable to a specific type, 
    /// providing a factory method for creating ISaveables.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SaveSubsystem<T> : SaveSubsystem where T : ISaveable, new()
    {
        public virtual S Create<S>() where S : T, new()
        {
            var ISaveable = new S()
            {
                FileName = SaveFileName,
                FileNameExtension = SaveFileExtension,
                Directory = SaveDirectory,
                SaveSubsystem = this
            };

            return ISaveable;
        }
    }

    /// <summary>
    /// 
    /// A Sub save system that uses the JsonSerializer in dotnet core.
    /// Notice that a JsonSerializerContext is required to be passed in the constructor,
    /// for AOT completeness.
    /// <para> So you need to code like this as an example: </para>
    /// <sample>
    /// 
    /// <para> [JsonSerializable(typeof(SaveData))] </para>
    /// 
    /// <para> public partial class DataContext : JsonSerializerContext { } </para>
    /// 
    /// <para> public class SaveData : JsonISaveable </para>
    /// <para> { </para>
    /// <para> public int Health { get; set; } </para>
    /// <para> } </para>
    /// 
    /// </sample>
    /// </summary>
    public class JsonSaveSubsystem(JsonSerializerContext serializerContext) : SaveSubsystem<JsonSaveable>
    {
        public readonly JsonSerializerContext SerializerContext = serializerContext;

        public override string Serialize(ISaveable saveable) =>

             JsonSerializer.Serialize(saveable, saveable.GetType(), SerializerContext);

        public override ISaveable Deserialize(string data, ISaveable saveable) =>

          JsonSerializer.Deserialize(data, saveable.GetType(), SerializerContext) as ISaveable;
    }    

    #endregion

    #region Extension Methods

    /// <summary>
    /// All functions used to extend the SaveSystem class. Fully optional, but recommended to use.
    /// </summary>
    public static class SaveSystemExtensions
    {
        public static void Save(this ISaveable saveable)
        {
            saveable.SaveSubsystem.Save(saveable);
        }

        /// <summary>
        /// Unfortuantely, Extension Methods do not support ref classes, so we need to recevive the return value.
        /// </summary>        
        public static T Load<T>(this T saveable) where T : class, ISaveable
        {
            return saveable.SaveSubsystem.Load(saveable) as T;
        }

        public static bool Exists(this ISaveable saveable)
        {
            return SaveSystem.Exists(saveable);
        }

        public static string GetFullPath(this ISaveable saveable)
        {
            return SaveSystem.GetFullPath(saveable);
        }

        public static void Delete(this ISaveable saveable)
        {
            SaveSystem.Delete(saveable);
        }
    }

    #endregion

    #region Encryption

    public static class SaveEncryption
    {
        #region 取反

        public static string EncryInNegation(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)~bytes[i];
            }

            return Encoding.Unicode.GetString(bytes);
        }

        public static string DecryInNegation(string data) => EncryInNegation(data);

        #endregion
    }

    #endregion
}