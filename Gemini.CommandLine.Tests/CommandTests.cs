﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gemini.CommandLine.Tests
{
    [TestClass]
    public class CommandTests
    {
        // ReSharper disable UnusedMember.Local
        private class ExampleCommandType
        {
            public static bool ExampleCommandRan;
            public static bool ExampleCommandWithOptionsRan;
            public static bool StaticCommandRan;
            public static bool StaticCommandWithOptionsRan;
            public static bool CommandWithNameRan;
            public static bool TestThePropertyRan;
            public static bool TestTheConstructorRan;

            [Argument("TP")]
            public TimeSpan TestProperty { get; set; }

            public bool TestBoolean { get; private set; }

            public ExampleCommandType()
            {                
            }

            public ExampleCommandType(bool tp)
            {
                TestBoolean = tp;
            }

            public void ExampleCommand()
            {
                ExampleCommandRan = true;
            }

            public void ExampleCommand(string options)
            {
                ExampleCommandWithOptionsRan = true;
            }

            public static void StaticCommand()
            {
                StaticCommandRan = true;
            }

            public static void StaticCommand(string options)
            {
                StaticCommandWithOptionsRan = true;
            }

            [CommandName("NamedCommand")]
            public void CommandWithName([Argument("I")] int index)
            {
                if (index == 12)
                {
                    CommandWithNameRan = true;
                }
            }

            public void TestTheProperty()
            {
                TestThePropertyRan = this.TestProperty > TimeSpan.FromDays(1);                
            }

            public void TestTheConstructor()
            {
                TestTheConstructorRan = this.TestBoolean;
            }
        }

        // ReSharper restore UnusedMember.Local


        [TestMethod]
        public void CommandCanFindNamedMethods()
        {
            var types = new[] {typeof (ExampleCommandType)};
            var command = Command.FromArguments("ExampleCommandType.ExampleCommand");
            var methods = command.FindSuitableMethods(types).ToArray();

            Assert.IsNotNull(methods);
            Assert.AreEqual(2, methods.Length);
            Assert.IsTrue(methods.All(method => method.Name == "ExampleCommand"));

            Expect.Throw<InvalidOperationException>(() => command.FindSuitableMethods(new Type[] {}));
        }

        [TestMethod]
        public void CommandCanRun()
        {
            var types = new[] {typeof (ExampleCommandType)};
            var command = Command.FromArguments("ExampleCommandType.ExampleCommand");

            Assert.IsTrue(command.Run(types));
            Assert.IsTrue(ExampleCommandType.ExampleCommandRan);
        }

        [TestMethod]
        public void CommandWithOptionsCanRun()
        {
            var types = new[] {typeof (ExampleCommandType)};
            var command = Command.FromArguments("ExampleCommandType.ExampleCommand", "/options");

            Assert.IsTrue(command.Run(types));
            Assert.IsTrue(ExampleCommandType.ExampleCommandWithOptionsRan);
        }

        [TestMethod]
        public void StaticCommandCanRun()
        {
            var types = new[] {typeof (ExampleCommandType)};
            var command = Command.FromArguments("ExampleCommandType.StaticCommand");

            Assert.IsTrue(command.Run(types));
            Assert.IsTrue(ExampleCommandType.StaticCommandRan);
        }

        [TestMethod]
        public void StaticCommandWithOptionsCanRun()
        {
            var types = new[] { typeof(ExampleCommandType) };
            var command = Command.FromArguments("ExampleCommandType.StaticCommand", "/options");

            Assert.IsTrue(command.Run(types));
            Assert.IsTrue(ExampleCommandType.StaticCommandWithOptionsRan);
        }

        [TestMethod]
        public void CommandWithNameCanRun()
        {
            var types = new[] { typeof(ExampleCommandType) };
            var command = Command.FromArguments("NamedCommand", "/I:12");

            Assert.IsTrue(command.Run(types));
            Assert.IsTrue(ExampleCommandType.CommandWithNameRan);
        }

        [TestMethod]
        public void TestThePropertyCanRun()
        {
            var types = new[] { typeof(ExampleCommandType) };
            var command = Command.FromArguments("TestTheProperty", "/TP:12");

            Assert.IsTrue(command.Run(types));
            Assert.IsTrue(ExampleCommandType.TestThePropertyRan);
        }

        [TestMethod]
        public void TestTheConstructorCanRun()
        {
            var types = new[] { typeof(ExampleCommandType) };
            var command = Command.FromArguments("TestTheConstructor", "/tp");

            Assert.IsTrue(command.Run(types));
            Assert.IsTrue(ExampleCommandType.TestTheConstructorRan);
        }

    }
}
