using ServerNodeCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace M_Servd
{



    class Server
    {
        #region Singleton

        /// <summary>
        /// Only instance of the server
        /// </summary>
        private static volatile Server m_instance;

        /// <summary>
        /// Object to lock the server instance creation 
        /// (we cannot use this AKA m_instance see "Multithreaded Singleton" @ https://msdn.microsoft.com/en-us/library/ff650316.aspx)
        /// </summary>
        private static object syncInstance = new object();
        private Server() { registerInternalCommands(); }

        /// <summary>
        /// Create and/or give the only instance of the server (is thread sage)
        /// </summary>
        public static Server Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (syncInstance)
                    {
                        if (m_instance == null)
                            m_instance = new Server();
                    }
                }
                return m_instance;
            }
        }
        #endregion

        #region State
        /// <summary>
        /// Possible states of the server
        /// </summary>
        public enum ServerState
        {
            Running,
            Stopping,
            Halted,
            Restarting
        }

        /// <summary>
        /// State of the server
        /// </summary>
        public ServerState State { get; private set; } = ServerState.Halted;
        #endregion

        #region Start/Receive Connection

        /// <summary>
        /// handle the async lock of the tcplistener
        /// </summary>
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        /// <summary>
        /// Run the server in an async mode
        /// </summary>
        /// <param name="stateInfo">might be useless (used for the threadpool?)</param>
        public void RunServerAsync(Object stateInfo)
        {
            if (State == ServerState.Halted)
            {
                do
                {
                    if (State == ServerState.Restarting) Log.Success("Server Restarting...");

                    TcpListener Serverlistener = new TcpListener(IPAddress.Any, ServerListeningPort);
                    try
                    {
                        Serverlistener.Server.ReceiveTimeout = ServerReceiveTimeOut;
                        Serverlistener.Start();
                        State = ServerState.Running;
                        Log.Success($"Server started and listening at port: {ServerListeningPort}");
                    }
                    catch (Exception e)
                    {
                        State = ServerState.Stopping;
                        Serverlistener?.Stop();
                        Log.Error("Server failed to Start");
                        Log.Error(e.Message);
                    }

                    while (State == ServerState.Running)
                    {
                        tcpClientConnected.Reset();
                        Serverlistener.BeginAcceptTcpClient(new AsyncCallback(handleClient), Serverlistener);

                        tcpClientConnected.WaitOne(ServerReceiveTimeOut);
                        processCommands();
                    }
                    Serverlistener?.Stop();
                    Log.Warning("Server Stopping...");
                } while (State == ServerState.Restarting);
                State = ServerState.Halted;
                Log.Success("Server Stopped");
            }

        }

        /// <summary>
        /// Callback when a client connect to the server
        /// </summary>
        /// <param name="result"></param>
        private void handleClient(IAsyncResult result)
        {
            tcpClientConnected.Set();
            TcpListener listener = (TcpListener)result.AsyncState;
            if (State == ServerState.Running)
                using (TcpClient client = listener.EndAcceptTcpClient(result))
                {
                    //Schtuff
                }

        }


        #endregion

        #region Server Util

        /// <summary>
        /// Start the server
        /// </summary>
        public void StartServer()
        {
            ThreadPool.QueueUserWorkItem(Server.Instance.RunServerAsync);
        }

        /// <summary>
        /// Halt the server
        /// </summary>
        public void HaltServer()
        {
            if (State != ServerState.Halted || State != ServerState.Stopping)
            {
                State = ServerState.Stopping;
            }
        }

        /// <summary>
        /// Restart the server
        /// </summary>
        public void RestartServer()
        {
            if (State == ServerState.Running)
            {
                State = ServerState.Restarting;
            }
        }

        /// <summary>
        /// Port at which the server will listen
        /// </summary>
        public int ServerListeningPort { get; set; } = 308;

        /// <summary>
        /// Time in millisecond to which te server will recheck his state (mainly for Halting)
        /// </summary>
        public int ServerReceiveTimeOut { get; set; } = 500;



        #endregion

        #region Server Command

        /// <summary>
        /// Contains a command to be executed
        /// </summary>
        public delegate void CommandItemCallback();

        /// <summary>
        /// Hold Data for a command
        /// </summary>
        public class CommandItem
        {
            public String Description { get; set; }
            public CommandItemCallback Executable { get; set; }
        }

        /// <summary>
        /// Callback to a function which will create a CommandItem based on the input command
        /// </summary>
        /// <param name="commandStr">inputed command parameters</param>
        /// <returns></returns>
        public delegate CommandItem CommandFactoryItem(String commandStr);
        Dictionary<String, CommandFactoryItem> m_commandFactory = new Dictionary<String, CommandFactoryItem>();
        Queue<CommandItem> m_commandQueue = new Queue<CommandItem>();

        /// <summary>
        /// Ask the server to execute a command
        /// </summary>
        /// <param name="commandStr">Command to execute</param>
        public void executeCommand(string commandStr)
        {

            lock (m_commandQueue)
            {

                lock (m_commandFactory)
                {
                    String[] splitProduce = commandStr.Split(':');
                    for (int i = 0; i < splitProduce.Count(); i++)
                        splitProduce[i].Trim();

                    if (splitProduce.Count() > 1)
                    {
                        if (m_commandFactory.ContainsKey(splitProduce[0]))
                            m_commandQueue.Enqueue(m_commandFactory[splitProduce[0]](splitProduce[1]));
                    }
                    else if (splitProduce.Count() == 1)
                    {
                        if (m_commandFactory.ContainsKey(splitProduce[0]))
                            m_commandQueue.Enqueue(m_commandFactory[splitProduce[0]](""));
                    }

                }
            }
            // If server is not running we process the command on the calling thread
            if (State != ServerState.Running)
                processCommands();
        }

        /// <summary>
        /// Process (Execute) all commands currently in the queue
        /// </summary>
        private void processCommands()
        {
            lock (m_commandQueue)
            {
                while (m_commandQueue.Count > 0)
                    m_commandQueue.Dequeue().Executable();
            }
        }

        /// <summary>
        /// Register a new command to te server and return if the command was succesfully registered
        /// </summary>
        /// <param name="commandStr">String which trigger the command</param>
        /// <param name="commandFactory">function that will output wath will be executed</param>
        /// <returns>return true if the command was succesfully registered</returns>
        public bool registerCommand(String commandStr, CommandFactoryItem commandFactory)
        {
            bool wasSuccesful = false;
            if (!m_commandFactory.ContainsKey(commandStr))
            {
                wasSuccesful = true;
                m_commandFactory.Add(commandStr, commandFactory);
            }
            return wasSuccesful;
        }

        /// <summary>
        /// Register default server commands
        /// </summary>
        private void registerInternalCommands()
        {
            registerCommand("Start", str =>
            {
                return new CommandItem
                {
                    Description = "Start the server in a new Thread",
                    Executable = () =>
                {
                    StartServer();
                }
                };
            });

            registerCommand("Halt", str =>
            {
                return new CommandItem
                {
                    Description = "Stop the Server",
                    Executable = () =>
                    {
                        HaltServer();
                    }
                };
            });

            registerCommand("Restart", str =>
            {
                return new CommandItem
                {
                    Description = "Restart the Server",
                    Executable = () =>
                    {
                        RestartServer();
                    }
                };
            });

            registerCommand("Status", str =>
            {
                return new CommandItem
                {
                    Description = "Give the current Status of the Server",
                    Executable = () =>
                    {
                        Console.WriteLine(State.ToString());
                    }
                };
            });

            registerCommand("EchoArgs", str =>
            {
                return new CommandItem
                {
                    Description = "Echo back the arguments of the command",
                    Executable = () =>
                    {
                        Console.WriteLine(str);
                    }
                };
            });

            registerCommand("Man", str =>
             {
                 return new CommandItem
                 {
                     Description = "List all commands available",
                     Executable = () =>
                     {
                         const int CommandWidth = 10;
                         const int DescriptionWidth = 60;
                         Console.WriteLine("");
                         foreach (var item in m_commandFactory.OrderBy(x => x.Key))
                         {
                             String Desc = item.Value("").Description;

                             Console.WriteLine($" {item.Key,-CommandWidth}{Desc.Substring(0, Math.Min(DescriptionWidth, Desc.Length)),-DescriptionWidth}");
                             for (int i = 1; i < Math.Ceiling((double)Desc.Length / DescriptionWidth); i++)
                             {
                                 Console.WriteLine($" {"",-CommandWidth}{Desc.Substring(i * DescriptionWidth, Math.Min(DescriptionWidth, Desc.Length - (i * DescriptionWidth))),-DescriptionWidth}");
                             }
                             Console.WriteLine("");
                         }

                     }
                 };
             });

            registerCommand("LoadNode", str =>
            {
                return new CommandItem
                {
                    Description = "Load the node from the specified File or all files in the specified directory",
                    Executable = () =>
                    {
                        if(Directory.Exists(str))
                        {
                            LoadAllServerNodeFromDirectory(str);
                        }
                        else if(File.Exists(str))
                        {
                            LoadServerNodeFromFile(str);
                        }
                        else
                        {
                            Log.Warning($"Target \"{str}\" doesn't exist");
                        }
                        foreach (var item in m_nodes)
                        {
                            Log.Debug(item.Key);
                        }
                    }
                };
            });

            registerCommand("ListNodes", str =>
            {
                return new CommandItem
                {
                    Description = "List all currently loaded nodes",
                    Executable = () =>
                    {
                        lock (m_nodes)
                        {
                            foreach (var item in m_nodes)
                            {
                                NodeMeta meta = Attribute.GetCustomAttributes(item.Value).FirstOrDefault(x => x is NodeMeta) as NodeMeta;

                                Log.Debug($"({item.Key}) Node {meta.NodeName} From project {meta.ProjectName}");
                            }
                        }
                    }
                };
            });

        }
        #endregion

        #region ServerNode Management

        /// <summary>
        /// Contains all loaded server nodes
        /// </summary>
        Dictionary<String, Type> m_nodes = new Dictionary<string, Type>();

        /// <summary>
        /// Try to load a node from a file
        /// </summary>
        /// <param name="path">Path of the file</param>
        void LoadServerNodeFromFile(String path)
        {
            path.Trim();

            if (File.Exists(path))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(path);

                    Type[] Nodetypes = assembly.GetTypes();

                    foreach (var Nodetype in Nodetypes)
                    {
                        bool isValidNode = false;
                        System.Attribute[] attrs = System.Attribute.GetCustomAttributes(Nodetype);
                        foreach (var attr in attrs)
                        {
                            if (attr is NodeMeta)
                            {
                                isValidNode = true;
                                NodeMeta meta = (NodeMeta)attr;
                                lock (m_nodes)
                                {
                                    if (m_nodes.ContainsKey(meta.NodeUniqueID))
                                    {
                                        NodeMeta oldMeta = Attribute.GetCustomAttributes(m_nodes[meta.NodeUniqueID]).FirstOrDefault(x => x is NodeMeta) as NodeMeta;
                                        Log.Warning($"Loading nodes from {path} and NodeID \"{meta.NodeUniqueID}\" was already register from Project \"{oldMeta?.ProjectName}\" with node Name \"{oldMeta?.NodeName}\" and will be override");
                                    }
                                    m_nodes[meta.NodeUniqueID] = Nodetype;
                                }
                            }
                        }
                    }
                }catch(Exception e)
                {
                    Log.Warning($"Failed to load Server Node from File : {path}");
                }
            }
            else
            {
                Log.Error($"File \"{path}\" doesn't exist");
            }

        }

        /// <summary>
        /// Try to load nodes from all files of a directory
        /// </summary>
        /// <param name="path"></param>
        void LoadAllServerNodeFromDirectory(String path)
        {
            if(Directory.Exists(path))
            {
                String[] files = Directory.GetFiles(path);
                if(files.Length > 0)
                {
                    foreach (String file in files)
                    {
                        LoadServerNodeFromFile(file);
                    }
                }
                else
                {
                    Log.Warning($"Directory \"{path}\" is empty");
                }
            }
            else
            {
                Log.Error($"Directory \"{path}\" doesn't exist");
            }
        }

        bool CreateServerNode()
        {

            return false;
        }

        #endregion

    }
}
