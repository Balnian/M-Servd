using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Servd
{
    class Program
    {
        private static volatile bool Exit = false;

        static void Main(string[] args)
        {
            Server.Instance.executeCommand("EchoArgs: Allo banas=");

            Server.Instance.registerCommand("test", str =>
             {
                 return new Server.CommandItem
                 {
                     Description = "asddasd asdjksdjksdfjksdfjk sadkjbsdfajfjfdsajdfas jbdfsjhdfsajhfdsajhfdashjl; dsfnhjdfshjdasfhjdfashjldfsa dsfahjdfshjdsfhjl;dsfhdjl sdafnhjfsdhjklsdfhjl;dfsa ndsfjkdsfahjdfsahjdsfa sdfahujdsfahjkdsf"
                 };
             });
            //Server.Instance.StartServer();
            Server.Instance.registerCommand("Exit", str =>
            {
                return new Server.CommandItem
                {
                    Description = "Halt the server and exit application",
                    Executable = () =>
                    {
                        Server.Instance.HaltServer();
                        Exit = true;
                        Log.Message("The Server will now stop (You may have to Press enter to exit)");
                    }
                };
            });
            while (!Exit/*&& Server.Instance.State != Server.ServerState.Halted*/)
            {
                String input = Console.ReadLine();
                if (input == "qwerty")
                {
                    Server.Instance.HaltServer();
                    Exit = true;
                }
                Server.Instance.executeCommand(input);
            };
        }
    }
}
