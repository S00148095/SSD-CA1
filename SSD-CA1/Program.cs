using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SSD_CA1
{
    class Program
    {
        static void Main(string[] args)
        {
            string input="";
            try
            {
                do
                {
                    Console.Clear();
                    input = GetData(input);
                } while (input != "");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.ReadKey();
            }
        }

        private static string GetData(string input)
        {
            Console.WriteLine("Enrolled Students:");
            StreamReader sr = new StreamReader("Data.txt");
            input = sr.ReadLine();
            while (input != null && input != "")
            {
                Console.WriteLine(input);
                input = sr.ReadLine();
            }
            sr.Close();
            input = CheckInput(input);
            return input;
        }

        private static string CheckInput(string input)
        {
            input = "";
            while (input.ToUpper() != "Y" && input.ToUpper() != "N")
            {
                Console.WriteLine("Would you like to add a student?(Y/N)");
                input = Console.ReadLine();
                if (input.ToUpper() == "Y") {
                    AddStudent();
                }
                else if (input.ToUpper() == "N")
                {
                    input = "";
                    break;
                }
            }
            return input;
        }

        private static void AddStudent()
        {
            Console.WriteLine("Enter the student's name");
            string input = Console.ReadLine();
            StreamWriter sw = new StreamWriter("Data.txt",true);
            sw.WriteLine(input);
            sw.Close();
        }
    }
}
