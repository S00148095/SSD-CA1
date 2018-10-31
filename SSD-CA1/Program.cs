using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SSD_CA1
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "";
            try
            {
                do
                {
                    Console.Clear();
                    input = CheckInput(input);
                } while (input != "5");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.ReadKey();
            }
        }
        private static string CheckInput(string input)
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
                        AddData();
                        break;
                    case "2":
                        GetData(2);
                        break;
                    case "3":
                        GetData(3);
                        break;
                    case "4":
                        GetData(4);
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
        private static void AddData()
        {
            Console.Clear();
            Console.WriteLine("Enter the data");
            string input = Console.ReadLine();
            StreamWriter sw = new StreamWriter("Data.txt", true);
            sw.WriteLine(input);
            sw.Close();
        }
        private static void GetData(int option)
        {
            int i = 1;
            Console.Clear();
            List<string> arr = new List<string>();
            Console.WriteLine("Data:");
            StreamReader sr = new StreamReader("Data.txt");
            string input = sr.ReadLine();
            while (input != null && input != "")
            {
                if (option == 3 || option == 4)
                {
                    arr.Add(input);
                }
                Console.WriteLine("{0}. {1}",i,input);
                i++;
                input = sr.ReadLine();
            }
            sr.Close();
            if (option == 3)
            {
                UpdateData(arr);
            }
            else if (option == 4)
            {
                DeleteData(arr);
            }
            else
            {
                Console.WriteLine("Press enter to continue");
                Console.ReadKey();
            }
        }
        private static void DeleteData(List<string> arr)
        {
            string input="";
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
                        OverwriteFile(arr);
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
        private static void UpdateData(List<string> arr)
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
                        arr[intVal - 1]=newInput;
                        OverwriteFile(arr);
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
        private static void OverwriteFile(List<string> arr)
        {
            File.WriteAllText("Data.txt", "");
            StreamWriter sw = new StreamWriter("Data.txt", true);
            foreach (var item in arr)
            {
                sw.WriteLine(item);
            }
            sw.Close();
        }
    }
}