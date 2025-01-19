using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedie
{
    public class Features
    {
        public Features() { }
        private void SaveCodeToFile(string code, string filePath)
        {
            System.IO.File.WriteAllText(filePath, code);
        }
        private string ExecutePythonScript()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "C:\\Users\\frank\\AppData\\Local\\Programs\\Python\\Python313\\python.exe", // Path to the Python executable
                Arguments = $"\"D:\\Speedie\\speediexec.py\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();

                // Capture the output
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"Python script error: {error}");
                }
                return output;
            }
        }

        public string RunPortScanAndGraph(string target)
        {
            string scriptPath = "D:\\Speedie\\speedexec.py";  // Path to the Python script

            // Python code to write
            string pythonCode = $@"
import socket
import matplotlib.pyplot as plt

def port_scan(target):
    open_ports = []
    for port in range(1, 1025):
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        result = sock.connect_ex((target, port))
        if result == 0:
            open_ports.append(port)
        sock.close()
    return open_ports

def create_graph(open_ports):
    plt.bar(open_ports, [1]*len(open_ports), color='green')
    plt.xlabel('Open Ports')
    plt.ylabel('Status')
    plt.title('Port Scan Results')
    plt.savefig('port_scan_results.png')
    plt.close()

if __name__ == '__main__':
    target = '{target}'
    open_ports = port_scan(target)
    create_graph(open_ports)
";

            // Save the code to a file
            //SaveCodeToFile(pythonCode, scriptPath);

            // Execute the Python script
            return ExecutePythonScript();
        }
    }
}
