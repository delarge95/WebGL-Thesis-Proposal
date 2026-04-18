using System.IO;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WebGL.Core.Content;
using WebGL.Core.Managers;

namespace WebGL.Tests.Editor
{
    [TestFixture]
    public class MigrationSmokeTests
    {
        private const string MainScenePath = "Assets/Scenes/MainScene_Final.unity";

        private string _originalScenePath;

        private static string ProjectRoot => Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        private static string ReportsRoot => Path.Combine(ProjectRoot, "Reports", "MigrationMetrics");

        [SetUp]
        public void Setup()
        {
            _originalScenePath = SceneManager.GetActiveScene().path;
        }

        [TearDown]
        public void TearDown()
        {
            if (!string.IsNullOrWhiteSpace(_originalScenePath) && File.Exists(Path.Combine(ProjectRoot, _originalScenePath)))
            {
                EditorSceneManager.OpenScene(_originalScenePath, OpenSceneMode.Single);
            }
        }

        [Test]
        public void ProjectVersion_TargetsUnity643f1()
        {
            string projectVersionPath = Path.Combine(ProjectRoot, "ProjectSettings", "ProjectVersion.txt");
            string projectVersionText = File.ReadAllText(projectVersionPath);

            StringAssert.Contains("m_EditorVersion: 6000.4.3f1", projectVersionText);
            StringAssert.Contains("m_EditorVersionWithRevision: 6000.4.3f1 (39d1a88d4dd1)", projectVersionText);
        }

        [Test]
        public void BuildSettings_UsesMainSceneFinal()
        {
            bool foundEnabledMainScene = false;
            foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
            {
                if (scene.enabled && scene.path == MainScenePath)
                {
                    foundEnabledMainScene = true;
                    break;
                }
            }

            Assert.IsTrue(foundEnabledMainScene, "MainScene_Final.unity must stay enabled in Build Settings.");
        }

        [Test]
        public void MainSceneFinal_ContainsCriticalRuntimeSystems()
        {
            EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

            Assert.IsNotNull(Camera.main ?? FindSceneObject<Camera>(), "Main scene must provide a main camera.");
            Assert.IsNotNull(FindSceneObject<OrbitCameraController>(), "Main scene must contain OrbitCameraController.");
            Assert.IsNotNull(FindSceneObject<SelectionManager>(), "Main scene must contain SelectionManager.");
            Assert.IsNotNull(FindSceneObject<ViewModeManager>(), "Main scene must contain ViewModeManager.");
            Assert.IsNotNull(FindSceneObject<CrossSectionManager>(), "Main scene must contain CrossSectionManager.");
            Assert.IsNotNull(FindSceneObject<ExplodedViewManager>(), "Main scene must contain ExplodedViewManager.");
            Assert.IsNotNull(FindSceneObject<UIDocument>(), "Main scene must contain a UIDocument.");
        }

        [Test]
        public void CriticalAssets_ExistForWebMigration()
        {
            AssertFileExists("Assets/Settings/URP_WebGL.asset");
            AssertFileExists("Assets/UI/MainPanelSettings.asset");
            AssertFileExists("Assets/Shaders/Thermal.shader");
            AssertFileExists("Assets/Scripts/Core/Utils/MigrationBenchmarkRunner.cs");
            AssertFileExists("Assets/Editor/Antigravity/Fixes/MigrationMetricsReporter.cs");
        }

        [Test]
        public void MigrationMetricArtifacts_Exist()
        {
            string[] expectedFiles =
            {
                "README.md",
                "baseline_6_0.md",
                "baseline_6_4_sin_lod.md",
                "wave_0_piloto.md",
                "wave_1.md",
                "wave_2.md",
                "wave_3.md"
            };

            for (int i = 0; i < expectedFiles.Length; i++)
            {
                string fullPath = Path.Combine(ReportsRoot, expectedFiles[i]);
                Assert.IsTrue(File.Exists(fullPath), $"Expected migration artifact was not found: {fullPath}");
            }
        }

        [Test]
        public void ProjectSettings_WebFlagsMatchMigrationPlan()
        {
            string projectSettingsPath = Path.Combine(ProjectRoot, "ProjectSettings", "ProjectSettings.asset");
            string projectSettingsText = File.ReadAllText(projectSettingsPath);

            StringAssert.Contains("webGLLinkerTarget: 1", projectSettingsText);
            StringAssert.Contains("webGLThreadsSupport: 0", projectSettingsText);
            StringAssert.Contains("webGLEnableWebGPU: 0", projectSettingsText);
            StringAssert.Contains("webGLDataCaching: 1", projectSettingsText);
        }

        private static void AssertFileExists(string relativePath)
        {
            string fullPath = Path.Combine(ProjectRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Assert.IsTrue(File.Exists(fullPath), $"Expected file was not found: {relativePath}");
        }

        private static T FindSceneObject<T>() where T : Object
        {
            T[] candidates = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < candidates.Length; i++)
            {
                if (candidates[i] is Component component && component.gameObject.scene.path == MainScenePath)
                {
                    return candidates[i];
                }
            }

            return null;
        }
    }
}
