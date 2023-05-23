using System;
using System.Collections.Generic;
using FlutterUnityIntegration;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace Foxar.Flutter
{
    [Serializable]
    public class FlutterUnityMessageEvent : UnityEvent<FlutterMessageReceivedEvent, FlutterMessageCallback>
    {
    }

    [Serializable]
    public class FlutterUnityMessageEventItem
    {
        public string action;
        public FlutterUnityMessageEvent flutterUnityMessageEvent;
    }

    [RequireComponent(typeof(UnityMessageManager))]
    public class OnFlutterMessage : MonoBehaviour
    {
        [SerializeField] private List<FlutterUnityMessageEventItem> _events = new();

        [Preserve]
        public void Call(string message)
        {
            var (messageReceivedEvent, eventResult) = GetReceivedEvent(message);
            eventResult.flutterUnityMessageEvent.Invoke(messageReceivedEvent, null);
        }

        [Preserve]
        public void CallAndReturn(string message)
        {
            var (messageReceivedEvent, eventResult) = GetReceivedEvent(message);
            var messageCallback = new FlutterMessageCallback();
            messageCallback.AddListener(jsonObject =>
            {
                // print($"SendToFlutter output: {jsonObject}, id: {messageReceivedEvent.id}");
                FlutterUnity.SendToFlutter(jsonObject, messageReceivedEvent.id);
                messageCallback.RemoveAllListeners();
            });
            eventResult.flutterUnityMessageEvent.Invoke(messageReceivedEvent, messageCallback);
        }

        [Preserve]
        private (FlutterMessageReceivedEvent, FlutterUnityMessageEventItem) GetReceivedEvent(string message)
        {
            var messageReceivedEvent = JsonConvert.DeserializeObject<FlutterMessageReceivedEvent>(message);
            var eventResult = _events.Find(e => e.action == messageReceivedEvent.action);
            if (eventResult == null)
            {
                throw new Exception($"No action found for {messageReceivedEvent.action}");
            }

            return (messageReceivedEvent, eventResult);
        }
    }

    [Serializable]
    public class FlutterMessageReceivedEvent
    {
        public string id;
        public string action;
        [CanBeNull] public object arguments;
    }

    [Serializable]
    public class FlutterMessageCallback : UnityEvent<JObject>
    {
    }
}