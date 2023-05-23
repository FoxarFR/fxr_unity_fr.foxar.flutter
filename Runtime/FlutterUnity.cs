using System;
using FlutterUnityIntegration;
using Foxar.Core.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace Foxar.Flutter
{
    public class FlutterUnity : Singleton<FlutterUnity>
    {
        [Preserve]
        public static void SendToFlutter(JObject jsonObject, [CanBeNull] string id = null)
        {
            jsonObject["id"] = id ?? $"u_{Guid.NewGuid().ToString()}";
            var outputString = jsonObject.ToString(Formatting.None);
            // print($"SendToFlutter output: {outputString}");
            try
            {
                UnityMessageManager.Instance.SendMessageToFlutter(outputString);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
    }
}