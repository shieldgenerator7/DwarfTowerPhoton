using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using UnityEditor.SceneManagement;
using System;
using System.Linq;

public class CustomMenu
{
    //Find Missing Scripts
    //2018-04-13: copied from http://wiki.unity3d.com/index.php?title=FindMissingScripts
    static int go_count = 0, components_count = 0, missing_count = 0;
    [MenuItem("SG7/Editor/Refactor/Find Missing Scripts")]
    private static void FindMissingScripts()
    {
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.isLoaded)
            {
                foreach (GameObject go in s.GetRootGameObjects())
                {
                    FindInGO(go);
                }
            }
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }
    private static void FindInGO(GameObject go)
    {
        go_count++;
        Component[] components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = go.name;
                Transform t = go.transform;
                while (t.parent != null)
                {
                    s = $"{t.parent.name}/{s}";
                    t = t.parent;
                }
                Debug.Log(
                    $"{s} has an empty script attached in position: {i}",
                    go
                    );
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in go.transform)
        {
            FindInGO(childT.gameObject);
        }
    }

    [MenuItem("SG7/Editor/Show or Hide All Colliders %&c")]
    public static void showHideAllColliders()
    {
        Physics2D.alwaysShowColliders = !Physics2D.alwaysShowColliders;
    }

    [MenuItem("SG7/Build/Build Windows %w")]
    public static void buildWindows()
    {
        build(BuildTarget.StandaloneWindows, "exe");
    }
    //[MenuItem("SG7/Build/Build Linux %l")]
    //public static void buildLinux()
    //{
    //    build(BuildTarget.StandaloneLinux, "x86");
    //}
    //[MenuItem("SG7/Build/Build Mac OS X %#l")]
    //public static void buildMacOSX()
    //{
    //    build(BuildTarget.StandaloneOSX, "");
    //}
    public static void build(BuildTarget buildTarget, string extension, bool openDialog = true)
    {
        string defaultPath = getDefaultBuildPath();
        if (!System.IO.Directory.Exists(defaultPath))
        {
            System.IO.Directory.CreateDirectory(defaultPath);
        }
        //2017-10-19 copied from https://docs.unity3d.com/Manual/BuildPlayerPipeline.html
        // Get filename.
        string buildName = getBuildNamePath(extension);
        if (openDialog)
        {
            buildName = EditorUtility.SaveFilePanel(
                "Choose Location of Built Game",
                defaultPath,
                PlayerSettings.productName,
                extension
                );
        }
        // User hit the cancel button.
        if (buildName == "")
            return;

        string path = buildName.Substring(0, buildName.LastIndexOf("/"));
        Debug.Log($"BUILDNAME: {buildName}");
        Debug.Log($"PATH: {path}");

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

        //Kill running builds if any
        killProcesses();

        // Build player.
        BuildPipeline.BuildPlayer(levels, buildName, buildTarget, BuildOptions.None);
    }

    [MenuItem("SG7/Test/Test Windows %t")]
    public static void testWindows()
    {
        runWindows();
        GameLauncherSettings gls = GameObject.FindObjectOfType<GameLauncherSettings>();
        if (gls)
        {
            if (gls.enterPlayMode)
            {
                EditorApplication.EnterPlaymode();
            }
        }
    }

    [MenuItem("SG7/Test/Build and Test Windows %#t")]
    public static void buildAndtestWindows()
    {
        killProcesses();
        build(BuildTarget.StandaloneWindows, "exe", false);
        testWindows();
    }

    [MenuItem("SG7/Test/Kill Processes &t")]
    public static void killProcesses()
    {
        GameLauncherSettings gls = GameObject.FindObjectOfType<GameLauncherSettings>();
        if (gls)
        {
            if (gls.enterPlayMode && EditorApplication.isPlaying)
            {
                EditorApplication.ExitPlaymode();
            }
        }
        string gameProcName = PlayerSettings.productName;
        List<Process> gameProcList = Process.GetProcesses().ToList().FindAll(
            proc => !proc.HasExited && proc.ProcessName == gameProcName
            );
        Debug.Log($"Kill Processes: killing {gameProcList.Count} {gameProcName} processes");
        foreach (Process proc in gameProcList)
        {
            if (!proc.HasExited)
            {
                if (proc.ProcessName == gameProcName)
                {
                    proc.Kill();
                }
                else
                {
                    Debug.LogWarning($"Can't kill process {proc.ProcessName} ({proc.Id})" +
                        $" because it is not a process of {gameProcName}");
                }
            }
            else
            {
                Debug.LogWarning($"Process ({proc.Id}) has already exited");
            }
        }
    }

    [MenuItem("SG7/Run/Run Windows %#w")]
    public static void runWindows()
    {//2018-08-10: copied from build()
        int windowCount = 1;
        GameLauncherSettings gls = GameObject.FindObjectOfType<GameLauncherSettings>();
        if (gls)
        {
            windowCount = gls.clientCount;
            if (gls.resetClients)
            {
                killProcesses();
            }
        }
        else
        {
            Debug.LogWarning(
                "No GameLauncherSettings object found!" +
                " Make a new one or open up the Tools scene to make this tool more useful."
                );
        }
        string extension = "exe";
        string buildName = getBuildNamePath(extension);
        Debug.Log($"Launching: {buildName}");
        for (int i = 0; i < windowCount; i++)
        {
            // Run the game (Process class from System.Diagnostics).
            Process proc = new Process();
            proc.StartInfo.FileName = buildName;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
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
        return $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}" +
            $"/Unity/DwarfTowerPhoton/Builds/" +
            $"{PlayerSettings.productName}_{PlayerSettings.bundleVersion.Replace(".", "_")}";
    }
    public static string getBuildNamePath(string extension, bool checkFolderExists = true)
    {
        string defaultPath = getDefaultBuildPath();
        if (checkFolderExists && !System.IO.Directory.Exists(defaultPath))
        {
            throw new UnityException(
                $"You need to build the {extension} for {PlayerSettings.productName} " +
                $"(Version {PlayerSettings.bundleVersion}) first!");
        }
        string buildName = $"{defaultPath}/{PlayerSettings.productName}.{extension}";
        return buildName;
    }

    [MenuItem("SG7/Session/Begin Session")]
    public static void beginSession()
    {
        Debug.Log("=== Beginning session ===");
        string oldVersion = PlayerSettings.bundleVersion;
        string[] split = oldVersion.Split('.');
        int versionNumber = int.Parse(split[1]) + 1;
        string newVersion = split[0] + "."
            + ((versionNumber < 100) ? "0" : "")
            + versionNumber;
        PlayerSettings.bundleVersion = newVersion;
        //Save and Log
        EditorSceneManager.SaveOpenScenes();
        Debug.LogWarning($"Updated build version number from {oldVersion} to {newVersion}");
    }

    [MenuItem("SG7/Session/Finish Session")]
    public static void finishSession()
    {
        Debug.Log("=== Finishing session ===");
        EditorSceneManager.SaveOpenScenes();
        buildWindows();
        //Open folders
        openBuildFolder();
    }

    [MenuItem("SG7/Upgrade/Force save all assets")]
    public static void forceSaveAllAssets()
    {
        AssetDatabase.ForceReserializeAssets();
    }
}
