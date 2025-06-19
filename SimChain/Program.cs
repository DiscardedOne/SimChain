using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SimChain
{
    class Program
    {
        public static int MinerCount { get; set; } = 0;
        public static int Complexity { get; set; } = 1;
        public static string PrivateKey { get; set; } = string.Empty;
        public static string PublicKey { get; set; } = string.Empty;

        public static LinkedList<Block> Blockchain = new LinkedList<Block>();

        public static List<Miner> Miners = new List<Miner>();
        public static async Task Main(string[] args)
        {
            int menuOption = 0;

            Console.WriteLine("-- Configuration Stage --");
            Console.WriteLine();
            Console.Write("Enter the number of Miners: ");
            MinerCount = Int32.Parse(Console.ReadLine());
            Console.WriteLine();

            for(int i = 1; i <= MinerCount; i++)
            {
                Miner m = new Miner(i);
                Miners.Add(m);
            }

            Console.Write("Enter the Complexity (number of 0's): ");
            Complexity = Int32.Parse(Console.ReadLine());
            Console.WriteLine();

            Console.Write("Do you have Private and Public Keys? (Y/N): ");
            if(Console.ReadKey().Key.ToString().ToLower() == "y")
            {
                Console.WriteLine();
                Console.WriteLine("Enter your Public key: ");
                PublicKey = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine("Enter your Private key: ");
                PrivateKey = Console.ReadLine();
            }
            else
            {
                Console.WriteLine();
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
                {
                    PublicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                    PrivateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine("Public Key: \t" + PublicKey);
                    Console.WriteLine();
                    Console.WriteLine("Private Key:\t" + PrivateKey);
                    Console.WriteLine("-----------------------------------------------------------------------------");
                    Console.WriteLine();
                    Console.WriteLine("Please note the provided keys for future reference. Press any key to continue...");
                    Console.ReadKey();
                }
            }

            while(menuOption != 5)
            {
                Console.Clear();
                BlockBO.displayChain(Blockchain);
                menuOption = displayMenu();
                Console.Clear();
                if (menuOption == 1)
                {
                    string data= string.Empty;
                    Console.Write("Enter the data to be added: ");
                    data = Console.ReadLine();
                    Console.WriteLine();

                    string edata = encryptData(data, PublicKey);
                    AddBlockWrapper res = await BlockBO.addBlock(edata, Blockchain, Miners, Complexity);
                    if(res.status == 400)
                    {
                        string verifyStatus = BlockBO.verifyChain(ref Blockchain, Miners);
                        Console.WriteLine("Peer connection error, try again");
                    }
                    else if(res.status == 200)
                    {
                        Console.WriteLine("Block Added Successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Unable to add block, try again");
                    }
                }
                else if (menuOption == 2)
                {
                    string blockId = string.Empty;
                    Console.WriteLine("View last entered block? (Y/N): ");
                    string resp = Console.ReadKey().Key.ToString();
                    Console.Clear();
                    if(resp.ToLower() == "y" )
                    {
                        if (Blockchain.Count == 0) Console.WriteLine("Chain is empty.");
                        else
                        {
                            blockId = Blockchain.Last.Value.id;
                            string res = BlockBO.getBlock(blockId, Blockchain);
                            string data = decryptData(res, PrivateKey);
                            Console.WriteLine("The data is: " + data);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter the Id of the block to view: ");
                        blockId = Console.ReadLine();
                        string res = BlockBO.getBlock(blockId, Blockchain);
                        string data = decryptData(res, PrivateKey);
                        Console.WriteLine("The data is: " + data);
                    }
                }
                else if (menuOption == 3)
                {
                    string res = BlockBO.verifyChain(ref Blockchain, Miners);
                    Console.WriteLine(res);
                }
                else if (menuOption == 4)
                {
                    int count = 0;
                    foreach(Miner miner in Miners)
                    {
                        if (miner.Blockchain.Count == 0) Console.WriteLine($"Miner {++count} has no blocks: " + miner.chainScore.ToString() + " / Coins: " + miner.Coin.ToString());
                        else Console.WriteLine($"Miner {++count}: " + miner.chainScore.ToString() + " / Coins: " + miner.Coin.ToString());
                    }
                }
                else if (menuOption == 5)
                {
                    Console.WriteLine("The chain has been terminated. Hope you learned something new!");
                    break;
                }
                else if (menuOption == 6)
                {
                    if (Blockchain.Count > 1)
                    {
                        Blockchain.RemoveLast();
                        Console.WriteLine("Inconsistency introduced: Removed Last from local chain. Run Option 2 -> Option 1 -> Option 3.");
                    }
                }
                else if (menuOption == 7)
                {
                    if (Miners[0].Blockchain.Count > 1)
                    {
                        Miners[0].Blockchain.RemoveLast();
                        Console.WriteLine("Inconsistency introduced: Removed Last from first Miner chain. Run Option 3 -> Option 4 -> Option 6 -> Option 3. -> Option 4.");
                    }
                    if (Miners[1].Blockchain.Count > 1)
                    {
                        Miners[1].Blockchain.RemoveLast();
                        Console.WriteLine("Inconsistency introduced: Removed Last from second Miner chain. Run Option 3 -> Option 4 -> Option 6 -> Option 3. -> Option 4.");
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid response.");
                }
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

        }

        public static int displayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("=====================================");
            Console.WriteLine("           BLOCKCHAIN_SIM MENU           ");
            Console.WriteLine("=====================================");
            Console.WriteLine("1) Add Block");
            Console.WriteLine("2) Read Block");
            Console.WriteLine("3) Update Chain");
            Console.WriteLine("4) View Miner Info");
            Console.WriteLine("5) Terminate Chain");
            Console.WriteLine("6) Inconsistency: Delete last block of Local chain");
            Console.WriteLine("7) Inconsistency: Alter Miner Chains");
            Console.WriteLine("=====================================");
            Console.WriteLine();
            Console.Write("Please select an option: ");
            char[] temp = Console.ReadKey().Key.ToString().ToCharArray();
            if (temp.Length > 1) return Int32.Parse(temp[1].ToString());
            else return 0;
        }
        
        private static string encryptData(string data, string pubKey)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            // Encrypt data with OAEP padding
            string encryptedString;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(pubKey), out _);
                byte[] encryptedData = rsa.Encrypt(dataBytes, RSAEncryptionPadding.OaepSHA1);
                encryptedString = Convert.ToBase64String(encryptedData);
            }
            return encryptedString;
        }
        
        private static string decryptData(string edata, string prKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(prKey), out _);
                byte[] encryptedData = Convert.FromBase64String(edata);
                byte[] decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA1);
                string decryptedString = Encoding.UTF8.GetString(decryptedData);
                return decryptedString;
            }
        }
    }
}
