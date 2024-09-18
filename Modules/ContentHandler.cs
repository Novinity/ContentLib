using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContentLib.Modules;

public class ContentHandler {
    public static List<ContentEvent> EventList = new List<ContentEvent>();

    public static void RegisterEvent(ContentEvent contentEvent) {
        Plugin.Logger.LogInfo($"[ContentLib] Registering content event for {contentEvent.GetName()}");
        EventList.Add(contentEvent);
    }

    public static ushort GetEventID(string contentEventName) {
        Plugin.Logger.LogInfo(EventList.Count);

        int foundIndex = EventList.FindIndex(match => match.GetType().Name == contentEventName);
        if (foundIndex == -1) {
            for (int index = 0; index < EventList.Count; index++) {
                Plugin.Logger.LogInfo($"[ContentLib_Debug] {EventList[index].GetType().Name}, {contentEventName}, {EventList[index].GetType().Name == contentEventName}");
            }
            Plugin.Logger.LogError($"[ContentLib] GetEventID for {contentEventName} returned -1");
        }

        return (ushort)(2000 + foundIndex);
    }
}

[HarmonyPatch(typeof(ContentEventIDMapper))]
internal class ContentEventIDMapperPatches {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ContentEventIDMapper.GetContentEvent))]
    public static bool GetContentEventPrefix(ref ushort id, ref ContentEvent __result) {
        Plugin.Logger.LogDebug($"[ContentLib_Debug] GetContentEvent was called: {id} Normalized: {id - 2000} EventList count: {ContentHandler.EventList.Count}");
        if (id - 2000 < 0) return true;

        ContentEvent? contentEvent = ContentHandler.EventList[id - 2000];
        if (contentEvent == null) return true;

        __result = (ContentEvent)Activator.CreateInstance(contentEvent.GetType());
        return false;
    }
}