using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.Server
{
    public interface ICmdProcessor
    {
        void processCmd(GameServerClient client, byte[] cmdParams, int count);
    }

}
