using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace lab_2
{
    public class UserInterface
    {
        public UserInterface(DataManager.DataManager dataManager)
        {
            this.dataManager1 = dataManager;
            IsDone = false;
        }

        DataManager.DataManager dataManager1;
        public bool IsDone { get; set; }


        public void MainMenu()
        {
            ConsoleKeyInfo mainKey = new ConsoleKeyInfo();

            while (mainKey.Key != ConsoleKey.D3)
            {
                Console.Clear();
                Console.WriteLine("1. Get data by Store");
                Console.WriteLine("2. About");
                Console.WriteLine("3. Exit");
                Console.WriteLine("\n");
                mainKey = Console.ReadKey();
                if (mainKey.Key == ConsoleKey.D1)
                {
                    DataMenu();
                }
                if (mainKey.Key == ConsoleKey.D2)
                {
                    AboutMessage();
                }
            }
        }
        async void DataMenu()
        {
            string storename;
            Console.Clear();
            Console.WriteLine("Store name: ");
            storename = Console.ReadLine();
            await GetDataAsync(storename);
        }
        async Task GetDataAsync(string storename)
        {
            await dataManager1.BeginExtractAsync(storename);
            await dataManager1.GenerateXmlAsync();
            await dataManager1.TransferFileAsync("D:\\SourceDir");
        }
        void AboutMessage()
        {
            Console.Clear();
            Console.WriteLine("This method was made to show working async methods");
            Console.ReadKey();
        }
    }
}