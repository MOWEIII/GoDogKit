using System;
using System.Text;

namespace GoDogKit
{
    public abstract class Encryption
    {
        public static Encryption None { get; } = new NoneEncryption();

        public abstract string Encrypt(string data);

        public abstract string Decrypt(string data);
    }

    /// <summary>
    /// Represents a no-op encryption method.
    /// </summary>
    public class NoneEncryption : Encryption
    {
        public override string Encrypt(string data) => data;

        public override string Decrypt(string data) => data;
    }

    /// <summary>
    /// Encryption method in negation.
    /// </summary>
    public class NegationEncryption : Encryption
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

    /// <summary>
    /// Base on XOR encryption method. Remember to set the Seed property and 
    /// <para>KEEP THE SEED !!!</para>
    /// <para>KEEP THE SEED !!!</para>
    /// <para>KEEP THE SEED !!!</para>
    /// </summary>
    public class XOREncryption : Encryption
    {
        public byte Seed { get; set; } = (byte)DateTime.Now.Second;

        public override string Encrypt(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ Seed);
            }

            return Encoding.Unicode.GetString(bytes);
        }

        public override string Decrypt(string data) => Encrypt(data);
    }
}

