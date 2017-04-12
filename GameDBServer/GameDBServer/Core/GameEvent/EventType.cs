using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.Core.GameEvent
{
    /// <summary>
    /// Event type definition, only allow the definition of large events, the same type of events need to break the type, please define their own variables within their own distinction (example: see Zhanmeng event logic / BangHui / ZhanMengShiJian / ZhanMengShiJianManager.cs)
    /// </summary>
    public enum EventTypes
    {
        PlayerLogin,//Player login
        PlayerLogout,//The player logs out
    }
}
