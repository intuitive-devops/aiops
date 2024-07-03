
using Boagaphish;
using Boagaphish.Settings;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SoftAgent.Core
{
    public static class Tasks
    {
        public enum CommandType
        {
            docker, dockercompose
        }
        static Process _dockerProcess;
        static DataReceivedEventArgs _dataReceivedEventArgs;
        static string FileName { get; set; }
        static string Output { get; set; }
        // A formatted version of the DockerStats Method
        static string Formatted { get { return DockerStats("0"); } }
        static int exitCode;
        /// <summary>
        /// Is the agent running?
        /// </summary>
        public static bool AgentRunning;
        /// <summary>
        /// Is the agent task running?
        /// </summary>
        public static bool AgentTaskRunning;
        /// <summary>
        /// Transmits a docker or docker-compose command to the container remotely-controlling a daemon.
        /// </summary>
        /// <param name="command">The docker command to be sent.</param>
        /// <returns></returns>
        public static bool TransmitDocker(string command, CommandType type)
        {
            if (type == CommandType.dockercompose)
            {
                FileName = @"docker-compose";
            }
            else if (type == CommandType.docker)
            {
                FileName = "docker";
            }
            try
            {
                var processInfo = new ProcessStartInfo(FileName, command);

                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                processInfo.RedirectStandardOutput = true;
                processInfo.RedirectStandardError = true;

                int exitCode;
                using (var process = new Process())
                {
                    process.StartInfo = processInfo;
                    //process.OutputDataReceived += new DataReceivedEventHandler(Logging.WriteLog(this, Logging.LogType.Information, Logging.LogCaller.Tasks);
                    //process.ErrorDataReceived += new DataReceivedEventHandler(Logging.LoggingDelegate.CreateDelegate());

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit(1200000);
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }

                    exitCode = process.ExitCode;
                    process.Close();
                }
                return true;
            }
            catch (System.Exception message)
            {
                Logging.WriteLog(message.ToString(), Logging.LogType.Information, Logging.LogCaller.Tasks);
                return false;
            }
        }

        public static void TransmitReturnDocker(string command, CommandType type)
        {
            if (type == CommandType.dockercompose)
            {
                FileName = @"docker-compose";
            }
            else if (type == CommandType.docker)
            {
                FileName = "docker";
            }
            try
            {

                //                var startInfo = new ProcessStartInfo
                //                {
                //                    FileName = FileName,
                //                    UseShellExecute = false, // Required to use RedirectStandardOutput
                //                    RedirectStandardOutput = true, //Required to be able to read StandardOutput
                //                    Arguments = command // Skip this if you don't use Arguments
                //};

                //                using (var process = new Process { StartInfo = startInfo })
                //                {
                //                    process.Start();

                //                    process.OutputDataReceived += (sender, line) =>
                //                    {
                //                        if (line.Data != null)
                //                            Console.WriteLine(line.Data);
                //                    };

                //                    process.BeginOutputReadLine();

                //                    process.WaitForExit();
                //                }



                var processInfo = new ProcessStartInfo(FileName, command);

                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                processInfo.RedirectStandardInput = true;
                processInfo.RedirectStandardOutput = true;
                processInfo.RedirectStandardError = true;

                int exitCode;
                using (_dockerProcess = new Process())
                {
                    _dockerProcess.StartInfo = processInfo;
                    _dockerProcess.EnableRaisingEvents = true;
                    //_dockerProcess.OutputDataReceived += new DataReceivedEventHandler(Logging.WriteLog(this, Logging.LogType.Information, Logging.LogCaller.Tasks);
                    //process.ErrorDataReceived += new DataReceivedEventHandler(Logging.LoggingDelegate.CreateDelegate());

                    //_dockerProcess.OutputDataReceived += (sender, _dataReceivedEventArgs) => System.Console.WriteLine(_dataReceivedEventArgs.Data);
                    //_dockerProcess.ErrorDataReceived += (sender, args2) => System.Console.WriteLine(args2.Data);
                    _dockerProcess.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputDataReceived);


                    _dockerProcess.Start();
                    _dockerProcess.BeginOutputReadLine();
                    //_dockerProcess.BeginErrorReadLine();
                    _dockerProcess.WaitForExit(1200000);
                    var writer = _dockerProcess.StandardInput;
                    //writer.WriteLine(Output);
                    //while (true)
                    //{
                    //    writer.WriteLine(Output);
                    //}
                    //_dockerProcess.Close();


                    //Task.Run(() =>
                    //{
                    //    while (true)
                    //    {
                    //        string output = _dockerProcess.StandardOutput.ReadToEnd();
                    //        return output;
                    //    }
                    //});
                    if (!_dockerProcess.HasExited)
                    {
                        _dockerProcess.Kill();
                    }

                    exitCode = _dockerProcess.ExitCode;
                    _dockerProcess.Close();
                    //return Output;
                    //Thread.Sleep(2000);
                }
                //return "---";
            }
            catch (System.Exception message)
            {
                Logging.WriteLog(message.ToString(), Logging.LogType.Information, Logging.LogCaller.Tasks);
                //return "";
            }
        }
        static void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Output = e.Data;
            }
               // Console.WriteLine("Console: {0}", e.Data);
        }
        /// <summary>
        /// Returns a single statistic from a running container.
        /// </summary>
        /// <param name="container">The container to retrieve the statistic on.</param>
        /// <returns>A single line of metrical data.</returns>
        public static string ReturnStatistic(string container)
        {
            TransmitReturnDocker("container stats -a " + container + " --no-stream", CommandType.docker);
            return Output;
        }

        /// <summary>
        /// Spawns a noisy pod with the default set of yaml values.
        /// </summary>
        /// <returns></returns>
        public static string DockerStats(string pod)
        {
            // docker attach and get the statistical data (no prometheus middle)
            if (pod == "0")
            {
                // Return a
            }
            return "Getting statistical data from the pod.";
        }
        public static bool NetworkContainers()
        {
            return true;
        }

        /// <summary>
        /// Create a task for the agent to execute.
        /// </summary>
        /// <returns></returns>
        public static string CreateAgentTask()
        {
            if (AgentTaskRunning)
            {
                return "An agent task is running.";
            }
            AgentTaskRunning = true;
            return "Agent task completed.";
        }
        public static void CreateAgent()
        {
            var agent = new SoftAgent.AgentCore();

        }
    }
}
