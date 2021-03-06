﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Logic;
using Server.Protocol;

namespace GameServer.Server
{
    public interface ICmdProcessor
    {
        bool processCmd(GameClient client, string[] cmdParams);
    }
}
