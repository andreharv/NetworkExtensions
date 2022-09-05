using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkExtensions2.Roads.Common
{
    public class NetStrip
    {
        public string ModelName { get; set; }
        public string TextureName { get; set; }
        public NetInfo.Lane TemplateLane { get; set; }
        public NetInfo.Segment TemplateSegment { get; set; }
        public NetInfo.Node TemplateNode { get; set; }
        public bool OverlayPrevious { get; set; }

        public NetStrip(string argModelName, string argTextureName, bool argOverlayPrevious = false)
        {
            ModelName = argModelName;
            TextureName = argTextureName;
            OverlayPrevious = argOverlayPrevious;
        }
        public NetStrip(NetInfo.Segment argSegment, string argModelName, string argTextureName, bool argOverlayPrevious = false)
        {
            TemplateSegment = argSegment;
            ModelName = argModelName;
            TextureName = argTextureName;
            OverlayPrevious = argOverlayPrevious;
        }
        public NetStrip(NetInfo.Node argNode, string argModelName, string argTextureName, bool argOverlayPrevious = false)
        {
            TemplateNode = argNode;
            ModelName = argModelName;
            TextureName = argTextureName;
            OverlayPrevious = argOverlayPrevious;
        }
    }
}
