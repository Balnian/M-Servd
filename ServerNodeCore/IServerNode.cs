using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerNodeCore
{
    public interface IServerNode
    {
        /// <summary>
        /// Authenticate the caller
        /// and return an object that represent the caller
        /// or other data defined by the dev
        /// </summary>
        /// <param name="Data">Data to Authenticate the user (from the network)</param>
        /// <returns>An arbitrary object containing the authenticated user data or other data defined by the devs</returns>
        object Auth(object Data);

        /// <summary>
        /// Execute node specific logic
        /// </summary>
        /// <param name="PlayerData">Data return by the Auth method</param>
        /// <param name="Data">Data from the caller (from the network)</param>
        /// <returns>An object that will be send as an answer to the caller</returns>
        object ExecuteLogic(object PlayerData, object Data,object NodeSavedData);
    }
}
