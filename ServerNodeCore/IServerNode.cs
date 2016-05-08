using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerNodeCore
{
    public interface IServerNode
    {
        object Auth(object Data);

        object ExecuteLogic(object PlayerData, object Data);
    }
}
