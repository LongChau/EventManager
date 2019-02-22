using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Key.Ultility
{
    public sealed class EventManager : MySingleton<EventManager>
    {
        private Dictionary<EventID, MyCustomEvent> dictListeners = new Dictionary<EventID, MyCustomEvent>();

        public override void Init()
        {
            base.Init();
            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable()
        {
            Log.Info("EventManager:OnDisable()");
            Key_ClearAllListener();
        }

        protected void OnDestroy()
        {
            Log.Info("EventManager:OnDestroy()");
            Instance = null;
        }

        public void Key_RegisterListener(EventID eventID, UnityAction<object> callback)
        {
            if (dictListeners.ContainsKey(eventID))
            {
                dictListeners[eventID].AddListener(callback);
            }
            else
            {
                dictListeners.Add(eventID, new MyCustomEvent());
                dictListeners[eventID].AddListener(callback);
            }
        }

        public void Key_PostEvent(EventID eventID, object param = null)
        {
            if (!dictListeners.ContainsKey(eventID))
            {
                Log.Warning($"No listener for this event: {eventID.ToString()}");
                return;
            }

            dictListeners[eventID]?.Invoke(param);

            if (dictListeners[eventID] == null)
            {
                Log.Warning($"PostEvent {eventID}, but no listener remain, Remove this key");
                dictListeners.Remove(eventID);
            }
        }

        public void Key_RemoveListener(EventID eventID, UnityAction<object> callback)
        {
            // checking params
            Log.Assert(callback != null, "RemoveListener, event {0}, callback = null !!", eventID.ToString());
            Log.Assert(eventID != EventID.None, "AddListener, event = None !!");

            dictListeners[eventID]?.RemoveListener(callback);

            if (!dictListeners.ContainsKey(eventID))
            {
                Log.Warning(false, "RemoveListener, not found key : " + eventID);
            }
        }

        public void Key_ClearAllListener()
        {
            foreach (var eventCallback in dictListeners)
            {
                eventCallback.Value.RemoveAllListeners();
                Log.Info($"Event {eventCallback.Key.ToString()} removed");
            }

            dictListeners.Clear();
        }
    }
}


#region Extension class

namespace Key.Ultility
{
    /// <summary>
    /// Delare some "shortcut" for using EventDispatcher easier
    /// </summary>
    public static class EventManagerSystemExtension
    {
        /// /// <summary>
        /// Use for registering with EventsManager
        /// </summary>
        /// "this MonoBehaviour listener" for all MonoBehaviour can access
        public static void Key_RegisterListener(this MonoBehaviour listener, EventID eventID, UnityAction<object> callback)
        {
            EventManager.Instance?.Key_RegisterListener(eventID, callback);
        }

        /// <summary>
        /// Post event with param
        /// </summary>
        public static void Key_PostEvent(this MonoBehaviour listener, EventID eventID, object param)
        {
            EventManager.Instance?.Key_PostEvent(eventID, param);
        }

        /// <summary>
        /// Post event with no param (param = null)
        /// </summary>
        public static void Key_PostEvent(this MonoBehaviour sender, EventID eventID)
        {
            EventManager.Instance?.Key_PostEvent(eventID, null);
        }

        /// <summary>
        /// Post event with no param (param = null)
        /// </summary>
        public static void Key_RemoveEvent(this MonoBehaviour sender, EventID eventID, UnityAction<object> callback)
        {
            EventManager.Instance?.Key_RemoveListener(eventID, callback);
        }
    }
}

#endregion Extension class
