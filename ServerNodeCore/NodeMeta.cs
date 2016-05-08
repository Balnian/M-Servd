using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerNodeCore
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class NodeMeta : System.Attribute
    {
        public String ProjectName { get; set; }
        public String ProjectID { get; set; }
        public String NodeName { get; set; }
        public String NodeID { get; set; }

        public String NodeFullID => $"{ProjectID}.{NodeID}";
        public NodeMeta(String ProjectName, String ProjectID,String NodeName, String NodeID)
        {
            this.ProjectName = ProjectName;
            this.ProjectID = ProjectID;
            this.NodeName = NodeName;
            this.NodeID = NodeID;
        }

    }
}
