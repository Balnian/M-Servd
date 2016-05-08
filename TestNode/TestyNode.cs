using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerNodeCore;

namespace TestNode
{
    [NodeMeta(ProjectName: "Underworld", ProjectID: "u", NodeName: "TestListener", NodeID: "asd") ]
    public class ServerNode: IServerNode
    {
        object IServerNode.Auth(object Data)
        {
            throw new NotImplementedException();
        }

        object IServerNode.ExecuteLogic(object PlayerData, object Data)
        {
            throw new NotImplementedException();
        }
    }
}
