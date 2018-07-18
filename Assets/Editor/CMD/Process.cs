using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CMD
{
public class Process
{
    public static void ProcessCommand(string command, string argument)
    {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
        info.Arguments = argument;
        info.CreateNoWindow = false;
        info.ErrorDialog = true;
        info.UseShellExecute = true;

        if (info.UseShellExecute)
        {
            info.RedirectStandardOutput = false;
            info.RedirectStandardError = false;
            info.RedirectStandardInput = false;
        }
        else
        {
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }
        

        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

        //System.Diagnostics.Process process = System.Diagnostics.Process.Start(command , argument);

        //process.StandardInput.WriteLine("pause");

        if (!info.UseShellExecute)
        {
            Debug.Log(process.StandardOutput.ReadToEnd());
            Debug.Log(process.StandardError.ReadToEnd());
        }

        //process.WaitForInputIdle();

        process.WaitForExit();
        process.Close();
    }
}
}
