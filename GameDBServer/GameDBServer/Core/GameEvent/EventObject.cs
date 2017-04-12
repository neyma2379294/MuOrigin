using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.Core.GameEvent
{
    /// <summary>
    /// The most basic event object, in the Core / GameEvent / EventObjectImpl derived from their own needs XXXXXBaseEventObject event object, and their own logic closely related, and then in their own logic to further derived, so that other programmers can immediately understand
    /// How many events exist in the system, and what event objects are available
    /// </summary>
    public abstract class EventObject
    {
        protected int eventType = -1;

        protected EventObject(int eventType)
        {
            this.eventType = eventType;
        }

        public int getEventType()
        {
            return eventType;
        }
    }
}
