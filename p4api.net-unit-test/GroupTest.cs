﻿using Perforce.P4;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using NLog;

namespace p4api.net.unit.test
{
    /// <summary>
    ///This is a test class for GroupTest and is intended
    ///to contain all GroupTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GroupTest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void SetupTest()
        {
            Utilities.LogTestStart(TestContext);
        }
        [TestCleanup]
        public void CleanupTest()
        {
            Utilities.LogTestFinish(TestContext);
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        private static String GroupSpec =
                "Group:\tTest" + Environment.NewLine + Environment.NewLine
                + "MaxResults:\t100" + Environment.NewLine + Environment.NewLine
                + "MaxScanRows:\t10" + Environment.NewLine + Environment.NewLine
                + "MaxLockTime:\t20" + Environment.NewLine + Environment.NewLine
                + "MaxOpenFiles:\t50" + Environment.NewLine + Environment.NewLine
                + "Timeout:\t30" + Environment.NewLine + Environment.NewLine
                + "PasswordTimeout:\t40" + Environment.NewLine + Environment.NewLine
                + "Subgroups:" + Environment.NewLine
                + "\tSG1" + Environment.NewLine 
                + "\tSG2" + Environment.NewLine + Environment.NewLine 
                + "Owners:" + Environment.NewLine
                + "\tAlpha" + Environment.NewLine
                + "\tBeta" + Environment.NewLine + Environment.NewLine
                + "Users:" + Environment.NewLine
                + "\tAlice" + Environment.NewLine
                + "\tBob" + Environment.NewLine + Environment.NewLine
                + "Description:\tTest Description" + Environment.NewLine + Environment.NewLine
                + "MaxMemory:\t60" + Environment.NewLine + Environment.NewLine;


        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest()
        {
            Group target = new Group();
            string spec = GroupSpec; 
            bool expected = true; 
            bool actual;
            actual = target.Parse(spec);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(target.TimeOut, 30);
            Assert.AreEqual(target.OwnerNames[0], "Alpha");

            Assert.AreEqual(target.Id, "Test");
            Assert.AreEqual(target.Description, "Test Description");
            Assert.AreEqual(target.MaxResults, 100);
            Assert.AreEqual(target.MaxScanRows, 10);
            Assert.AreEqual(target.MaxLockTime, 20);
            Assert.AreEqual(target.TimeOut, 30);
            Assert.AreEqual(target.PasswordTimeout, 40);
            Assert.AreEqual(target.MaxOpenFiles, 50);
            Assert.AreEqual(target.MaxMemory, 60);
            Assert.AreEqual(target.OwnerNames[0], "Alpha");
            Assert.AreEqual(target.OwnerNames[1], "Beta");
            Assert.AreEqual(target.UserNames[0], "Alice");
            Assert.AreEqual(target.UserNames[1], "Bob");
            Assert.AreEqual(target.SubGroups[0], "SG1");
            Assert.AreEqual(target.SubGroups[1], "SG2");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Group target = new Group();
            target.Id = "Test";
            target.Description = "Test Description";
            target.MaxResults = 100;
            target.MaxScanRows = 10;
            target.MaxLockTime = 20;
            target.TimeOut = 30;
            target.PasswordTimeout = 40;
            target.MaxOpenFiles = 50;
            target.MaxMemory = 60;
            target.OwnerNames = new List<string>() { "Alpha", "Beta" };
            target.UserNames = new List<string>() { "Alice", "Bob" };
            target.SubGroups = new List<string>() { "SG1", "SG2" };

            string expected = GroupSpec.Trim(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ToString().Trim();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FromGroupCmdTaggedOutput
        ///</summary>
        [TestMethod()]
        public void FromGroupCmdTaggedOutputTest()
        {
            Group target = new Group(); // TODO: Initialize to an appropriate value
            TaggedObject objectInfo = new TaggedObject(); // TODO: Initialize to an appropriate value

            objectInfo["Group"] = "Test";

            objectInfo["Description"] = "Test Description";
            objectInfo["MaxResults"] = "100";
            objectInfo["MaxScanRows"] = "10";
            objectInfo["MaxLockTime"] = "20";
            objectInfo["Timeout"] = "30";
            objectInfo["PasswordTimeout"] = "40";
            objectInfo["MaxOpenFiles"] = "50";
            objectInfo["MaxMemory"] = "60";

            objectInfo["Users0"] = "Alice";
            objectInfo["Users1"] = "Bob";

            objectInfo["Owners0"] = "Alpha";
            objectInfo["Owners1"] = "Beta";

            objectInfo["Subgroups0"] = "SG1";
            objectInfo["Subgroups1"] = "SG2";

            target.FromGroupCmdTaggedOutput(objectInfo);

            Assert.AreEqual(target.Id, "Test");
            Assert.AreEqual(target.Description, "Test Description");
            Assert.AreEqual(target.MaxResults, 100);
            Assert.AreEqual(target.MaxScanRows, 10);
            Assert.AreEqual(target.MaxLockTime, 20);
            Assert.AreEqual(target.MaxOpenFiles, 50);
            Assert.AreEqual(target.MaxMemory, 60);
            Assert.AreEqual(target.TimeOut, 30);
            Assert.AreEqual(target.PasswordTimeout, 40);
            Assert.AreEqual(target.OwnerNames[0], "Alpha");
            Assert.AreEqual(target.OwnerNames[1], "Beta");
            Assert.AreEqual(target.UserNames[0], "Alice");
            Assert.AreEqual(target.UserNames[1], "Bob");
            Assert.AreEqual(target.SubGroups[0], "SG1");
            Assert.AreEqual(target.SubGroups[1], "SG2");
        }
    }
}
