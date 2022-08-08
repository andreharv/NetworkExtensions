using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkExtensions2.Roads.Common
{
    public class LaneRecipe
    {
        public string ModelName { get; set; }
        public string TextureName { get; set; }
        public int order { get; set; }
        public NetInfo.Segment TemplateSegment { get; set; }
        public NetInfo.Node TemplateNode { get; set; }

        public LaneRecipe(NetInfo.Segment argSegment, string argModelName, string argTextureName)
        {
            TemplateSegment = argSegment;
            ModelName = argModelName;
            TextureName = argTextureName;
        }
        public LaneRecipe(NetInfo.Node argNode, string argModelName, string argTextureName)
        {
            TemplateNode = argNode;
            ModelName = argModelName;
            TextureName = argTextureName;
        }
    }
}
