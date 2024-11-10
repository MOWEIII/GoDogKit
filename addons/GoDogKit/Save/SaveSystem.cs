using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Godot;

namespace GoDogKit
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

        public virtual void Clone(ISaveable saveable)
        {
            FileName = saveable.FileName;
            FileNameExtension = saveable.FileNameExtension;
            Directory = saveable.Directory;
            SaveSubsystem = saveable.SaveSubsystem;
        }
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
        public static SaveEncryption DefaultEncryption { get; set; } = SaveEncryption.Default;
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

        public static string Encrypt(string data, SaveEncryption encryption)
        {
            return encryption.Encrypt(data);
        }

        public static string Decrypt(string data, SaveEncryption encryption)
        {
            return encryption.Decrypt(data);
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

        /// <summary>
        /// Checks if there are any files in the system's save directory.
        /// It will count the number of files with the same extension as the system's 
        /// by default.
        /// </summary>
        /// <param name="system"> The save subsystem to check. </param>
        /// <param name="saveNumber"> The number of files found. </param>
        /// <param name="extensionCheck"> Whether to check the file extension or not. </param>
        /// <returns></returns>
        public static bool HasFiles(SaveSubsystem system, out int saveNumber, bool extensionCheck = true)
        {
            var fileInfos = system.SaveDirectory.GetFiles();

            saveNumber = 0;

            if (fileInfos.Length == 0)
            {
                return false;
            }

            if (extensionCheck)
            {
                foreach (var fileInfo in fileInfos)
                {
                    if (fileInfo.Extension == system.SaveFileExtension)
                    {
                        saveNumber++;
                    }
                }

                if (saveNumber == 0) return false;
            }
            else
            {
                saveNumber = fileInfos.Length;
            }

            return true;
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
        public SaveEncryption Encryption { get; set; } = SaveSystem.DefaultEncryption;

        public abstract string Serialize(ISaveable saveable);

        public abstract ISaveable Deserialize(string data, ISaveable saveable);

        public virtual void Save(ISaveable saveable)
        {
            string data = Serialize(saveable);

            string encryptedData = SaveSystem.Encrypt(data, Encryption);

            File.WriteAllText(SaveSystem.GetFullPath(saveable), encryptedData);
        }

        public virtual ISaveable Load(ISaveable saveable)
        {
            if (!SaveSystem.Exists(saveable)) throw new FileNotFoundException("Save file not found!");

            string data = File.ReadAllText(SaveSystem.GetFullPath(saveable));

            string decryptedData = SaveSystem.Decrypt(data, Encryption);

            var newSaveable = Deserialize(decryptedData, saveable);

            newSaveable.Clone(saveable);

            return newSaveable;
        }

        public virtual Task SaveAsync(ISaveable saveable)
        {
            string data = Serialize(saveable);

            string encryptedData = SaveSystem.Encrypt(data, Encryption);

            return File.WriteAllTextAsync(SaveSystem.GetFullPath(saveable), encryptedData);
        }

        public virtual Task<ISaveable> LoadAsync(ISaveable saveable)
        {
            if (!SaveSystem.Exists(saveable)) throw new FileNotFoundException("Save file not found!");

            return File.ReadAllTextAsync(SaveSystem.GetFullPath(saveable)).ContinueWith(task =>
            {
                string data = task.Result;

                string decryptedData = SaveSystem.Decrypt(data, Encryption);

                var newSaveable = Deserialize(decryptedData, saveable);

                newSaveable.Clone(saveable);

                return newSaveable;
            });
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
        [Obsolete("Use Subsystem.Save() instead.")]
        public static void Save(this ISaveable saveable)
        {
            saveable.SaveSubsystem.Save(saveable);
        }

        /// <summary>
        /// Unfortuantely, Extension Methods do not support ref classes, so we need to recevive the return value.
        /// </summary>  
        [Obsolete("Use Subsystem.Load() instead.")]
        public static T Load<T>(this T saveable) where T : class, ISaveable
        {
            return saveable.SaveSubsystem.Load(saveable) as T;
        }

        /// <summary>
        /// Save a saveable into local file system depends on its own properties.
        /// </summary>
        public static void Save<T>(this SaveSubsystem subsystem, T saveable) where T : class, ISaveable
        {
            subsystem.Save(saveable);
        }

        /// <summary>
        /// Load a saveable from local file system depends on its own properties.
        /// This an alternative way to load a saveable object, remember to use a ref parameter.
        /// </summary>
        public static void Load<T>(this SaveSubsystem subsystem, ref T saveable) where T : class, ISaveable
        {
            saveable = subsystem.Load(saveable) as T;
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

        public static bool HasFiles(this SaveSubsystem system, out int saveNumber, bool extensionCheck = true)
        {
            return SaveSystem.HasFiles(system, out saveNumber, extensionCheck);
        }
    }

    #endregion

    #region Encryption

    public abstract class SaveEncryption
    {
        public abstract string Encrypt(string data);

        public abstract string Decrypt(string data);

        public static NoneEncryption Default { get; } = new NoneEncryption();
    }

    public class NoneEncryption : SaveEncryption
    {
        public override string Encrypt(string data) => data;

        public override string Decrypt(string data) => data;
    }

    /// <summary>
    /// Encryption method in negation.
    /// </summary>
    public class NegationEncryption : SaveEncryption
    {
        public override string Encrypt(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)~bytes[i];
            }

            return Encoding.Unicode.GetString(bytes);
        }

        public override string Decrypt(string data) => Encrypt(data);
    }

    #endregion
}