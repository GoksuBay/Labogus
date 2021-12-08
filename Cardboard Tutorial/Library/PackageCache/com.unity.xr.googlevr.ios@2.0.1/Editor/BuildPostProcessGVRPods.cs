#if UNITY_IOS
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

using Debug = UnityEngine.Debug;

namespace PostProcessGVR
{
    public class PodItem
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string Value { get; set; }

        public static PodItem CreateFromMatchGroup(Group g)
        {
            PodItem pi = new PodItem();
            pi.Index = g.Index;
            pi.Length = g.Length;
            pi.Value = g.Value;
            return pi;
        }
    };

    public class PodInfo
    {
        private static readonly int kMaxVersionParts = 3;

        public PodItem Library;
        public PodItem VersionOperator;
        public PodItem Version;

        private static short[] CrackVersion(string version)
        {
            short[] versions = {0, 0, 0};
            string[] versionDigits = version.Split('.');
            int versionIndex = 0;

            for (int i = 0; i < Math.Min(kMaxVersionParts, versionDigits.Length); i++)
            {
                versions[versionIndex] = Convert.ToInt16(versionDigits[i]);
                versionIndex++;
            }
            return versions;
        }

        public override String ToString()
        {
            string ret = String.Format("pod '{0}'", Library.Value);
            if (!String.IsNullOrEmpty(Version.Value))
            {
                string version = String.Format("{0} {1}", VersionOperator.Value, Version.Value).Trim();
                ret = String.Format("{0}, '{1}'", ret, version);
            }
            return ret;
        }

        public bool IsLowerVersionThan(PodInfo otherPod)
        {
            if (otherPod == null  || String.IsNullOrEmpty(otherPod.Version.Value)) return false;
            if (String.IsNullOrEmpty(Version.Value)) return true;

            short[] ourVersions = CrackVersion(Version.Value);
            short[] theirVersions = CrackVersion(otherPod.Version.Value);

            for (int i = 0; i < kMaxVersionParts; i++)
            {
                if (ourVersions[i] < theirVersions[i])
                    return true;
            }

            return false;
        }

        public static bool operator<(PodInfo lhs, PodInfo rhs)
        {
            return lhs != null && lhs.IsLowerVersionThan(rhs);
        }

        public static bool operator>(PodInfo lhs, PodInfo rhs)
        {
            return rhs != null && rhs.IsLowerVersionThan(lhs);
        }
    };

    public class Podfile
    {
        // The Podfile format is specified on the Cocoapods web site: http://cocoapods.org
        // When I say specified that is really giving it too much credit. The specification is more of
        // a high level overview of the major parts of the file and not a real format or spec per se.
        // As such, this regex pattern is a best effort to match pod library entries in the Podfile based
        // on the examples provided in the spec site.
        //
        private static readonly string podPattern = @"\s*pod\s+'(?<library>[^']*)'((,\s*'(?<operator>~>|>=|<=|>|)?<?\s*(?<version>\d+(\.\d+)*)+')|,\s*:\w+\s*=>\s*'[^']*')*";
        private static readonly Regex podRegex = new Regex(podPattern);


        // Return all matching pod files in the Podfile
        private void ParsePodInfos(string podFileData)
        {
            m_PodInfos = new List<PodInfo>();
            var matches = podRegex.Matches(podFileData);

            foreach (Match m in matches)
            {
                PodInfo pi = new PodInfo();
                pi.Library = PodItem.CreateFromMatchGroup(m.Groups["library"]);
                pi.VersionOperator = PodItem.CreateFromMatchGroup(m.Groups["operator"]);
                pi.Version = PodItem.CreateFromMatchGroup(m.Groups["version"]);
                m_PodInfos.Add(pi);
            }
        }

        private List<PodInfo> m_PodInfos;
        public string Path { get; private set; }

        public Podfile(string path)
        {
            Path = path;
            string poddata = File.ReadAllText(path);
            ParsePodInfos(poddata);
        }

        public PodInfo GetPodInfoForLibrary(string library)
        {
            foreach (var pi in m_PodInfos)
            {
                if (String.Compare(library, pi.Library.Value) == 0)
                    return pi;
            }
            return null;
        }
    };

    public static class PostProcessVR
    {
        private static readonly string kIOSFolder = "Packages/com.unity.xr.googlevr.ios/Plugins/iOS";
        private static readonly string kWorkspaceData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Workspace version = \"1.0\"><FileRef location = \"group:Unity-iPhone.xcodeproj\"></FileRef><FileRef location = \"group:Pods/Pods.xcodeproj\"></FileRef></Workspace>";
        private static readonly string kGoogleVRNDKLibrary = "GVRSDK/NDK"; // Initially, Google delivered just the NDK and that was what we used.
        private static readonly string kGoogleVRSDKLibrary = "GVRSDK"; // Then they pulled it all into one SDK so that is what we use now.

        [PostProcessBuildAttribute(0)]
        public static void PostProcessBuild_iOS(BuildTarget target, string pathToBuiltProject)
        {
            if (!PlayerSettings.virtualRealitySupported || target != BuildTarget.iOS)
                return;

            bool shouldCreateXcodeWorkspace = true;

            string[] virtualRealitySDKs = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.iOS);
            foreach(string sdkName in virtualRealitySDKs)
            {
                if (sdkName == "cardboard")
                {
                    // TODO: Get Path the Package vs application
                    string vrPluginsPath = kIOSFolder;
                    if (SafeToCopyCocoapods(pathToBuiltProject))
                    {
                        ModifyXcodeProjectGVR(vrPluginsPath, pathToBuiltProject);
                    }
                    else
                    {
                        GenerateCocoapodsUpdateWarning(vrPluginsPath, pathToBuiltProject);
                        shouldCreateXcodeWorkspace = false;
                    }
                }
            }

            if (shouldCreateXcodeWorkspace)
                CreateXcodeWorkspace(pathToBuiltProject);
        }

        private static bool SafeToCopyCocoapods(string buildFolder)
        {
            return !File.Exists(Path.Combine(buildFolder, "Podfile"));
        }

        public static void GenerateCocoapodsUpdateWarning(string pluginFolder, string buildFolder)
        {
            // Open Podfile
            string sourcePodfilePath = Path.Combine(pluginFolder, "GVRSDK/Podfile");
            Podfile sourcePodfile = new Podfile(sourcePodfilePath);
            // extract version
            PodInfo sourcePod = sourcePodfile.GetPodInfoForLibrary(kGoogleVRSDKLibrary);
            if (sourcePod == null)
            {
                throw new Exception("Google VR (iOS) package is missing the source Podfile data for the Google VR library!");
            }

            // Open target Podfile
            string targetPodfilePath = Path.Combine(buildFolder, "Podfile");
            Podfile targetPodfile = new Podfile(targetPodfilePath);
            // Extract version
            PodInfo targetPod = targetPodfile.GetPodInfoForLibrary(kGoogleVRSDKLibrary);
            if (targetPod == null)
            {
                targetPod = targetPodfile.GetPodInfoForLibrary(kGoogleVRNDKLibrary);
            }

            // If versions don't match, log warning
            if (targetPod == null || targetPod < sourcePod)
            {
                string msg = "The generated project already contains a Podfile and so are unable to automatically update the target Podfile.\n\n\n" +
                    "To update to the latest version of Google VR shipped with Unity, you may need to modify the Podfile (located here: " + targetPodfilePath +
                    ") in the following way:\n\n";

                if (targetPod != null)
                {
                    msg += "Remove the following from the Podfile: " + targetPod.ToString() + "\n";
                }

                msg += "Add the following to the Podfile: " + sourcePod.ToString() + "\n";
                msg += "\nFor more information on Cocoapods, including how to install and use the pod tool, please see http://cocoapods.org\n";
                Debug.LogWarning(msg);
            }
        }

        private static void CreateXcodeWorkspace(string buildFolder)
        {
            string workspacePath = Path.Combine(buildFolder, "Unity-iPhone.xcworkspace");
            if (!Directory.Exists(workspacePath))
            {
                Directory.CreateDirectory(workspacePath);
                string workspceContentFile = Path.Combine(workspacePath, "contents.xcworkspacedata");

                if (!File.Exists(workspceContentFile))
                {
                    File.WriteAllText(workspceContentFile, kWorkspaceData);
                }
            }
        }

        private static void ModifyXcodeProjectGVR(string pluginFolder, string buildFolder)
        {
            CopyCocoapodsGVR(pluginFolder, buildFolder);

            PBXProject proj = new PBXProject();
            string projPath = Path.Combine(buildFolder, "Unity-iPhone.xcodeproj/project.pbxproj");
            proj.ReadFromFile(projPath);

            string mainTargetGuid;
            string unityFrameworkTargetGuid;

            var unityMainTargetGuidMethod = proj.GetType().GetMethod("GetUnityMainTargetGuid");
            var unityFrameworkTargetGuidMethod = proj.GetType().GetMethod("GetUnityFrameworkTargetGuid");

            if (unityMainTargetGuidMethod != null && unityFrameworkTargetGuidMethod != null)
            {
                mainTargetGuid = (string)unityMainTargetGuidMethod.Invoke(proj, null);
                unityFrameworkTargetGuid = (string)unityFrameworkTargetGuidMethod.Invoke(proj, null);
            }
            else
            {
                mainTargetGuid = proj.TargetGuidByName ("Unity-iPhone");
                unityFrameworkTargetGuid = mainTargetGuid;
            }

            ModifyXcodeProjectSettingsGVR(proj, buildFolder, unityFrameworkTargetGuid);
            AddCocoapodsConfigs(proj, unityFrameworkTargetGuid);

            ModifyShellScriptPermissions(proj, unityFrameworkTargetGuid);
            AddCocoapodsShellScripts(proj, unityFrameworkTargetGuid);

            proj.WriteToFile(projPath);
        }

        private static void CopyCocoapodsGVR(string pluginFolder, string buildFolder)
        {
            FileUtil.CopyFileOrDirectory(Path.Combine(pluginFolder, "GVRSDK/Podfile"),
                Path.Combine(buildFolder, "Podfile"));
            FileUtil.CopyFileOrDirectory(Path.Combine(pluginFolder, "GVRSDK/Podfile.lock"),
                Path.Combine(buildFolder, "Podfile.lock"));
            FileUtil.CopyFileOrDirectory(Path.Combine(pluginFolder, "GVRSDK/Pods"),
                Path.Combine(buildFolder, "Pods"));
            FileUtil.CopyFileOrDirectory(Path.Combine(pluginFolder, "Source/CardboardAppController.mm"),
                Path.Combine(buildFolder, "Classes/CardboardAppController.mm"));
        }

        private static void ModifyXcodeProjectSettingsGVR(PBXProject proj, string buildFolder, string target)
        {
            string file;

            // Cardboard libs are not built with bitcode yet
            proj.SetBuildProperty(target, "ENABLE_BITCODE", "false");
            proj.AddFrameworkToProject(target, "Security.framework", true);

            file = proj.AddFile("Classes/CardboardAppController.mm",
                    "Classes/CardboardAppController.mm", PBXSourceTree.Source);
            proj.AddFileToBuild(target, file);

            file = proj.AddFile("libPods-Unity-iPhone.a", "libPods-Unity-iPhone.a", PBXSourceTree.Build);
            proj.AddFileToBuild(target, file);

            // Remove ReleaseForRunning and ReleaseForProfiling because the Pods project only supports
            // Debug and Release and we don't want to extensively modify it
            proj.RemoveBuildConfig("ReleaseForRunning");
            proj.RemoveBuildConfig("ReleaseForProfiling");
        }

        private static void AddCocoapodsConfigs(PBXProject proj, string target)
        {
            // Add CocoaPods base configs
            string releaseConfigGuid = proj.BuildConfigByName(target, "Release");
            string debugConfigGuid = proj.BuildConfigByName(target, "Debug");

            string file;
            file = proj.AddFile("Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone.release.xcconfig",
                         "Pods/Pods-Unity-iPhone.release.xcconfig", PBXSourceTree.Source);
            proj.SetBaseReferenceForConfig(releaseConfigGuid, file);

            file = proj.AddFile("Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone.debug.xcconfig",
                         "Pods/Pods-Unity-iPhone.debug.xcconfig", PBXSourceTree.Source);
            proj.SetBaseReferenceForConfig(debugConfigGuid, file);
        }

        private static void ModifyShellScriptPermissions(PBXProject proj, string target)
        {
            string resourcesScript = "chmod u+x \"${SRCROOT}/Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone-resources.sh\"\n";
            string frameworksScript = "chmod u+x \"${SRCROOT}/Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone-frameworks.sh\"\n";
            proj.AddShellScriptBuildPhase(target, "Modify Shell script Permissions", "/bin/sh", resourcesScript);
            proj.AddShellScriptBuildPhase(target, "Modify Shell script Permissions", "/bin/sh", frameworksScript);
        }

        private static void AddCocoapodsShellScripts(PBXProject proj, string target)
        {
            // setup CocoaPods shell scripts
            string script;

            // [CP] Copy Pods Resources
            script = "\"${SRCROOT}/Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone-resources.sh\"\n";
            proj.AddShellScriptBuildPhase(target, "[CP] Copy Pods Resources", "/bin/sh", script);

            // [CP] Check Pods Manifest.lock
            script = "diff \"${PODS_ROOT}/../Podfile.lock\" \"${PODS_ROOT}/Manifest.lock\" > /dev/null\n" +
                "if [ $? != 0 ] ; then\n    # print error to STDERR\n" +
                "    echo \"error: The sandbox is not in sync with the Podfile.lock. Run 'pod install' or update your CocoaPods installation.\" >&2\n" +
                "    exit 1\n" +
                "fi\n";
            proj.AddShellScriptBuildPhase(target, "[CP] Check Pods Manifest.lock", "/bin/sh", script);

            // [CP] Embed Pods Frameworks
            script = "\"${SRCROOT}/Pods/Target Support Files/Pods-Unity-iPhone/Pods-Unity-iPhone-frameworks.sh\"\n";
            proj.AddShellScriptBuildPhase(target, "[CP] Embed Pods Frameworks", "/bin/sh", script);
        }
    }
}
#endif
