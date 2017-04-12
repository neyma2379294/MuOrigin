using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.Core.GameEvent
{
    /// <summary>
    /// Event listener
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventListener
    {
        /// <summary>
        /// Deal with events
        /// </summary>
        /// <param name="eventObject"></param>
        void processEvent(EventObject eventObject);
    }
}
