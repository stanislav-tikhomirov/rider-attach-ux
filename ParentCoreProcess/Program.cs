// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Lib;

const int port = 12345;
const string id = "Parent Process";

var startInfo = new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = $"ChildProcess.dll {port}"
};
Process.Start(startInfo);

await Connections.StartConnection(port, id);