using System;
using UnityEngine;

namespace VRCTools
{
    [Serializable]
    internal class VRCTRequest
    {
        public string version = "1.0";
        public string username;
        public string type;
        public string data;

        public VRCTRequest(string type, string data)
        {
            this.username = DeobfGetters.getCurrentUser().id;
            this.type = type;
            this.data = data;
        }

        internal string AsJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}