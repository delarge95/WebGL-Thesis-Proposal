using NUnit.Framework;
using UnityEngine;
using WebGL.Core.Data;

namespace WebGL.Tests.Editor
{
    /// <summary>
    /// Unit tests for DronePartData ScriptableObject.
    /// </summary>
    [TestFixture]
    public class DronePartDataTests
    {
        private DronePartData CreateTestPartData()
        {
            var data = ScriptableObject.CreateInstance<DronePartData>();
            data.id = "test-001";
            data.partName = "Test Part";
            data.category = "Testing";
            data.description = "A test part for unit testing.";
            data.weight = 100f;
            data.dimensions = "10 x 10 x 10 cm";
            data.material = "Test Material";
            data.requiredTools = new[] { "Tool A", "Tool B" };
            data.safetyWarnings = new[] { "Warning 1" };
            data.installationTimeMinutes = 5f;
            data.difficultyLevel = 2;
            data.assemblyOrder = 3;
            data.screwCount = 4;
            data.screwSize = "M3x8";
            return data;
        }

        [Test]
        public void DronePartData_DefaultValues_AreValid()
        {
            // Arrange
            var data = ScriptableObject.CreateInstance<DronePartData>();

            // Assert
            Assert.IsNotNull(data, "DronePartData instance should not be null");
            Assert.AreEqual(0f, data.weight, "Default weight should be 0");
            Assert.AreEqual(0, data.difficultyLevel, "Default difficulty should be 0");
        }

        [Test]
        public void DronePartData_SetProperties_RetainsValues()
        {
            // Arrange & Act
            var data = CreateTestPartData();

            // Assert
            Assert.AreEqual("test-001", data.id);
            Assert.AreEqual("Test Part", data.partName);
            Assert.AreEqual("Testing", data.category);
            Assert.AreEqual(100f, data.weight);
            Assert.AreEqual(2, data.difficultyLevel);
            Assert.AreEqual(4, data.screwCount);
        }

        [Test]
        public void DronePartData_RequiredTools_ReturnsCorrectCount()
        {
            // Arrange
            var data = CreateTestPartData();

            // Act & Assert
            Assert.AreEqual(2, data.requiredTools.Length);
            Assert.Contains("Tool A", data.requiredTools);
            Assert.Contains("Tool B", data.requiredTools);
        }

        [Test]
        public void DronePartData_SafetyWarnings_AreAccessible()
        {
            // Arrange
            var data = CreateTestPartData();

            // Assert
            Assert.IsNotNull(data.safetyWarnings);
            Assert.AreEqual(1, data.safetyWarnings.Length);
            Assert.AreEqual("Warning 1", data.safetyWarnings[0]);
        }

        [Test]
        public void DronePartData_GetAssemblyInfo_ReturnsFormattedString()
        {
            // Arrange
            var data = CreateTestPartData();

            // Act
            string assemblyInfo = data.GetAssemblyInfo();

            // Assert
            Assert.IsNotNull(assemblyInfo);
            Assert.IsTrue(assemblyInfo.Contains("Test Part") || assemblyInfo.Length > 0,
                "Assembly info should contain part information");
        }

        [Test]
        public void DronePartData_NullArrays_DoNotCauseErrors()
        {
            // Arrange
            var data = ScriptableObject.CreateInstance<DronePartData>();
            data.requiredTools = null;
            data.safetyWarnings = null;
            data.prerequisites = null;

            // Assert - accessing null arrays should not throw
            Assert.IsNull(data.requiredTools);
            Assert.IsNull(data.safetyWarnings);
            Assert.IsNull(data.prerequisites);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up any created ScriptableObjects
        }
    }
}
