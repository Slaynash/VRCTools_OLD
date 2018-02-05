using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VRCTools
{
    [Serializable]
    class SerializableApiAvatar
    {
        public string id;
        public string name;
		public string imageUrl;
		public string authorName;
		public string authorId;
        public string assetUrl;
		public string description;
		public string tags;
		public string version;
        public string unityPackageUrl;
        public string thumbnailImageUrl;

        public SerializableApiAvatar(string id, string name, string imageUrl, string authorName, string authorId, string assetUrl, string description, string tags, string version)
        {
            this.id = id;
            this.name = name;
            this.imageUrl = imageUrl;
            this.authorName = authorName;
            this.authorId = authorId;
            this.assetUrl = assetUrl;
            this.description = description;
            this.tags = tags;
            this.version = version;
        }
    }
}
