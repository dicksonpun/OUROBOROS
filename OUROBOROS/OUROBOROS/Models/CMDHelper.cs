using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OUROBOROS
{
    public class CMDHelper
    {
        // Members
        protected string m_arguments;
        protected int m_timeoutInMilliseconds;

        // Methods
        public virtual void ProcessOutput(string output) { Console.WriteLine(output); }
        public virtual void ProcessError(string error) { Console.WriteLine(error); }
        public virtual void ProcessTimeout() { Console.WriteLine("Process Timeout."); }

        // Constructor
        public CMDHelper(string arguments, int timeoutInMilliseconds = 1000)
        {
            m_arguments             = arguments;
            m_timeoutInMilliseconds = timeoutInMilliseconds;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : CMDHelper
        // Method      : RunCommand
        // Description : This function is a wrapper for executing CMD commands
        //               NOTE: The ProcessOutput() and ProcessError() methods should be overridden accordingly.
        // Parameters  :
        // - arguments_ (string) : Input command arguments
        // Credit      : https://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results
        //               https://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
        // ----------------------------------------------------------------------------------------
        public void RunCommand()
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = @"/c " + m_arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) => {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine(); // Read synchronously
                    process.BeginErrorReadLine();  // Read asynchronously

                    if (process.WaitForExit(m_timeoutInMilliseconds) &&
                        outputWaitHandle.WaitOne(m_timeoutInMilliseconds) &&
                        errorWaitHandle.WaitOne(m_timeoutInMilliseconds))
                    {
                        // Process completed. Check process. ExitCode here.
                        if (output.Length > 0)
                        {
                            // Successful
                            ProcessOutput(output.ToString());
                        }
                        else if (error.Length > 0)
                        {
                            // Unsuccessful
                            ProcessOutput(error.ToString());
                        }
                    }
                    else
                    {
                        // Timed out.
                        ProcessTimeout();
                    }
                }
            }
        }
    }
}
