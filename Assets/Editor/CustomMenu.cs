using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.SceneManagement;

public class CustomMenu
{
    private static List<Process> buildProcesses = new List<Process>();

    [MenuItem("SG7/Build/Build Windows %w")]
    public static void buildWindows()
    {
        build(BuildTarget.StandaloneWindows, "exe");
    }
    [MenuItem("SG7/Build/Build Linux %l")]
    public static void buildLinux()
    {
        build(BuildTarget.StandaloneLinux, "x86");
    }
    [MenuItem("SG7/Build/Build Mac OS X %#l")]
    public static void buildMacOSX()
    {
        build(BuildTarget.StandaloneOSX, "");
    }
    public static void build(BuildTarget buildTarget, string extension)
    {
        string defaultPath = getDefaultBuildPath();
        if (!System.IO.Directory.Exists(defaultPath))
        {
            System.IO.Directory.CreateDirectory(defaultPath);
        }
        //2017-10-19 copied from https://docs.unity3d.com/Manual/BuildPlayerPipeline.html
        // Get filename.
        string buildName = EditorUtility.SaveFilePanel("Choose Location of Built Game", defaultPath, PlayerSettings.productName, extension);

        // User hit the cancel button.
        if (buildName == "")
            return;

        string path = buildName.Substring(0, buildName.LastIndexOf("/"));
        UnityEngine.Debug.Log("BUILDNAME: " + buildName);
        UnityEngine.Debug.Log("PATH: " + path);

        string[] levels = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            if (EditorBuildSettings.scenes[i].enabled)
            {
                levels[i] = EditorBuildSettings.scenes[i].path;
            }
            else
            {
                break;
            }
        }

        // Build player.
        BuildPipeline.BuildPlayer(levels, buildName, buildTarget, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        string resourcesPath = path + "/Assets/Resources";

        if (buildTarget == BuildTarget.StandaloneWindows)
        {
            killProcesses();
            runWindows();
        }
    }

    public static void killProcesses()
    {
        foreach(Process proc in buildProcesses)
        {
            proc.CloseMainWindow();
            proc.Close();
        }
        buildProcesses.Clear();
    }

    [MenuItem("SG7/Run/Run Windows %#w")]
    public static void runWindows()
    {//2018-08-10: copied from build()
        int windowCount = 1;
        GameLauncherSettings gls = GameObject.FindObjectOfType<GameLauncherSettings>();
        if (gls)
        {
            windowCount = gls.clientCount;
        }
        string extension = "exe";
        string buildName = getBuildNamePath(extension);
        UnityEngine.Debug.Log("Launching: " + buildName);
        for (int i = 0; i < windowCount; i++)
        {
            // Run the game (Process class from System.Diagnostics).
            Process proc = new Process();
            buildProcesses.Add(proc);
            proc.StartInfo.FileName = buildName;
            proc.Start();
        }
    }

    [MenuItem("SG7/Run/Open Build Folder #w")]
    public static void openBuildFolder()
    {
        string extension = "exe";
        string buildName = getBuildNamePath(extension);
        //Open the folder where the game is located
        EditorUtility.RevealInFinder(buildName);
    }

    public static string getDefaultBuildPath()
    {
        return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Unity/DwarfTowerPhoton/Builds/" + PlayerSettings.productName + "_" + PlayerSettings.bundleVersion.Replace(".", "_");
    }
    public static string getBuildNamePath(string extension, bool checkFolderExists = true)
    {
        string defaultPath = getDefaultBuildPath();
        if (checkFolderExists && !System.IO.Directory.Exists(defaultPath))
        {
            throw new UnityException("You need to build the " + extension + " for " + PlayerSettings.productName + " (Version " + PlayerSettings.bundleVersion + ") first!");
        }
        string buildName = defaultPath + "/" + PlayerSettings.productName + "." + extension;
        return buildName;
    }
}
