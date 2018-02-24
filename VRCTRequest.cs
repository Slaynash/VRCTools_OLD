using System;
using UnityEngine;

namespace VRCTools
{
    [Serializable]
    internal class VRCTRequest
    {
        public string version = "1.1";
        public string uuid;
        public string username;
        public string type;
        public string data;

        public VRCTRequest(string type, string data)
        {
            GKGODKNLLII cu = DeobfGetters.getCurrentUser();
            if (cu != null)
            {
                this.uuid = cu.id;
                this.username = cu.username;
            }
            else
            {
                this.uuid = "usr_none";
                this.username = "<none>";
            }
            this.type = type;
            this.data = data;
        }

        internal string AsJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}