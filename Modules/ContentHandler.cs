using HarmonyLib;
using System;
using System.Collections.Generic;

namespace ContentLib.Modules;

/// <summary>
/// Class for handling custom content events
/// </summary>
public class ContentHandler {
    public static List<ContentEvent> EventList = new List<ContentEvent>();

    /// <summary>
    /// Registers the given content event
    /// </summary>
    /// <param name="contentEvent"></param>
    public static void RegisterEvent(ContentEvent contentEvent) {
        // Add the passed contentEvent to the list of events
        Plugin.Logger.LogInfo($"[ContentLib] Registering content event for {contentEvent.GetName()}");
        EventList.Add(contentEvent);
    }

    /// <summary>
    /// A custom method for getting the correct content event ID
    /// Referenced from the ContentLibrary mod
    /// </summary>
    /// <param name="contentEventName"></param>
    /// <returns></returns>
    public static ushort GetEventID(string contentEventName) {
        // The base game reserves IDs 1-1999 around, so we start at 2000.

        Plugin.Logger.LogDebug(EventList.Count);

        // Make sure the event has been registered
        int foundIndex = EventList.FindIndex(match => match.GetType().Name == contentEventName);
        if (foundIndex == -1) {
            for (int index = 0; index < EventList.Count; index++) {
                Plugin.Logger.LogDebug($"[ContentLib_Debug] {EventList[index].GetType().Name}, {contentEventName}, {EventList[index].GetType().Name == contentEventName}");
            }
            Plugin.Logger.LogError($"[ContentLib] GetEventID for {contentEventName} returned -1");
        }

        // Return the sanitized ID
        return (ushort)(2000 + foundIndex);
    }
}

[HarmonyPatch(typeof(ContentEventIDMapper))]
internal class ContentEventIDMapperPatches {
    /// <summary>
    /// Patch for getting the content events to allow custom events to work
    /// Referenced from the ContentLibrary mod
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ContentEventIDMapper.GetContentEvent))]
    public static bool GetContentEventPrefix(ref ushort id, ref ContentEvent __result) {
        Plugin.Logger.LogDebug($"[ContentLib_Debug] GetContentEvent was called: {id} Normalized: {id - 2000} EventList count: {ContentHandler.EventList.Count}");
        // If the ID is part of the base game, get out and allow the original function to run
        if (id - 2000 < 0) return true;

        // Get the content event from the list
        // If it doesn't exist / hasn't been registered, get out and allow the original function to run
        ContentEvent? contentEvent = ContentHandler.EventList[id - 2000];
        if (contentEvent == null) return true;

        // Create a reference instance of the content event type and disallow the original function from running
        __result = (ContentEvent)Activator.CreateInstance(contentEvent.GetType());
        return false;
    }
}