using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimChain
{
    public class BlockBO
    {
        public BlockBO() { }

        public static async Task<AddBlockWrapper> addBlock(string edata, LinkedList<Block> blockchain, List<Miner> miners, int complexity)
        {
            if (edata == null || blockchain == null || miners == null) return new AddBlockWrapper(500, "Values are null", null);

            string prevId;
            if (blockchain.Count == 0) prevId = new string(Enumerable.Repeat('0', 64).ToArray());
            else prevId = blockchain.Last.Value.id;
            Block block = new Block(edata, prevId);
            bool flag = false;
            int mId = -1;
            double acceptRate = 0;
            int count = 0;
            int reward = (10*complexity) - (int)Math.Floor((double)blockchain.Count/10);
            reward =  (reward<0) ? 0 : reward;

            Console.Write("Mining in Progress..");
            do
            {
                Console.Write(".");
                CancellationTokenSource cts = new CancellationTokenSource();
                List<Task<MiningWrapper>> tasks = miners.Select(miner => miner.mine(edata, complexity, cts.Token)).ToList();
                
                Task<MiningWrapper> firstCompletedtask = await Task.WhenAny(tasks);
                Console.Write(".");
                firstCompletedtask.Wait();
                if (firstCompletedtask.IsCompletedSuccessfully)
                {
                    cts.Cancel();
                    flag = true;
                    mId = firstCompletedtask.Result.minerId;
                    Console.WriteLine();
                    Console.WriteLine("Mining Complete by Miner " + mId);
                    Console.WriteLine("BlockId is " + firstCompletedtask.Result.id);
                    block.nonce = firstCompletedtask.Result.nonce;
                    block.id = firstCompletedtask.Result.id;
                }
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Mining Terminated");
                }
            } while (!flag);

            foreach(Miner miner in miners)
            {
                if (miner.id == mId) continue;
                if (miner.verifyBlock(block)) count++;
            }
            acceptRate = (double)count/(miners.Count-1);
            count = 0;
            Debug.WriteLine($"verification accept rate: {acceptRate}");
            if (acceptRate*100 < 90) return new AddBlockWrapper(500);
            blockchain.AddLast(block);

            foreach (Miner miner in miners)
            {
                if (miner.updateChain(blockchain)) count++;
            }
            acceptRate = (double)count/(miners.Count);
            count = 0;
            Debug.WriteLine($"modification accept rate: {acceptRate}");
            if (acceptRate*100 < 50) return new AddBlockWrapper(400);

            foreach(Miner miner in miners)
            {
                if(miner.id == mId) miner.Coin += reward;
            }
            return new AddBlockWrapper(200);
        }
        
        public static string getBlock(string id, LinkedList<Block> blockchain)
        {
            LinkedListNode<Block> node = blockchain.First;

            while (node != null)
            {
                if (node.Value.id == id) return node.Value.data;
                node = node.Next;
            }
            return "Not Found";
        }

        public static string verifyChain(ref LinkedList<Block> blockchain, List<Miner> miners)
        {
            int count = 0;
            double acceptRate = 0;

            foreach (Miner miner in miners)
            {
                if (miner.verifyChain(blockchain)) count++;
            }
            acceptRate = (double)count / miners.Count;
            count = 0;

            if (acceptRate * 100 > 90) return "Chain Integrity Verified";

            Miner peakMiner = new Miner(0);
            double peakChainScore = 0;

            foreach(Miner miner in miners)
            {
                int mId = miner.id;
                int mcount = 0;
                double mAcceptRate = 0;
                foreach(Miner m in miners)
                {
                    if (m.id == mId) continue;
                    if(m.verifyChain(miner.Blockchain)) mcount++;
                }
                mAcceptRate = (double)mcount / (miners.Count-1);
                miner.chainScore = mAcceptRate * 100;
                if (miner.chainScore > peakChainScore) {
                    peakChainScore = miner.chainScore;
                    peakMiner = miner;
                } 
            }

            int outdatedMiners = 0; // This logs all the Miners that were outdated
            foreach (Miner miner in miners)
            {
                if(miner.chainScore < peakChainScore - 10 || miner.chainScore < 50)
                {
                    if(miner.updateChain(peakMiner.Blockchain)) outdatedMiners++;
                } 
            }

            // The following code is resposible for updating the localchain with the most accepted chain

            LinkedListNode<Block> bnode = blockchain.First;
            LinkedListNode<Block> cnode = peakMiner.Blockchain.First;
            bool flag = false;

            while (cnode != null)
            {
                if (bnode == null)
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
                else if (bnode.Next != null)
                {
                    LinkedListNode<Block> nodeToDelete = bnode.Next;
                    while (nodeToDelete != null)
                    {
                        LinkedListNode<Block> nextNode = nodeToDelete.Next;
                        blockchain.Remove(nodeToDelete);
                        nodeToDelete = nextNode;
                    }
                }
                while (cnode != null)
                {
                    blockchain.AddLast(cnode.Value);
                    cnode = cnode.Next;
                }
            }

            return "Chain updated.";
        }
    
        public static void displayChain(LinkedList<Block> blockchain)
        {
            Console.WriteLine("-- Current Blockchain State --");
            Console.WriteLine();
            if (blockchain == null || blockchain.Count == 0)
            {
                Console.WriteLine("The blockchain is empty.");
                return;
            }

            for(int i = 0; i < blockchain.Count; i++)
            {
                Console.Write("[X]");
                if(i != blockchain.Count - 1) Console.Write(" -- ");
            }

            Console.WriteLine();
        }
    }
}
