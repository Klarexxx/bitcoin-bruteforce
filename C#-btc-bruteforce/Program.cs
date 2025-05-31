using System;
using System.IO;
using System.Threading;
using NBitcoin;

class BruteForce
{

    private static HashSet<string> addressSet = new HashSet<string>();
    private static string filePath = "";
    private static int totalchecked = 0;
    private static int totalAddress = 0;


    static void Main(string[] args)
    {
        Console.Title = "Klarex BTC-BRUTEFORCE"; 
        Console.Write("File Path:");
        filePath = Console.ReadLine();
        LoadAddresses();


        Console.ForegroundColor = ConsoleColor.Red;


        if (totalAddress <= 0)
        {
            Console.WriteLine("Press enter to exit..");
            Console.Read();
            Environment.Exit(0);
        }


        Console.Write("Thread:");
        int threadCount = int.Parse(Console.ReadLine());

        Thread[] threads = new Thread[threadCount];


        for (int i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(checker);
            threads[i].Start();
        }

    }



    static void checker()
    {
        while (true)
        {

            var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
            var masterKey = mnemonic.DeriveExtKey();


            //var segwitKeyPath = new KeyPath("m/84'/0'/0'/0/0");



            var segwitKey = masterKey.Derive(new KeyPath("m/84'/0'/0'/0/0"));
            var segwitAddress = segwitKey.PrivateKey.PubKey.WitHash.GetAddress(Network.Main).ToString();




            totalchecked++;
            if (addressSet.Contains(segwitAddress))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("FOUND:"+segwitAddress);
                Console.WriteLine("Seed Phrase:"+mnemonic.ToString());
                Console.WriteLine("Wallet Import Format(WIF):"+segwitKey.PrivateKey.GetWif(Network.Main).ToString());
                using (StreamWriter writer = new StreamWriter("BTC-found-addresses.txt", true))
                {
                    writer.WriteLine("Address:"+segwitAddress+"  Seed Phrase:"+mnemonic.ToString()+"  Wallet Import Format(WIF):"+segwitKey.PrivateKey.GetWif(Network.Main).ToString());
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Read();
            }
            else if (totalchecked % 10 == 0)
            {
                Console.Title = "Checked:"+totalchecked.ToString(); 
                Console.WriteLine("Trying:"+segwitAddress);
            }
        }
    }





    static void LoadAddresses()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Reading File..");
        if (File.Exists(filePath))
        {
            foreach (var addr in File.ReadLines(filePath))
            {
                if (addr.StartsWith("bc1q") && !addr.Contains(" "))
                {
                    if (totalAddress % 20 == 0) { Console.WriteLine("imported:"+addr); }
                    totalAddress++;
                    addressSet.Add(addr.Trim());
                }
            }
            Console.WriteLine("Total Address:"+totalAddress.ToString());
            Thread.Sleep(4000);

        }
        else
        {
            Console.WriteLine("File Not Found");
            Console.Read();
            Environment.Exit(0);
        }
    }
}