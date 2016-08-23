using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
//using System.ServiceModel;

//
// Create Eventlog using PowerShell:
// $EVENT_SOURCE = 'EduserWindowsAgentWcf'
// $EVENT_LOG = 'EduserWindowsAgent'
// New-EventLog -LogName Application -Source $EVENT_SOURCE
// New-EventLog -LogName Application -Source $EVENT_LOG
//

namespace StringTest
{
    class Program
    {
        private const string COMMANDS_PS_SNAP_IN = "IST.EduserWindowsAgent.Commands";
        private const string EVENT_SOURCE = "EduserWindowsAgentWcf";
        private const string EVENT_LOG = "EduserWindowsAgent";

        static void Main(string[] args)
        {
            string username, firstname, lastname;
            int uid;
            string domain, password, type, workgroup;

            username = "bkirk";
            firstname = "Brian";
            lastname = "Kirkegaard";
            uid = 147;
            domain = "daimos.net";
            type = "Student";
            workgroup = "";

            Console.Write("Please enter a password:");
            password = Console.ReadLine();

            string res = AddISTUser(username,firstname,lastname,uid,domain,password,type,workgroup);
            //string res = Test01(password);

            Console.WriteLine(res);
            Console.ReadKey();
        }

        static string Test01(string pwd)
        {
            Console.WriteLine("Hello" + pwd);
            return "Hello2" + pwd;
        }

        static string AddISTUser(string username, string firstname, string lastname, int uid, string domain, string password, string type, string workgroup)
        {
            //string script = "Add-ISTUser " + username + " \"" + firstname + "\" \"" + lastname + "\" " + uid + " \"" + domain + "\" \"" + password + "\" " + type + " \"" + workgroup + "\"";
            string script = "New-User " + username + " \"" + firstname + "\" \"" + lastname + "\" " + uid + " \"" + domain + "\" \"" + password + "\" " + type + " \"" + workgroup + "\"";
            //string script = "\"Hello\" | Out-File \"c:\\_tmp\\test.txt\"" ;

            Console.WriteLine(script);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return ExecuteScript(script, "User is created", "Error occured creating user: ", 0);
        }

        static string ExecuteScript(string script, string successText, string failText, int logVerbosity)
        {
            return ExecuteScript(script, successText, failText, COMMANDS_PS_SNAP_IN, logVerbosity, false);
        }
        static string ExecuteScript(string script, string successText, string failText, string snapInName, int logVerbosity, bool returnLastCommandResult)
        {
            try
            {
                LogMessage("Running script: " + script, EventLogEntryType.Information, 5);
                RunspaceConfiguration runspaceConfiguration = GetRunspaceConfiguration(snapInName);
                Runspace psRunner = GetPsRunner(runspaceConfiguration);
                try
                {
                    using (Pipeline pipeline = GetPipeline(psRunner))
                    {
                        pipeline.Commands.AddScript("Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope CurrentUser");
                        pipeline.Commands.AddScript("Import-Module -Name 'ISTCmdlets'");
                        Collection<PSObject> commandResults = InvokeScript(script, pipeline);
                        if (returnLastCommandResult)
                        {
                            string result = CheckLastCommandResult(commandResults);
                            if (!string.IsNullOrEmpty(result)) return result;
                        }
                        else
                        {
                            CheckCommandResult(commandResults);
                        }
                        CheckPipelineState(pipeline, failText);
                        LogMessage(successText, EventLogEntryType.Information, logVerbosity);
                        return string.Empty;
                    }
                }
                finally
                {
                    psRunner.Close();
                    // Does not work. It does not implement the IDisposable interface.
                    //((IDisposable)runspaceConfiguration).Dispose();
                }
            }
            catch (EduserException e)
            {
                LogMessage("Caught exception: " + e.Message, EventLogEntryType.Warning, logVerbosity);
                return e.Message;
            }
            catch (Exception e)
            {
                LogMessage("Error occured [11]: " + e.Message, EventLogEntryType.Error, logVerbosity);
                return "Error occured [11]: " + e.Message;
            }
        }

        static void LogMessage(string message, EventLogEntryType logLevel, int logVerbosity)
        {
            // > var oprindeligt < hvad har Michael tænkte på?
//            if (logLevel == EventLogEntryType.Information && logVerbosity > 3) return;
//            if (logLevel == EventLogEntryType.Warning && logVerbosity > 2) return;
//            if (logLevel == EventLogEntryType.Error && logVerbosity > 1) return;

            if (!EventLog.SourceExists(EVENT_LOG)) EventLog.CreateEventSource(EVENT_LOG, EVENT_SOURCE);
            EventLog.WriteEntry(EVENT_LOG, message, logLevel);
        }

        private class EduserException : Exception
        {
            public EduserException(string msg) : base(msg) { }
        }

        static Collection<PSObject> InvokeScript(string script, Pipeline pipeline)
        {
            pipeline.Commands.AddScript(script);
            return pipeline.Invoke();
        }

        static RunspaceConfiguration GetRunspaceConfiguration(string snapInName)
        {
            RunspaceConfiguration runspaceConfiguration = null;
            try
            {
                runspaceConfiguration = RunspaceConfiguration.Create();
//                PSSnapInException nonFatalWarning;
//                runspaceConfiguration.AddPSSnapIn(snapInName, out nonFatalWarning);
//                if (nonFatalWarning != null) throw nonFatalWarning;
                return runspaceConfiguration;
            }
            catch (Exception e)
            {
                if (runspaceConfiguration != null) ((IDisposable)runspaceConfiguration).Dispose();
                throw new EduserException("Failed to load " + snapInName + " snapin. Error was: " + e.Message);
            }
        }

        static Runspace GetPsRunner(RunspaceConfiguration runspaceConfiguration)
        {
            Runspace psRunner = null;
            try
            {
                psRunner = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                psRunner.Open();
                return psRunner;
            }
            catch (Exception e)
            {
                if (psRunner != null) ((IDisposable)psRunner).Dispose();
                throw new EduserException("Failed to initiate Runspace. Error was: " + e.Message);
            }
        }

        static Pipeline GetPipeline(Runspace psRunner)
        {
            try
            {
                return psRunner.CreatePipeline();
            }
            catch (Exception e)
            {
                throw new EduserException("Failed to initiate Pipeline. Error was: " + e.Message);
            }
        }

        static void CheckCommandResult(Collection<PSObject> commandResults)
        {
            if (commandResults == null) return;

            string result = string.Empty;
            foreach (PSObject pso in commandResults)
            {
                result += string.IsNullOrEmpty(result) ? pso.ToString() : ", " + pso.ToString();
            }
            if (!string.IsNullOrEmpty(result)) throw new EduserException(result);
        }

        static string CheckLastCommandResult(Collection<PSObject> commandResults)
        {
            string result = "";
            foreach (PSObject pso in commandResults)
            {
                result = pso.ToString();
            }
            return result;
        }

        static void CheckPipelineState(Pipeline pipeline, string failText)
        {
            PipelineState pipelineState = pipeline.PipelineStateInfo.State;
            if (pipelineState != PipelineState.Completed)
            {
                throw new EduserException(failText + pipelineState);
            }
        }

    }
}
