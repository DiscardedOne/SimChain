using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimChain
{
    public class Block
    {
        public Block(string data, string prevId) {
            this.data = data;
            this.prevId = prevId;
            this.nonce = 0;
        }

        public string id;
        public string data;
        public string prevId;
        public int nonce;
    }
}
