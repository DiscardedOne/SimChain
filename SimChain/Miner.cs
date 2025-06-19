using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Diagnostics;

namespace SimChain
{
    public class Miner
    {
        public int id;
        public LinkedList<Block> Blockchain;
        public double chainScore;
        public int Coin { get; set; } = 0;

        public Miner(int id) {
            this.id = id;
            this.Blockchain = new LinkedList<Block> ();
            this.chainScore = 100;
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 instance
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert the byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // x2 for specifying hexadecimal representation
                }
                return builder.ToString();
            }
        }

        /*

         First the function will get the prevId from this miner's chain
         Declare a nonce value as 0 or random generated value
         Combine the edata, prevId and nonce using +
         Hash this combined string
         Check if the hash fulfils the criteria of zeroes
         If yes then return otherwise update nonce value and continue

        */
        public async Task<MiningWrapper> mine(string edata, int complexity, CancellationToken token)
        {
            HashSet<int> encounteredNumbers = new HashSet<int>();
            Random random = new Random();
            int nonce;
            string prevId;
            string hash = string.Empty ;

            //Debug.WriteLine($"Miner {id} active");
            if (Blockchain.Count == 0) prevId = new string(Enumerable.Repeat('0', 64).ToArray());
            else prevId = Blockchain.Last.Value.id;

            await Task.Delay(random.Next(10, 500), token);

            do
            {
                nonce = random.Next();
                if (encounteredNumbers.Contains(nonce)) continue;
                if (token.IsCancellationRequested) break;
                hash = ComputeSha256Hash($"{prevId}{edata}{nonce}");
                encounteredNumbers.Add(nonce);
            } while (hash.Substring(0, complexity).Any(c => c != '0'));

            return new MiningWrapper(nonce, hash, id);
        }

        public bool verifyChain(LinkedList<Block> chain)
        {
            if (chain == null) return false;
            if(chain.Count == 0 && Blockchain.Count == 0) return true; // The blockchain is empty
            if(chain.Count != Blockchain.Count) return false;
            
            LinkedListNode<Block> bnode = Blockchain.First;
            LinkedListNode<Block> cnode = chain.First;
            while (bnode != null && cnode != null)
            {
                if(bnode.Value.id != cnode.Value.id || bnode.Value.data != cnode.Value.data || bnode.Value.prevId != cnode.Value.prevId || bnode.Value.nonce != cnode.Value.nonce)
                {
                    return false;
                }
                bnode = bnode.Next;
                cnode = cnode.Next;
            }
            return true;
        }

        public bool verifyBlock(Block block)
        {
            if (block == null) return false;

            int nonce = block.nonce;
            string prevId;
            string hash;

            if (Blockchain.Count == 0) prevId = new string(Enumerable.Repeat('0', 64).ToArray());
            else prevId = Blockchain.Last.Value.id;

            if (prevId != block.prevId) {
                return false; 
            }

            hash = ComputeSha256Hash($"{prevId}{block.data}{block.nonce}");

            byte[] hashByte = Convert.FromHexString(hash);
            byte[] blockHashByte = Convert.FromHexString(block.id);
            bool match = hashByte.SequenceEqual(blockHashByte);

            if (match) return true;
            else return false;
        }

        public bool updateChain(LinkedList<Block> chain)
        {
            if (chain == null) return false;
            if (chain.Count == 0 && Blockchain.Count == 0) return true; // The blockchain is empty

            LinkedListNode<Block> bnode = Blockchain.First;
            LinkedListNode<Block> cnode = chain.First;
            bool flag = false;

            while (cnode != null)
            {
                if( bnode == null)
                {
                    flag = true;
                    break;
                }
                if (bnode.Value.id != cnode.Value.id && bnode.Value.data != cnode.Value.data && bnode.Value.prevId != cnode.Value.prevId && bnode.Value.nonce != cnode.Value.nonce)
                {
                    flag = true;
                    bnode = bnode.Previous;
                    break;
                }
                bnode = bnode.Next;
                cnode = cnode.Next;
            }
            if (flag)
            {
                if (bnode == null) { }
                else if (bnode.Next != null) {
                    LinkedListNode<Block> nodeToDelete = bnode.Next;
                    while (nodeToDelete != null)
                    {
                        LinkedListNode<Block> nextNode = nodeToDelete.Next;
                        Blockchain.Remove(nodeToDelete);
                        nodeToDelete = nextNode;
                    }
                }
                while(cnode != null)
                {
                    Block temp = cnode.Value;
                    Blockchain.AddLast(temp);
                    cnode = cnode.Next;
                }
            }
            return true;
        }

    }
}