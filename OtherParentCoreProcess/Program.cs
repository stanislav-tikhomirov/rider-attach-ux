// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Lib;

const int port = 12346;
const string id = "Other Parent Process";

var startInfo = new ProcessStartInfo
{
    FileName = "dotnet",
    Arguments = $"ChildProcess.dll {port}"
};
Process.Start(startInfo);

await Connections.StartConnection(port, id);