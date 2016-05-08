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
        /// <summary>
        /// Name of the project that this node is part of
        /// </summary>
        public String ProjectName { get; set; }
        /// <summary>
        /// ID used to identify the project (will be concatenated with the NodeID to create a unique ID)
        /// </summary>
        public String ProjectID { get; set; }
        /// <summary>
        /// Name of this node
        /// </summary>
        public String NodeName { get; set; }
        /// <summary>
        /// ID used to identify the node (will be concatenated with the ProjectID to create a unique ID)
        /// </summary>
        public String NodeID { get; set; }

        /// <summary>
        /// Unique ID that will be use to call this node.
        /// The ID is formated like this : {ProjectID}.{NodeID}
        /// </summary>
        public String NodeUniqueID => $"{ProjectID}.{NodeID}";
        public NodeMeta(String ProjectName, String ProjectID,String NodeName, String NodeID)
        {
            this.ProjectName = ProjectName;
            this.ProjectID = ProjectID;
            this.NodeName = NodeName;
            this.NodeID = NodeID;
        }

    }
}
