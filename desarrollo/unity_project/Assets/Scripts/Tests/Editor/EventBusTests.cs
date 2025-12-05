using NUnit.Framework;
using UnityEngine;
using WebGL.Core.Events;

namespace WebGL.Tests.Editor
{
    /// <summary>
    /// Unit tests for the EventBus system.
    /// </summary>
    [TestFixture]
    public class EventBusTests
    {
        #region Test Events

        private class TestEvent
        {
            public string Message { get; }
            public TestEvent(string message) => Message = message;
        }

        private class AnotherTestEvent
        {
            public int Value { get; }
            public AnotherTestEvent(int value) => Value = value;
        }

        #endregion

        #region Setup / Teardown

        [SetUp]
        public void Setup()
        {
            // Clear all subscribers before each test
            EventBus.ClearAllSubscribers();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus.ClearAllSubscribers();
        }

        #endregion

        #region Subscribe Tests

        [Test]
        public void Subscribe_AddsSubscriber()
        {
            // Arrange
            int callCount = 0;
            void Handler(TestEvent e) => callCount++;

            // Act
            EventBus.Subscribe<TestEvent>(Handler);
            var count = EventBus.GetSubscriberCount<TestEvent>();

            // Assert
            Assert.AreEqual(1, count, "Should have exactly one subscriber");
        }

        [Test]
        public void Subscribe_PreventsDuplicates()
        {
            // Arrange
            void Handler(TestEvent e) { }

            // Act
            EventBus.Subscribe<TestEvent>(Handler);
            EventBus.Subscribe<TestEvent>(Handler); // Duplicate

            // Assert
            Assert.AreEqual(1, EventBus.GetSubscriberCount<TestEvent>(), 
                "Should not add duplicate subscribers");
        }

        [Test]
        public void Subscribe_NullCallback_DoesNotCrash()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => EventBus.Subscribe<TestEvent>(null));
        }

        #endregion

        #region Unsubscribe Tests

        [Test]
        public void Unsubscribe_RemovesSubscriber()
        {
            // Arrange
            void Handler(TestEvent e) { }
            EventBus.Subscribe<TestEvent>(Handler);

            // Act
            EventBus.Unsubscribe<TestEvent>(Handler);

            // Assert
            Assert.AreEqual(0, EventBus.GetSubscriberCount<TestEvent>());
        }

        [Test]
        public void Unsubscribe_NonExistentHandler_DoesNotCrash()
        {
            // Arrange
            void Handler(TestEvent e) { }

            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => EventBus.Unsubscribe<TestEvent>(Handler));
        }

        #endregion

        #region Publish Tests

        [Test]
        public void Publish_InvokesSubscriber()
        {
            // Arrange
            string receivedMessage = null;
            EventBus.Subscribe<TestEvent>(e => receivedMessage = e.Message);

            // Act
            EventBus.Publish(new TestEvent("Hello, World!"));

            // Assert
            Assert.AreEqual("Hello, World!", receivedMessage);
        }

        [Test]
        public void Publish_InvokesMultipleSubscribers()
        {
            // Arrange
            int callCount = 0;
            EventBus.Subscribe<TestEvent>(e => callCount++);
            EventBus.Subscribe<TestEvent>(e => callCount++);

            // Act
            EventBus.Publish(new TestEvent("Test"));

            // Assert
            Assert.AreEqual(2, callCount, "Both subscribers should be invoked");
        }

        [Test]
        public void Publish_DifferentEventTypes_DoNotInterfere()
        {
            // Arrange
            string testMessage = null;
            int anotherValue = 0;

            EventBus.Subscribe<TestEvent>(e => testMessage = e.Message);
            EventBus.Subscribe<AnotherTestEvent>(e => anotherValue = e.Value);

            // Act
            EventBus.Publish(new TestEvent("Test"));
            EventBus.Publish(new AnotherTestEvent(42));

            // Assert
            Assert.AreEqual("Test", testMessage);
            Assert.AreEqual(42, anotherValue);
        }

        [Test]
        public void Publish_NoSubscribers_DoesNotCrash()
        {
            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => EventBus.Publish(new TestEvent("Test")));
        }

        #endregion

        #region Clear Tests

        [Test]
        public void ClearSubscribers_RemovesAllForType()
        {
            // Arrange
            EventBus.Subscribe<TestEvent>(e => { });
            EventBus.Subscribe<TestEvent>(e => { });

            // Act
            EventBus.ClearSubscribers<TestEvent>();

            // Assert
            Assert.AreEqual(0, EventBus.GetSubscriberCount<TestEvent>());
        }

        [Test]
        public void ClearAllSubscribers_RemovesEverything()
        {
            // Arrange
            EventBus.Subscribe<TestEvent>(e => { });
            EventBus.Subscribe<AnotherTestEvent>(e => { });

            // Act
            EventBus.ClearAllSubscribers();

            // Assert
            Assert.AreEqual(0, EventBus.GetSubscriberCount<TestEvent>());
            Assert.AreEqual(0, EventBus.GetSubscriberCount<AnotherTestEvent>());
        }

        #endregion
    }
}
