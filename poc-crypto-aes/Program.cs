using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.IO;

namespace poc_crypto_aes
{
    class Program
    {
        static readonly string basePath = @"D:\mtiexe\testfiles\rege11\20211207_080314";
               
        static readonly string crypto = "E11_25_001_04_01_001_06122021151448398_01_24_05122021_TGS_C.txt";
               
        static readonly string plain = "cod-E11_25_001_04_01_001_06122021151448398_01_24_05122021_TGS_C.txt";

        static readonly string zip = "E11_25_001_04_01_001_06122021151448398_01_24_05122021_TGS_C.zip";

        static void Main(string[] args)
        {
            TestDescifrado();

            TestCifrado();

            Console.ReadKey();
        }

        static void TestDescifrado()
        {
            try
            {
                string plainTest = null;

                AesManaged aes = new();
                aes.Mode = CipherMode.CBC;

                string strKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
                byte[] key = Encoding.UTF8.GetBytes(strKey).Take(32).ToArray();
                byte[] iv = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);

                byte[] cipherText = Convert.FromBase64String(File.ReadAllText(Path.Combine(basePath, crypto)));

                using (MemoryStream ms = new(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new(cs))
                            plainTest = reader.ReadToEnd();
                    }
                }

                string plainOk = File.ReadAllText(Path.Combine(basePath, plain));

                if (plainOk == plainTest)
                    Console.WriteLine("Descifrado OK");
                else
                    Console.WriteLine("Descifrado fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        static void TestCifrado()
        {
            try
            {
                string cryptoTest = null;

                AesManaged aes = new();
                aes.Mode = CipherMode.CBC;

                string strKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
                byte[] key = Encoding.UTF8.GetBytes(strKey).Take(32).ToArray();
                byte[] iv = new byte[16];

                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                byte[] plainBytes = Encoding.UTF8.GetBytes(File.ReadAllText(Path.Combine(basePath, plain)));
                byte[] cryptoBytes;

                using (var ms = new MemoryStream())
                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    cryptoBytes = ms.ToArray();
                }

                cryptoTest = Convert.ToBase64String(cryptoBytes);

                string cryptoOk = File.ReadAllText(Path.Combine(basePath, crypto));

                if (cryptoTest == cryptoOk)
                    Console.WriteLine("Cifrado OK");
                else
                    Console.WriteLine("Cifrado fail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }
    }
}
