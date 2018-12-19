using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Security.AccessControl;

namespace SSD_CA1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (AesManaged myAes = new AesManaged())
            {
                myAes.Padding = PaddingMode.PKCS7;
                myAes.KeySize = 128;          // in bits
                myAes.Key = new byte[128 / 8];  // 16 bytes for 128 bit encryption
                myAes.IV = new byte[128 / 8];   // AES needs a 16-byte IV
                string input = "";
                try
                {
                    do
                    {
                        Console.Clear();
                        input = CheckInput(myAes, input);
                    } while (input != "5");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    Console.ReadKey();
                }
                GC.Collect();
            }
        }
        private static string CheckInput(AesManaged myAes, string input)
        {
            input = "";
            while (input != "5")
            {
                Console.Clear();
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Create");
                Console.WriteLine("2. Read");
                Console.WriteLine("3. Update");
                Console.WriteLine("4. Delete");
                Console.WriteLine("5. Exit");
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        AddData(myAes);
                        break;
                    case "2":
                        GetData(myAes, 2);
                        break;
                    case "3":
                        GetData(myAes, 3);
                        break;
                    case "4":
                        GetData(myAes, 4);
                        break;
                    case "5":
                        break;
                    default:
                        input = "";
                        break;
                }
            }
            return input;
        }
        private static void AddData(AesManaged myAes)
        {
            Console.Clear();
            Console.WriteLine("Enter the data");
            Text input = new Text(Console.ReadLine());
            // Encrypt the string to an array of bytes.
            byte[] encrypted = EncryptStringToBytes_Aes(input.value, myAes.Key, myAes.IV);
            if (File.Exists("Data.txt"))
            {
                StreamWriter sw = new StreamWriter("Data.txt", true);
                sw.WriteLine(Convert.ToBase64String(encrypted));
                sw.Close();
            }
            else {
                string fileName = "Data.txt";
                var stream=File.Create(fileName);
                stream.Close();
                // Add the access control entry to the file.
                AddFileSecurity(fileName, Environment.UserDomainName+"\\"+Environment.UserName, FileSystemRights.ReadData, AccessControlType.Allow);
                StreamWriter sw = new StreamWriter("Data.txt", true);
                sw.WriteLine(Convert.ToBase64String(encrypted));
                sw.Close();
            }
        }
        private static void GetData(AesManaged myAes, int option)
        {
            if (File.Exists("Data.txt"))
            {
                int i = 1;
                Console.Clear();
                List<Text> arr = new List<Text>();
                Console.WriteLine("Data:");
                StreamReader sr = new StreamReader("Data.txt");
                string input = sr.ReadLine();
                while (input != null && input != "")
                {
                    // Decrypt the bytes to a string.
                    Text decrypted = new Text(DecryptStringFromBytes_Aes(Convert.FromBase64String(input), myAes.Key, myAes.IV));
                    Console.WriteLine("{0}. {1}", i, decrypted.value);
                    if (option == 3 || option == 4)
                    {
                        arr.Add(decrypted);
                    }
                    i++;
                    input = sr.ReadLine();
                }
                sr.Close();
                if (option == 3)
                {
                    UpdateData(myAes, arr);
                }
                else if (option == 4)
                {
                    DeleteData(myAes, arr);
                }
                else
                {
                    Console.WriteLine("Press enter to continue");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Try creating data first, press enter to return to the menu");
                Console.ReadKey();
            }
        }
        private static void DeleteData(AesManaged myAes, List<Text> arr)
        {
            string input = "";
            int intVal;
            while (input == "")
            {
                Console.WriteLine("Enter the number of the record you would like to delete");
                input = Console.ReadLine();
                if (Int32.TryParse(input, out intVal))
                {
                    if (intVal <= arr.Count() && intVal > 0)
                    {
                        arr.RemoveAt(intVal - 1);
                        OverwriteFile(myAes, arr);
                    }
                    else
                    {
                        Console.WriteLine("Record does not exist");
                        input = "";
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    input = "";
                }
            }
        }
        private static void UpdateData(AesManaged myAes, List<Text> arr)
        {
            string input = "";
            int intVal;
            while (input == "")
            {
                Console.WriteLine("Enter the number of the record you would like to update");
                input = Console.ReadLine();
                if (Int32.TryParse(input, out intVal))
                {
                    if (intVal <= arr.Count() && intVal > 0)
                    {
                        Console.WriteLine("What would you like to replace it with");
                        string newInput = Console.ReadLine();
                        arr[intVal - 1] = new Text(newInput);
                        OverwriteFile(myAes, arr);
                    }
                    else
                    {
                        Console.WriteLine("Record does not exist");
                        input = "";
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    input = "";
                }
            }
        }
        private static void OverwriteFile(AesManaged myAes, List<Text> arr)
        {
            File.WriteAllText("Data.txt", "");
            if (arr.Count > 0)
            {
                StreamWriter sw = new StreamWriter("Data.txt", true);
                foreach (var item in arr)
                {
                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptStringToBytes_Aes(item.value, myAes.Key, myAes.IV);
                    sw.WriteLine(Convert.ToBase64String(encrypted));
                }
                sw.Close();
            }
            else
            {
                File.Delete("Data.txt");
            }
        }
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.KeySize = 128;          // in bits
                aesAlg.Key = new byte[128 / 8];  // 16 bytes for 128 bit encryption
                aesAlg.IV = new byte[128 / 8];   // AES needs a 16-byte IV
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            GC.Collect();
            return encrypted;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.KeySize = 128;          // in bits
                aesAlg.Key = new byte[128 / 8];  // 16 bytes for 128 bit encryption
                aesAlg.IV = new byte[128 / 8];   // AES needs a 16-byte IV
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            GC.Collect();
            return plaintext;
        }

        public static void AddFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Add the FileSystemAccessRule to the security settings.
            fSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, controlType));

            // Set the new access settings.
            File.SetAccessControl(fileName, fSecurity);
            }
    }
}