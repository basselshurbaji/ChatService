﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Aub.Eece503e.ChatService.Datacontracts;
using Aub.Eece503e.ChatService.Web.Controllers;
using Aub.Eece503e.ChatService.Web.Store;
using Aub.Eece503e.ChatService.Web.Store.Exceptions;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.ApplicationInsights;
using Aub.Eece503e.ChatService.Web;

namespace Aub.Eece503e.ChatService.Tests
{
    public class ConversationsControllerTests
    {
        // We have three api calls
        // Need 6 tests

        private Message _testMessage = new Message
        {
            Id = "001",
            Text = "RandomMessage",
            SenderUsername = "JohnSmith"
        };

        private MessageWithUnixTime _testMessageWithUnixTime = new MessageWithUnixTime
        {
            Id = "001",
            Text = "RandomMessage",
            SenderUsername = "JohnSmith",
            UnixTime = 10
        };

        private Conversation _testConversation = new Conversation
        {
            Id = "001",
            LastModifiedUnixTime = 000001,
            Recepient = new Profile { Username = "Joe", Firstname = "Bryan", Lastname = "Davis" , ProfilePictureId = "002" }
        };

        private string _testContinuationToken = "0001";
        private int _testLimit = 10;

        [Fact]
        public async Task GetMessageReturns503WhenStorageIsDown()
        {
            var messageStoreMock = new Mock<IMessageStore>();
            messageStoreMock.Setup(store => store.GetMessage(_testConversation.Id, _testMessage.Id)).ThrowsAsync(new StorageErrorException());

            var loggerStub = new ConversationsControllerLoggerStub();
            var controller = new ConversationsController(messageStoreMock.Object, loggerStub, new TelemetryClient());
            IActionResult result = await controller.GetMessage(_testConversation.Id, _testMessage.Id);

            AssertUtils.HasStatusCode(HttpStatusCode.ServiceUnavailable, result);
            Assert.Contains(LogLevel.Error, loggerStub.LogEntries.Select(entry => entry.Level));
        }

        [Fact]
        public async Task GetMessageReturns500WhenExceptionIsNotKnown()
        {
            var messageStoreMock = new Mock<IMessageStore>();
            messageStoreMock.Setup(store => store.GetMessage(_testConversation.Id, _testMessage.Id)).ThrowsAsync(new Exception("Test Exception"));

            var loggerStub = new ConversationsControllerLoggerStub();
            var controller = new ConversationsController(messageStoreMock.Object, loggerStub, new TelemetryClient());
            IActionResult result = await controller.GetMessage(_testConversation.Id, _testMessage.Id);

            AssertUtils.HasStatusCode(HttpStatusCode.InternalServerError, result);
            Assert.Contains(LogLevel.Error, loggerStub.LogEntries.Select(entry => entry.Level));
        }

        [Fact]
        public async Task GetMessageListReturns503WhenStorageIsDown()
        {
            var messageStoreMock = new Mock<IMessageStore>();
            messageStoreMock.Setup(store => store.GetMessages(_testConversation.Id, _testContinuationToken, _testLimit)).ThrowsAsync(new StorageErrorException());

            var loggerStub = new ConversationsControllerLoggerStub();
            var controller = new ConversationsController(messageStoreMock.Object, loggerStub, new TelemetryClient());
            IActionResult result = await controller.GetMessageList(_testConversation.Id, _testContinuationToken, _testLimit);

            AssertUtils.HasStatusCode(HttpStatusCode.ServiceUnavailable, result);
            Assert.Contains(LogLevel.Error, loggerStub.LogEntries.Select(entry => entry.Level));
        }

        [Fact]
        public async Task GetMessageListReturns500WhenExceptionIsNotKnown()
        {
            var messageStoreMock = new Mock<IMessageStore>();
            messageStoreMock.Setup(store => store.GetMessages(_testConversation.Id, _testContinuationToken, _testLimit)).ThrowsAsync(new Exception("Test Exception"));

            var loggerStub = new ConversationsControllerLoggerStub();
            var controller = new ConversationsController(messageStoreMock.Object, loggerStub, new TelemetryClient());
            IActionResult result = await controller.GetMessageList(_testConversation.Id, _testContinuationToken, _testLimit);

            AssertUtils.HasStatusCode(HttpStatusCode.InternalServerError, result);
            Assert.Contains(LogLevel.Error, loggerStub.LogEntries.Select(entry => entry.Level));
        }

        [Fact]
        public async Task PostMessageReturns503WhenStorageIsDown()
        {
            var messageStoreMock = new Mock<IMessageStore>();
            messageStoreMock.Setup(store => store.AddMessage(_testMessageWithUnixTime, _testConversation.Id)).ThrowsAsync(new StorageErrorException());

            var loggerStub = new ConversationsControllerLoggerStub();
            var controller = new ConversationsController(messageStoreMock.Object, loggerStub, new TelemetryClient());
            IActionResult result = await controller.PostMessage(_testConversation.Id, _testMessage);

            AssertUtils.HasStatusCode(HttpStatusCode.ServiceUnavailable, result);
            Assert.Contains(LogLevel.Error, loggerStub.LogEntries.Select(entry => entry.Level));
        }

        [Fact]
        public async Task PostMessageReturns500WhenExceptionIsNotKnown()
        {
            var messageStoreMock = new Mock<IMessageStore>();
            messageStoreMock.Setup(store => store.AddMessage(_testMessageWithUnixTime, _testConversation.Id)).ThrowsAsync(new Exception("Test Exception"));

            var loggerStub = new ConversationsControllerLoggerStub();
            var controller = new ConversationsController(messageStoreMock.Object, loggerStub, new TelemetryClient());
            IActionResult result = await controller.PostMessage(_testConversation.Id, _testMessage);

            AssertUtils.HasStatusCode(HttpStatusCode.InternalServerError, result);
            Assert.Contains(LogLevel.Error, loggerStub.LogEntries.Select(entry => entry.Level));
        }
    }

    }
