using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Tools;

namespace GameDBServer.Core.GameEvent
{
    /// <summary>
    /// Event source
    /// Responsible for connecting the various functional modules, so as to avoid coupling
    /// All the event source when the single case, the server is global
    /// </summary>
    public abstract class EventSource
    {
        //Listener cache
        //There may be a lot of observers interested in an event
        protected Dictionary<int, List<IEventListener>> listeners = new Dictionary<int, List<IEventListener>>();

        public void registerListener(int eventType, IEventListener listener)
        {
            lock (listeners)
            {
                List<IEventListener> listenerList = null;
                if (!listeners.TryGetValue(eventType, out listenerList))
                {
                    listenerList = new List<IEventListener>();
                    listeners.Add(eventType, listenerList);
                }
                listenerList.Add(listener);
            }
        }


        public void removeListener(int eventType, IEventListener listener)
        {
            lock (listeners)
            {
                List<IEventListener> listenerList = null;
                if (!listeners.TryGetValue(eventType, out listenerList))
                {
                    return;
                }
                listenerList.Remove(listener);
            }
        }

        public void fireEvent(EventObject eventObj)
        {
            if (null == eventObj || eventObj.getEventType() == -1)
                return;

            List<IEventListener> listenerList = null;

            if (!listeners.TryGetValue(eventObj.getEventType(), out listenerList))
                return;

            dispatchEvent(eventObj, listenerList);

        }

        private void dispatchEvent(EventObject eventObj, List<IEventListener> listenerList)
        {
            foreach (IEventListener listener in listenerList)
            {
                try
                {
                    listener.processEvent(eventObj);
                }
                catch (System.Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("Event handling error: {0},{1}", (EventTypes)eventObj.getEventType(), ex));
                }

            }
        }
    }
}
