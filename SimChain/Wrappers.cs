using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimChain
{
    public class AddBlockWrapper
    {
        public int status;
        public string message = string.Empty;
        public Block? block;
        public AddBlockWrapper(int status, string message = "Unable to add block.", Block? block = null)
        {
            this.status = status;
            this.message = message;
            this.block = block;
        }
    }
    public class MiningWrapper
    {
        public int nonce;
        public string id;
        public int minerId;

        public MiningWrapper(int nonce, string id, int minerId)
        {
            this.nonce = nonce;
            this.id = id;
            this.minerId = minerId;
        }
    }
}
