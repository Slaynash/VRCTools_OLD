using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRCSDK2;

namespace VRCTools
{
    [RequireComponent(typeof(VRC_EventHandler))]
    class VRCT_Trigger : VRC_Trigger
    {

        private Action onInteract;

        public override void Interact()
        {
            base.Interact();
            onInteract?.Invoke();
        }

        public static VRCT_Trigger CreateVRCT_Trigger(GameObject parent, Action onInteract)
        {
            VRCT_Trigger t = parent.AddComponent<VRCT_Trigger>();

            TriggerEvent te = new TriggerEvent();
            te.Name = "triggerButton";
            te.BroadcastType = VRC_EventHandler.VrcBroadcastType.Local;
            te.AfterSeconds = 0f;
            te.TriggerType = TriggerType.OnInteract;

            t.Triggers.Add(te);

            t.onInteract += onInteract;

            return t;
        }
    }
}
