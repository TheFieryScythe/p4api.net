using Perforce.P4;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NLog;

namespace p4api.net.unit.test
{
    /// <summary>
    ///This is a test class for ClientTest and is intended
    ///to contain all ClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ClientTest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private UnitTestConfiguration configuration;
        private string TestDir = "";
        
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void SetupTest()
        {
            configuration = UnitTestSettings.GetApplicationConfiguration();
            TestDir = configuration.TestDirectory;
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


        /// <summary>
        ///A test for Client Constructor
        ///</summary>
        [TestMethod()]
        public void ClientConstructorTest()
        {
            Client target = new Client();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for FormatDateTime
        ///</summary>
        [TestMethod()]
        public void FormatDateTimeTest()
        {
            DateTime dt = new DateTime(2011, 2, 3, 4, 5, 6); // 2/3/2011 4:05:06
            string expected = "2011/02/03 04:05:06";
            string actual;
            actual = Client.FormatDateTime(dt);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for handling null DateTime object
        /// </summary>
        [TestMethod()]
        public void FormatDateTimeNullTest()
        {
            DateTime dt = new DateTime();
            string expected = "";
            string actual;
            actual = Client.FormatDateTime(dt);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FromTaggedOutput
        ///</summary>
        [TestMethod()]
        public void FromTaggedOutputTest()
        {
            Client target = new Client(); // TODO: Initialize to an appropriate value
            Perforce.P4.TaggedObject workspaceInfo = new Perforce.P4.TaggedObject();
            workspaceInfo["Client"] = "clientName";
            workspaceInfo["Update"] = "2010/01/02 03:04:05"; // DateTime(2010, 1, 2, 3, 4, 5);
            workspaceInfo["Access"] = "2011/02/03 04:05:06"; // DateTime(2011, 2, 3, 4, 5, 6);
            workspaceInfo["Owner"] = "JoeOwner";
            workspaceInfo["Options"] = "allwrite noclobber compress unlocked modtime normdir noaltsync";
            //new ClientOptionEnum(ClientOption.AllWrite | ClientOption.Compress | ClientOption.ModTime);
            workspaceInfo["SubmitOptions"] = "revertunchanged+reopen";
            //new ClientSubmitOptions(true, SubmitType.RevertUnchanged);
            workspaceInfo["LineEnd"] = "LOCAL"; // LineEnd.Local;
            workspaceInfo["Root"] = "C:\\clientname";
            workspaceInfo["Host"] = "MissManners";
            workspaceInfo["Description"] = "Miss Manners client";
            workspaceInfo["AltRoots0"] = "C:\\alt0";
            workspaceInfo["AltRoots1"] = "C:\\alt1";
            workspaceInfo["Stream"] = "//Rocket/dev1";
            workspaceInfo["Type"] = "readonly";
            workspaceInfo["ChangeView0"] = "//depot/main/p4/test@1";
            workspaceInfo["View0"] = "	//depot/main/p4/... //dbarbee_win-dbarbee/main/p4/...";
            // new MapEntry(MapType.Include,
            //		new PathSpec(PathType.DEPOT_PATH, null, "//depot/main/p4/..."),
            //		new PathSpec(PathType.CLIENT_PATH, null, "//dbarbee_win-dbarbee/main/p4/..."));
            workspaceInfo["View1"] = "-//usr/... //dbarbee_win-dbarbee/usr/...";
            //new MapEntry(MapType.Exclude,
            //		new PathSpec(PathType.DEPOT_PATH, null, "//usr/..."),
            //		new PathSpec(PathType.CLIENT_PATH, null, "//dbarbee_win-dbarbee/usr/..."));
            workspaceInfo["View2"] = "+//spec/... //dbarbee_win-dbarbee/spec/...";
            //new MapEntry(MapType.Overlay,
            //		new PathSpec(PathType.DEPOT_PATH, null, "//spec/..."),
            //		new PathSpec(PathType.CLIENT_PATH, null, "//dbarbee_win-dbarbee/spec/..."));

            target.FromClientCmdTaggedOutput(workspaceInfo);
            Assert.AreEqual("clientName", target.Name);
            Assert.AreEqual(new DateTime(2010, 1, 2, 3, 4, 5), target.Updated);
            Assert.AreEqual(new DateTime(2011, 2, 3, 4, 5, 6), target.Accessed);
            Assert.AreEqual("JoeOwner", target.OwnerName);
            Assert.AreEqual((ClientOption.AllWrite | ClientOption.Compress | ClientOption.ModTime),
                target.Options);
            Assert.AreEqual(new ClientSubmitOptions(true, SubmitType.RevertUnchanged), target.SubmitOptions);
            Assert.AreEqual(LineEnd.Local, target.LineEnd);
            Assert.AreEqual("C:\\clientname", target.Root);
            Assert.AreEqual("MissManners", target.Host);
            Assert.AreEqual("Miss Manners client", target.Description);
            Assert.AreEqual("C:\\alt0", target.AltRoots[0]);
            Assert.AreEqual("C:\\alt1", target.AltRoots[1]);
            Assert.AreEqual(ClientType.@readonly, target.ClientType);
            Assert.AreEqual("//depot/main/p4/test@1", target.ChangeView[0]);
            Assert.AreEqual("//Rocket/dev1", target.Stream);
            Assert.AreEqual(new MapEntry(MapType.Include,
                    new DepotPath("//depot/main/p4/..."),
                    new ClientPath("//dbarbee_win-dbarbee/main/p4/...")),
                target.ViewMap[0]);
            Assert.AreEqual(new MapEntry(MapType.Exclude,
                    new DepotPath("//usr/..."),
                    new ClientPath("//dbarbee_win-dbarbee/usr/...")),
                target.ViewMap[1]);
            Assert.AreEqual(new MapEntry(MapType.Overlay,
                    new DepotPath("//spec/..."),
                    new ClientPath("//dbarbee_win-dbarbee/spec/...")),
                target.ViewMap[2]);
        }

        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void ParseTest()
        {
            Client target = new Client(); // TODO: Initialize to an appropriate value
            string spec =
                "Client:\tclientName\r\n\r\nUpdate:\t2010/01/02 03:04:05\r\n\r\nAccess:\t2011/02/03 04:05:06\r\n\r\nOwner:\tJoeOwner\r\n\r\nHost:\tMissManners\r\n\r\nDescription:\r\n\tMiss Manners client\r\n\r\nRoot:\tC:\\clientname\r\n\r\nAltRoots:\r\n\tC:\\alt0\r\n\tC:\\alt1\r\n\r\nChangeView:\r\n\t//depot/main/p4/changeview1@1\r\n\t//depot/main/p4/changeview2@1\r\n\r\nOptions:\tallwrite noclobber compress unlocked modtime normdir noaltsync\r\n\r\nSubmitOptions:\trevertunchanged+reopen\r\n\r\nLineEnd:\tLocal\r\n\r\nView:\r\n\t//depot/main/p4/... //dbarbee_win-dbarbee/main/p4/...\r\n\t-//usr/... //dbarbee_win-dbarbee/usr/...\r\n\t+//spec/... //dbarbee_win-dbarbee/spec/...\r\n";
            bool expected = true;
            bool actual;
            actual = target.Parse(spec);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual("clientName", target.Name);
            Assert.AreEqual(new DateTime(2010, 1, 2, 3, 4, 5), target.Updated);
            Assert.AreEqual(new DateTime(2011, 2, 3, 4, 5, 6), target.Accessed);
            Assert.AreEqual("JoeOwner", target.OwnerName);
            Assert.AreEqual((ClientOption.AllWrite | ClientOption.Compress | ClientOption.ModTime),
                target.Options);
            Assert.AreEqual(new ClientSubmitOptions(true, SubmitType.RevertUnchanged), target.SubmitOptions);
            Assert.AreEqual(LineEnd.Local, target.LineEnd);
            Assert.AreEqual("C:\\clientname", target.Root);
            Assert.AreEqual("MissManners", target.Host);
            Assert.AreEqual("Miss Manners client", target.Description);
            Assert.AreEqual("C:\\alt0", target.AltRoots[0]);
            Assert.AreEqual("C:\\alt1", target.AltRoots[1]);
            Assert.AreEqual("//depot/main/p4/changeview1@1", target.ChangeView[0]);
            Assert.AreEqual("//depot/main/p4/changeview2@1", target.ChangeView[1]);
            Assert.AreEqual(new MapEntry(MapType.Include,
                    new DepotPath("//depot/main/p4/..."),
                    new ClientPath("//dbarbee_win-dbarbee/main/p4/...")),
                target.ViewMap[0]);
            Assert.AreEqual(new MapEntry(MapType.Exclude,
                    new DepotPath("//usr/..."),
                    new ClientPath("//dbarbee_win-dbarbee/usr/...")),
                target.ViewMap[1]);
            Assert.AreEqual(new MapEntry(MapType.Overlay,
                    new DepotPath("//spec/..."),
                    new ClientPath("//dbarbee_win-dbarbee/spec/...")),
                target.ViewMap[2]);
        }

        /// <summary>
        ///A test for ToStringjob084053
        ///</summary>
        [TestMethod()]
        public void ToStringTestjob084053()
        {
            Client target = new Client();
            target.Name = "clientName";
            target.Updated = new DateTime(2010, 1, 2, 3, 4, 5);
            target.Accessed = new DateTime(2011, 2, 3, 4, 5, 6);
            target.OwnerName = "JoeOwner";
            // Don't set target.Options, target.SubmitOptions, and target.LineEnd
            // to confirm default values are set on Client creation.
            target.Root = "C:\\clientname";
            target.Host = "MissManners";
            target.Description = "Miss Manners client";
            target.AltRoots = new List<string>();
            target.AltRoots.Add("C:\\alt0");
            target.AltRoots.Add("C:\\alt1");
            target.ServerID = configuration.ServerPort;
            target.Stream = "//Stream/main";
            target.StreamAtChange = "111";

            target.ViewMap = new ViewMap(new string[]
            {
                "	//depot/main/p4/... //dbarbee_win-dbarbee/main/p4/...",
                "-//usr/... //dbarbee_win-dbarbee/usr/...",
                "+//spec/... //dbarbee_win-dbarbee/spec/..."
            });

            string expected =
                "Client:\tclientName\n\nUpdate:\t2010/01/02 03:04:05\n\nAccess:\t2011/02/03 04:05:06\n\nOwner:\tJoeOwner\n\nHost:\tMissManners\n\nDescription:\n\tMiss Manners client\n\nRoot:\tC:\\clientname\n\nAltRoots:\n\tC:\\alt0\n\tC:\\alt1\n\nOptions:\tnoallwrite noclobber nocompress unlocked nomodtime normdir noaltsync\n\nSubmitOptions:\tsubmitunchanged\n\nLineEnd:\tLocal\n\nType:\twriteable\n\nStream:\t//Stream/main\n\nStreamAtChange:\t111\n\nServerID:\t" + configuration.ServerPort +"\n\nView:\n\t//depot/main/p4/... //dbarbee_win-dbarbee/main/p4/...\n\t-//usr/... //dbarbee_win-dbarbee/usr/...\n\t+//spec/... //dbarbee_win-dbarbee/spec/...\n";
            
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Client target = new Client();
            target.Name = "clientName";
            target.Updated = new DateTime(2010, 1, 2, 3, 4, 5);
            target.Accessed = new DateTime(2011, 2, 3, 4, 5, 6);
            target.OwnerName = "JoeOwner";
            target.Options = (ClientOption.AllWrite | ClientOption.Compress | ClientOption.ModTime);
            target.SubmitOptions = new ClientSubmitOptions(true, SubmitType.RevertUnchanged);
            target.LineEnd = LineEnd.Local;
            target.Root = "C:\\clientname";
            target.Host = "MissManners";
            target.Description = "Miss Manners client";
            target.AltRoots = new List<string>();
            target.AltRoots.Add("C:\\alt0");
            target.AltRoots.Add("C:\\alt1");
            target.ServerID = configuration.ServerPort;
            target.Stream = "//Stream/main";
            target.StreamAtChange = "111";
            target.ChangeView = new List<string>();
            target.ChangeView.Add("//dbarbee_win-dbarbee/main/p4/test@2");

            target.ViewMap = new ViewMap(new string[]
            {
                "	//depot/main/p4/... //dbarbee_win-dbarbee/main/p4/...",
                "-//usr/... //dbarbee_win-dbarbee/usr/...",
                "+//spec/... //dbarbee_win-dbarbee/spec/..."
            });

            string expected =
                "Client:\tclientName\n\nUpdate:\t2010/01/02 03:04:05\n\nAccess:\t2011/02/03 04:05:06\n\nOwner:\tJoeOwner\n\nHost:\tMissManners\n\nDescription:\n\tMiss Manners client\n\nRoot:\tC:\\clientname\n\nAltRoots:\n\tC:\\alt0\n\tC:\\alt1\n\nOptions:\tallwrite noclobber compress unlocked modtime normdir noaltsync\n\nSubmitOptions:\trevertunchanged+reopen\n\nLineEnd:\tLocal\n\nType:\twriteable\n\nStream:\t//Stream/main\n\nStreamAtChange:\t111\n\nServerID:\t"+ configuration.ServerPort + "\n\nChangeView:\t//dbarbee_win-dbarbee/main/p4/test@2\n\nView:\n\t//depot/main/p4/... //dbarbee_win-dbarbee/main/p4/...\n\t-//usr/... //dbarbee_win-dbarbee/usr/...\n\t+//spec/... //dbarbee_win-dbarbee/spec/...\n";
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for AltRootsStr
        ///</summary>
        [TestMethod()]
        public void AltRootsStrTest()
        {
            Client target = new Client();
            List<string> expected = new List<string>();
            string root = @"C:\depot";
            expected.Add(root);
            target.AltRoots = expected;
            IList<string> actual;
            actual = target.AltRoots;
            Assert.IsTrue(actual.Contains(@"C:\depot"));
        }

        /// <summary>
        ///A test for LineEnd
        ///</summary>
        [TestMethod()]
        public void LineEndTest()
        {
            Client target = new Client();
            LineEnd expected = LineEnd.Mac;
            LineEnd actual;
            target.LineEnd = expected;
            actual = target.LineEnd;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Options
        ///</summary>
        [TestMethod()]
        public void OptionsTest()
        {
            Client target = new Client();
            ClientOption expected = ClientOption.Clobber;
            ClientOption actual;
            target.Options = expected;
            actual = target.Options;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for initialize
        ///</summary>
        [TestMethod()]
        public void initializeTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            string dir = Directory.GetCurrentDirectory();

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                var cptype = (Utilities.CheckpointType)i;
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));

                    rep = new Repository(server);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        if (cptype == Utilities.CheckpointType.U)
                        {
                            con.CharacterSetName = "utf8";
                            Assert.AreEqual("utf8", con.CharacterSetName);
                        }

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for addFiles
        ///</summary>
        [TestMethod()]
        public void addFilesTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            string dir = Directory.GetCurrentDirectory();

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                var cptype = (Utilities.CheckpointType)i;
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        if (cptype == Utilities.CheckpointType.U)
                        {
                            con.CharacterSetName = "utf8";
                            Assert.AreEqual("utf8", con.CharacterSetName);
                        }
                        else
                        {
                            con.CharacterSetName = "none";
                            Assert.AreEqual("none", con.CharacterSetName);
                        }

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);
                    

                        var codePath = Path.Combine(clientRoot, "admin_space", "MyCode");
                        Directory.CreateDirectory(codePath);
                        System.IO.File.Copy(Path.Combine(codePath, "ReadMe.txt"),
                            Path.Combine(codePath, "NewFile2.txt"));
                        FileSpec toFile = new FileSpec(new LocalPath(Path.Combine(codePath, "NewFile2.txt")), null);
                        Options options = new Options(AddFilesCmdFlags.None, -1, null);
                        IList<FileSpec> newfiles = con.Client.AddFiles(options, toFile);

                        Assert.AreEqual(1, newfiles.Count);

                        foreach (var fileSpec in newfiles)
                        {
                            Assert.IsNotNull(fileSpec.DepotPath.Path);
                            Assert.IsNotNull(fileSpec.ClientPath.Path);
                            Assert.IsNotNull(fileSpec.LocalPath.Path);
                        }

                        con.Client.RevertFiles(null, toFile);

                        options = new Options(AddFilesCmdFlags.None, 0, null);
                        newfiles = con.Client.AddFiles(options, toFile);

                        Assert.AreEqual(1, newfiles.Count);

                        foreach (var fileSpec in newfiles)
                        {
                            Assert.IsNotNull(fileSpec.DepotPath.Path);
                            Assert.IsNotNull(fileSpec.ClientPath.Path);
                            Assert.IsNotNull(fileSpec.LocalPath.Path);
                    }
                }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for addFilesjob085728U
        ///</summary>
        [TestMethod()]
        public void addFilesTestjob085728U()
        {
            addFilesTestjob085728(Utilities.CheckpointType.U);
        }

        /// <summary>
        ///A test for addFilesjob085728A
        ///</summary>
        [TestMethod()]
        public void addFilesTestjob085728A()
        {
            addFilesTestjob085728(Utilities.CheckpointType.A);
        }
        
        /// <summary>
        ///A test for addFilesjob085728
        ///</summary>
        public void addFilesTestjob085728(Utilities.CheckpointType cptype)
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            string dir = Directory.GetCurrentDirectory();

            Process p4d = null;
            Repository rep = null;

            try
            {
                p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                Assert.IsNotNull(p4d, "Setup Failure");

                var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                var adminSpace = Path.Combine(clientRoot, "admin_space");
                Directory.CreateDirectory(adminSpace);

                Server server = new Server(new ServerAddress(uri));
                rep = new Repository(server);
                Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                using (Connection con = rep.Connection)
                {
                    con.UserName = user;
                    con.Client = new Client();
                    con.Client.Name = ws_client;


                    if (cptype == Utilities.CheckpointType.U)
                    {
                        con.CharacterSetName = "utf8";
                        Assert.AreEqual("utf8", con.CharacterSetName);
                    }
                    else
                    {
                        con.CharacterSetName = "none";
                        Assert.AreEqual("none", con.CharacterSetName);
                    }

                    Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                    Assert.AreEqual(con.Server.State, ServerState.Unknown);
                    Assert.IsTrue(con.Connect(null));
                    Assert.AreEqual(con.Server.State, ServerState.Online);
                    Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                    Assert.AreEqual("admin", con.Client.OwnerName);
                 

                    var codePath = Path.Combine(clientRoot, "admin_space", "MyCode");
                    Directory.CreateDirectory(codePath);
                    System.IO.File.Copy(Path.Combine(codePath, "ReadMe.txt"),
                        Path.Combine(codePath, "NewFile2.txt"));
                    FileSpec toFile = new FileSpec(new LocalPath(Path.Combine(codePath, "NewFile2.txt")), null);
                    Options options = new Options(AddFilesCmdFlags.None, -1, null);

                    IList<FileSpec> newfiles = con.Client.AddFiles(options, toFile);

                    Assert.AreEqual(1, newfiles.Count);

                    // Check to confirm that the rev is null for a filespec
                    // of a file marked for add
                    Assert.IsTrue(newfiles[0].Version == null);
                    foreach (var fileSpec in newfiles)
                    {
                        Assert.IsNotNull(fileSpec.DepotPath.Path);
                        Assert.IsNotNull(fileSpec.ClientPath.Path);
                        Assert.IsNotNull(fileSpec.LocalPath.Path);
                    }

                    IList<Perforce.P4.File> files = rep.GetOpenedFiles(newfiles, options);
                    // Check to confirm that the rev is null for a file
                    // of a file marked for add
                    Assert.IsTrue(files[0].Version == null);

                    // get filemetadata for the file marked for add. Previously
                    // this would come back null when attempting to fstat
                    // //depot/filemarkedforadd#1 as it would return
                    // "no such file(s)"
                    IList<FileMetaData> fmd = rep.GetFileMetaData(newfiles, null);
                    Assert.IsNotNull(fmd);
                }
            }
            finally
            {
                Utilities.RemoveTestServer(p4d, TestDir);
                p4d?.Dispose();
                rep?.Dispose();
            }
        }
        /// <summary>
        /// Test fstat calls - for performance - both existing and non-existent files
        /// </summary>
        [TestMethod()]
        public void fstatTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";
        

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        var codePath = Path.Combine(clientRoot, "admin_space", "MyCode");
                        var newFile = Path.Combine(codePath, "NewFile2.txt");
                        Directory.CreateDirectory(codePath);
                        System.IO.File.Copy(Path.Combine(codePath, "ReadMe.txt"),
                            newFile);
                        FileSpec fs = FileSpec.DepotSpec("//depot/MyCode/ReadMe.txt");
                        logger.Debug("Fstating {0}", fs.DepotPath.Path);
                        Options ops = new Options();
                        IList<FileMetaData> actual = rep.GetFileMetaData(ops, fs);
                        FileAction expected = FileAction.Add;
                        Assert.AreEqual(expected, actual[0].HeadAction);

                        // Now try for a non-existent file
                        fs = FileSpec.LocalSpec(newFile);
                        logger.Debug("Fstating {0}", fs.LocalPath.Path);
                        actual = rep.GetFileMetaData(ops, fs);
                        Assert.AreEqual(actual, null);
                        Assert.AreEqual(1, rep.Connection.LastResults.ErrorList.Count);
                        Assert.AreEqual(ErrorSeverity.E_WARN, rep.Connection.LastResults.ErrorList[0].SeverityLevel);
                        Assert.IsTrue(rep.Connection.LastResults.ErrorList[0].ToString().Contains("no such file(s)"));
                        logger.Debug("Finished fstat");
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }

                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for DeleteFiles
        ///</summary>
        [TestMethod()]
        public void DeleteFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec toFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "ReadMe.txt")),
                            null);
                        Options options = new Options(DeleteFilesCmdFlags.None, -1);
                        IList<FileSpec> oldfiles = con.Client.DeleteFiles(options, toFile);

                        Assert.AreEqual(1, oldfiles.Count);

                        con.Client.RevertFiles(null, toFile);

                        options = new Options(DeleteFilesCmdFlags.None, 0);
                        oldfiles = con.Client.DeleteFiles(options, toFile);

                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }

                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for DeleteFiles with Preview Only option
        ///</summary>
        [TestMethod()]
        public void DeleteFilesPreviewOnlyTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";
           

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        String filePath = Path.Combine(adminSpace, "MyCode", "ReadMe.txt");
                        FileSpec toFile = new FileSpec(new LocalPath(filePath), null);
                        Options options = new Options(DeleteFilesCmdFlags.PreviewOnly, -1);
                        IList<FileSpec> oldfiles = con.Client.DeleteFiles(options, toFile);

                        Assert.IsTrue(System.IO.File.Exists(filePath), filePath + " does not exist.");
                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }

                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for DeleteFiles with Server Only option
        ///</summary>
        [TestMethod()]
        public void DeleteFilesServerOnlyTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        String filePath = Path.Combine(adminSpace, "MyCode", "ReadMe.txt");
                        FileSpec toFile = new FileSpec(new LocalPath(filePath), null);
                        Options options = new Options(DeleteFilesCmdFlags.ServerOnly, -1);
                        IList<FileSpec> oldfiles = con.Client.DeleteFiles(options, toFile);

                        FileSpec fs = FileSpec.DepotSpec("//depot/MyCode/ReadMe.txt");
                        Options ops = new Options();
                        IList<FileMetaData> actual = rep.GetFileMetaData(ops, fs);
                        FileAction expected = FileAction.Delete;
                        Assert.AreEqual(expected, actual[0].Action);

                        Assert.IsTrue(System.IO.File.Exists(filePath), filePath + " does not exist.");
                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for DeleteFiles with Unsynced option
        ///</summary>
        [TestMethod()]
        public void DeleteFilesUnsyncedTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";


            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client, true);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        String filePath = Path.Combine(adminSpace, "MyCode", "ReadMe.txt");
                        String depotPath = "//depot/MyCode/ReadMe.txt";
                        FileSpec toFile = new FileSpec(new DepotPath(depotPath));
                        FileSpec toFile0 = new FileSpec(new DepotPath(depotPath), null, null, VersionSpec.None);
                        con.Client.SyncFiles(null, toFile0);

                        Options options = new Options(DeleteFilesCmdFlags.DeleteUnsynced, -1);
                        IList<FileSpec> oldfiles = con.Client.DeleteFiles(options, toFile);

                        FileSpec fs = FileSpec.DepotSpec(depotPath);
                        IList<FileMetaData> actual = rep.GetFileMetaData(null, fs);
                        FileAction expected = FileAction.Delete;
                        Assert.AreEqual(expected, actual[0].Action);

                        Assert.IsFalse(System.IO.File.Exists(filePath));
                        Assert.AreEqual(1, oldfiles.Count);

                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for EditFiles
        ///</summary>
        [TestMethod()]
        public void EditFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec toFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "ReadMe.txt")),
                            null);
                        Options options = new Options(EditFilesCmdFlags.None, -1, null);
                        IList<FileSpec> oldfiles = con.Client.EditFiles(options, toFile);
                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for GetSyncedFiles
        ///</summary>
        [TestMethod()]
        public void GetSyncedFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec toFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "ReadMe.txt")),
                            null);
                        Options options = null;
                        IList<FileSpec> oldfiles = con.Client.GetSyncedFiles(options, toFile);
                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for IntegrateFiles for the 
        ///"p4 integrate [options] fromFile[revRange] toFile"
        ///version of integrate
        ///</summary>
        [TestMethod()]
        public void IntegrateFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "ReadMe.txt")),
                            null);
                        FileSpec toFile =
                            new FileSpec(new LocalPath(Path.Combine(adminSpace, "branchAlpha", "ReadMe.txt")), null);
                        Options options = new Options(IntegrateFilesCmdFlags.None,
                            -1,
                            10,
                            null,
                            null,
                            null);
                        IList<FileSpec> oldfiles = con.Client.IntegrateFiles(fromFile, options, toFile);

                        Assert.AreEqual(1, oldfiles.Count);

                        con.Client.RevertFiles(null, toFile);

                        options = new Options(IntegrateFilesCmdFlags.None,
                            0,
                            10,
                            null,
                            null,
                            null);
                        oldfiles = con.Client.IntegrateFiles(fromFile, options, toFile);

                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for IntegrateFiles for the 
        ///"p4 integrate [options] -b branch [-r] [toFile[revRange] ...]"
        ///version of integrate
        ///</summary>
        [TestMethod()]
        public void IntegrateFilesTest1()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 10, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);
                        string branch = "MyCode->MyCode2";
                        List<FileSpec> toFiles = new List<FileSpec>();
                        FileSpec toFile = new FileSpec(new DepotPath("//depot/MyCode2/Silly.bmp"), null);
                        toFiles.Add(toFile);
                        Options options = new Options(IntegrateFilesCmdFlags.Force,
                            -1,
                            10,
                            branch,
                            null,
                            null);
                        IList<FileSpec> oldfiles = con.Client.IntegrateFiles(toFiles, options);

                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for LabelSync
        ///</summary>
        [TestMethod()]
        public void LabelSyncTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";



            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 4, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        IList<FileSpec> oldfiles = con.Client.LabelSync(null, "admin_label", (FileSpec) null);

                        Assert.AreEqual(null, oldfiles);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for LockFiles
        ///</summary>
        [TestMethod()]
        public void LockFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 5, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        IList<FileSpec> oldfiles = con.Client.LockFiles(null);

                        Assert.AreNotEqual(null, oldfiles);

                        con.Client.UnlockFiles(null);

                        Options lockOpts = new LockCmdOptions(-1);
                        oldfiles = con.Client.LockFiles(lockOpts, (FileSpec) null);

                        con.Client.UnlockFiles(null);

                        lockOpts = new LockCmdOptions(0);
                        oldfiles = con.Client.LockFiles(lockOpts, (FileSpec) null, (FileSpec) null, (FileSpec) null);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for MoveFiles
        ///</summary>
        [TestMethod()]
        public void MoveFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "ReadMe.txt")),
                            null);

                        IList<FileSpec> oldfiles = con.Client.EditFiles(null, fromFile);
                        Assert.AreEqual(1, oldfiles.Count);

                        FileSpec toFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "ReadMe42.txt")),
                            null);
                        Options options = new Options(MoveFileCmdFlags.None, -1, null);
                        oldfiles = con.Client.MoveFiles(fromFile, toFile, options);

                        Assert.AreEqual(1, oldfiles.Count);

                        con.Client.RevertFiles(null, toFile);

                        oldfiles = con.Client.EditFiles(null, fromFile);
                        Assert.AreEqual(1, oldfiles.Count);

                        options = new Options(MoveFileCmdFlags.None, 0, null);
                        oldfiles = con.Client.MoveFiles(fromFile, toFile, options);

                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for ReopenFiles
        ///</summary>
        [TestMethod()]
        public void ReopenFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "ReadMe.txt")),
                            null);

                        IList<FileSpec> oldfiles = con.Client.EditFiles(null, fromFile);
                        Assert.AreEqual(1, oldfiles.Count);

                        FileType ft = new FileType(BaseFileType.Unicode, FileTypeModifier.ExclusiveOpen);

                        Options ops = new Options(-1, ft);
                        oldfiles = con.Client.ReopenFiles(ops, fromFile);
                        Assert.AreEqual(1, oldfiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for ResolveFiles
        ///</summary>
        [TestMethod()]
        public void ResolveFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        //
                        // NEEDS WORK!
                        //

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);
                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Check It In!",
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, fromFile);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Options rFlags = new Options(
                            ResolveFilesCmdFlags.PreviewOnly, -1);
                        IList<FileResolveRecord> records = con.Client.ResolveFiles(rFlags, fromFile);
                        Assert.IsNotNull(records);

                        rFlags = new Options(ResolveFilesCmdFlags.AutomaticForceMergeMode, -1);
                        records = con.Client.ResolveFiles(rFlags, fromFile);
                        Assert.IsNotNull(records);

                        con.Client.RevertFiles(null, fromFile);

                        fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);
                        sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Check It In!",
                            null
                        );
                        sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, fromFile);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        rFlags = new Options(
                            ResolveFilesCmdFlags.PreviewOnly, 0);
                        records = con.Client.ResolveFiles(rFlags, fromFile);

                        // they no longer need resolve at this point
                        // only testing the rFlag with changelist 0
                        Assert.IsNull(records);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for ResolveFiles
        ///</summary>
        [TestMethod()]
        public void ResolveFilesTest0()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 13, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        //
                        // NEEDS WORK!
                        //

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec depotFiles = FileSpec.DepotSpec("//depot/MyCode2/...");
                        IList<FileSpec> files = new List<FileSpec>();
                        files.Add(new FileSpec(new DepotPath("//depot/MyCode2/DeleteResolve2.txt"), null, null, null));
                        files.Add(new FileSpec(new DepotPath("//depot/MyCode2/ReadMe.txt"), null, null, null));
                        rep.Connection.Client.ReopenFiles(files, new Options(6, null));
                        Options autoResolveOptions = new ResolveCmdOptions(ResolveFilesCmdFlags.AutomaticMergeMode, 6);
                        rep.Connection.Client.ResolveFiles(autoResolveOptions, depotFiles);

                        P4CommandResult results = con.LastResults;

                        Assert.IsTrue(results.Success);

                        IList<FileMetaData> fstat = rep.GetFileMetaData(files, null);

                        Assert.IsTrue(fstat[0].Resolved);
                        Assert.IsTrue(fstat[1].Resolved);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for ResolveFiles
        ///</summary>
       // [TestMethod()]      // Disable this test for now, it seems to not work reliably and brings up the editor requiring manual intervention
        public void ResolveFilesTest1()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        //
                        // NEEDS WORK!
                        //

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);
                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Check It In!",
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, fromFile);
                        }
                        catch
                        {
                        } // will fail because we need to resolve


                        Dictionary<String, String> responses = new Dictionary<string, string>();
                        responses["Accept(a) Edit(e) Diff(d) Merge (m) Skip(s) Help(?) am: "] = "am";
                        responses["Accept(a) Edit(e) Diff(d) Merge (m) Skip(s) Help(?) e:"] = "a";
                        responses["There are still change markers: confirm accept (y/n)"] = "y";

                        Options rFlags = new Options(ResolveFilesCmdFlags.IgnoreWhitespace, -1);
#pragma warning disable 618
                        IList<FileResolveRecord> records = con.Client.ResolveFiles(null, null, responses, rFlags,
                            fromFile);
#pragma warning restore 618
                        Assert.IsNotNull(records);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        private String HandlePrompt(uint cmdId, String msg, bool displayText)
        {
            if (msg == "Accept(a) Edit(e) Diff(d) Merge (m) Skip(s) Help(?) am: ")
                return "am";
            return "s";
        }

        /// <summary>
        ///A test for ResolveFiles
        ///</summary>
        [TestMethod()]
        public void ResolveFilesTest2()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

        

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        //
                        // NEEDS WORK!
                        //

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);
                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Check It In!",
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, fromFile);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Perforce.P4.P4Server.PromptHandlerDelegate promptHandler =
                            new Perforce.P4.P4Server.PromptHandlerDelegate(HandlePrompt);

                        Options rFlags = new Options(ResolveFilesCmdFlags.IgnoreWhitespace, -1);
#pragma warning disable 618
                        IList<FileResolveRecord> records = con.Client.ResolveFiles(null, promptHandler, null, rFlags,
                            fromFile);
#pragma warning restore 618
                        Assert.IsNotNull(records);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        private Connection resolveCon = null;

        private P4ClientMerge.MergeStatus HandleResolve(uint cmdId, P4ClientMerge merger)
        {
            if (resolveCon != null)
            {
                TaggedObjectList taggedOut = resolveCon.getP4Server().GetTaggedOutput(cmdId);
                P4ClientInfoMessageList infoOut = resolveCon.getP4Server().GetInfoResults(cmdId);
            }
            if (merger.AutoResolve(P4ClientMerge.MergeForce.CMF_AUTO) == P4ClientMerge.MergeStatus.CMS_MERGED)
            {
                return P4ClientMerge.MergeStatus.CMS_MERGED;
            }
            return P4ClientMerge.MergeStatus.CMS_SKIP;
        }

        /// <summary>
        ///A test for ResolveFiles
        ///</summary>
        [TestMethod()]
        public void ResolveFilesTest3()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        //
                        // NEEDS WORK!
                        //

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);
                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Check It In!",
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, fromFile);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Perforce.P4.P4Server.ResolveHandlerDelegate resolveHandler =
                            new Perforce.P4.P4Server.ResolveHandlerDelegate(HandleResolve);

                        Options rFlags = new Options(ResolveFilesCmdFlags.IgnoreWhitespace, -1);
                        resolveCon = con;
#pragma warning disable 618
                        IList<FileResolveRecord> records = con.Client.ResolveFiles(resolveHandler, null, null, rFlags,
                            fromFile);
#pragma warning restore 618
                        resolveCon = null;
                        Assert.IsNotNull(records);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for ResolveFiles
        ///</summary>
        [TestMethod()]
        public void ResolveFilesTest4()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        //
                        // NEEDS WORK!
                        //

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile1 = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);

                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Check It In!",
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, fromFile1);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Options rFlags = new Options(ResolveFilesCmdFlags.AutomaticMergeMode, -1);
                        resolveCon = con;
                        IList<FileResolveRecord> records = con.Client.ResolveFiles(null, rFlags, fromFile1);
                        //, fromFile2, fromFile3);
                        resolveCon = null;
                        Assert.IsNotNull(records);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        private bool itWorked = true;

        private P4ClientMerge.MergeStatus ResolveHandler6(FileResolveRecord resolveRecord,
            Client.AutoResolveDelegate AutoResolve, string sourcePath, string targetPath, string basePath,
            string resultsPath)
        {
            itWorked = true;

            if (sourcePath != null)
                itWorked &= System.IO.File.Exists(sourcePath);
            if (targetPath != null)
                itWorked &= System.IO.File.Exists(targetPath);
            if (basePath != null)
                itWorked &= System.IO.File.Exists(basePath);
            if (resultsPath != null)
                itWorked &= System.IO.File.Exists(resultsPath);

            return P4ClientMerge.MergeStatus.CMS_SKIP;
        }

        /// <summary>
        ///A test for ResolveFiles
        ///</summary>
        [TestMethod()]
        public void ResolveFilesTest6()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 12, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        //
                        // NEEDS WORK!
                        //

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile1 =
                            new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode2", "BranchResolve.txt")), null);
                        FileSpec fromFile2 =
                            new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode2", "DeleteResolve2.txt")), null);
                        FileSpec fromFile3 = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);

                        Options rFlags = new Options(ResolveFilesCmdFlags.DisplayBaseFile, -1);
                        resolveCon = con;

                        Client.ResolveFileDelegate resolver = new Client.ResolveFileDelegate(ResolveHandler6);

                        IList<FileResolveRecord> records = con.Client.ResolveFiles(resolver, rFlags, fromFile1,
                            fromFile2, fromFile3);
                        resolveCon = null;
                        Assert.IsNotNull(records);
                        Assert.IsTrue(itWorked);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for SubmitFiles
        ///</summary>
        [TestMethod()]
        public void SubmitFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 3, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = null;
                        // new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt"), null);
                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Submit the default changelist",
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, fromFile);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Assert.IsNotNull(sr);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for SubmitFiles
        ///</summary>
        [TestMethod()]
        public void SubmitFilesTest0()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 3, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        FileSpec fileToSubmit = new FileSpec(new DepotPath("//depot/TestData/Letters.txt"),
                            null, null, null);

                        SubmitCmdOptions submitOptions = new SubmitCmdOptions(
                            SubmitFilesCmdFlags.None, 0, null, "submitting default changelist", null);

                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(submitOptions, fileToSubmit);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Assert.IsNotNull(sr);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for SubmitFiles
        ///</summary>
        [TestMethod()]
        public void SubmitFilesTest1()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 3, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        Changelist change = new Changelist();
                        change.Description = "On the fly built change list";
                        FileMetaData file = new FileMetaData();
                        file.DepotPath = new DepotPath("//depot/TestData/Letters.txt");
                        change.Files.Add(file);

                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            change,
                            null,
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, null);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Assert.IsNotNull(sr);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for SubmitFiles
        ///</summary>
        [TestMethod()]
        public void SubmitFilesTest2()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 3, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            5,
                            null,
                            null,
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, null);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Assert.IsNotNull(sr);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }

                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for SubmitFiles
        ///</summary>
        [TestMethod()]
        public void SubmitShelvedFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";


            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 13, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        Options sFlags = new Options(
                            ShelveFilesCmdFlags.None,
                            null,
                            5
                        );

                        IList<FileSpec> rFiles = con.Client.ShelveFiles(sFlags);
                        rFiles[0].Version = null;
                        sFlags = new Options(
                            RevertFilesCmdFlags.None, 5);

                        rFiles = con.Client.RevertFiles(rFiles, sFlags);


                        sFlags = new Options(
                            SubmitFilesCmdFlags.SubmitShelved,
                            5,
                            null,
                            null,
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, null);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Assert.IsNotNull(sr);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        public void SubmitFilesTest3()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 3, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Test submit",
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            FileSpec fs = FileSpec.LocalSpec(Path.Combine(adminSpace, "TestData", "Letters.txt"));
                            sr = con.Client.SubmitFiles(sFlags, fs);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Assert.IsNotNull(sr);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for Parallel SubmitFiles
        ///</summary>
        [TestMethod()]
        public void ParallelSubmitFilesTestA()
        {
            ParallelSubmitFilesTest(Utilities.CheckpointType.A);
        }

        [TestMethod()]
        public void ParallelSubmitFilesTestU()
        {
            ParallelSubmitFilesTest(Utilities.CheckpointType.U);
        }
        
        public void ParallelSubmitFilesTest(Utilities.CheckpointType cptype)
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            int fileCount = 500;  // remember that this is an unlicensed server

            Process p4d = null;
            Repository rep = null;

            try
            {
                p4d = Utilities.DeployP4TestServer(TestDir, 8, cptype);
                Assert.IsNotNull(p4d, "Setup Failure");

                Server server = new Server(new ServerAddress(uri));
                rep = new Repository(server);

                string clientDir = Path.Combine(TestDir,cptype.ToString().ToLower(), "clients", ws_client);
                string syncFilesDir = Path.Combine(clientDir, "parallel");
                FileSpec parallelFileSpec = FileSpec.ClientSpec(Path.Combine(syncFilesDir, "..."));

                var parallelFileSpecArray = new FileSpec[] { parallelFileSpec };

                FileSpec parallelFileSpecZero = FileSpec.ClientSpec(Path.Combine(syncFilesDir, "..."), VersionSpec.None);

                Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                using (Connection con = rep.Connection)
                {
                    con.UserName = user;
                    con.Client = new Client();
                    con.Client.Name = ws_client;

                    Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                    Assert.AreEqual(con.Server.State, ServerState.Unknown);

                    Assert.IsTrue(con.Connect(null));

                    Assert.AreEqual(con.Server.State, ServerState.Online);

                    Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                    Assert.AreEqual("admin", con.Client.OwnerName);

                    // Set up a bunch of files in the workspace
                    PrepSyncFiles(syncFilesDir);
                    CreateSyncFiles(syncFilesDir, fileCount);

                    Changelist change = new Changelist();
                    change.Description = "On the fly built change list";
                    Changelist newChange = rep.CreateChangelist(change, null);

                    // Add them to the server
                    Options addFlags = new Options(AddFilesCmdFlags.None, newChange.Id, null);
                    IList<FileSpec> addFiles = con.Client.AddFiles(addFlags, parallelFileSpecArray);

                    Options submitFlags = new SubmitCmdOptions(
                        SubmitFilesCmdFlags.DisableParallel, newChange.Id, null, null,
                        new ClientSubmitOptions(false, SubmitType.SubmitUnchanged));

                    // submit the files
                    SubmitResults sr = con.Client.SubmitFiles(submitFlags, null);

                    // confirm that cmd flags were passed to disable parallel
                    Assert.IsNotNull(sr);
                    string[] lastResults = con.LastResults.CmdArgs;
                    Assert.IsTrue((Array.IndexOf(lastResults, "--parallel")) > -1);
                    Assert.IsTrue((Array.IndexOf(lastResults, "0")) > -1);

                    change = new Changelist();
                    change.Description = "On the fly built change list";
                    newChange = rep.CreateChangelist(change, null);

                    // move files to new change
                    con.Client.EditFiles(new EditCmdOptions(new EditFilesCmdFlags(),
                        newChange.Id, null), parallelFileSpecArray);
                    // now submit them with parallel
                    submitFlags = new SubmitCmdOptions(
                        SubmitFilesCmdFlags.None, newChange.Id, null, null,
                        new ClientSubmitOptions(true, SubmitType.SubmitUnchanged),
                        0, 4, 10, 0, 100);

                    sr = con.Client.SubmitFiles(submitFlags, null);

                    // confirm that no cmd flags were passed to disable parallel
                    Assert.IsNotNull(sr);
                    lastResults = con.LastResults.CmdArgs;
                    Assert.IsFalse((Array.IndexOf(lastResults, "--parallel")) > -1);
                    Assert.IsFalse((Array.IndexOf(lastResults, "0")) > -1);
                }
            }
            finally
            {
                Utilities.RemoveTestServer(p4d, TestDir);
                p4d?.Dispose();
                rep?.Dispose();
            }
        }

        /// <summary>
        ///A test for GetResolveFiles
        ///</summary>
        [TestMethod()]
        public void GetResolvedFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);
                        Options sFlags = new Options(
                            SubmitFilesCmdFlags.None,
                            -1,
                            null,
                            "Check It In!",
                            null
                        );
                        SubmitResults sr = null;
                        try
                        {
                            sr = con.Client.SubmitFiles(sFlags, fromFile);
                        }
                        catch
                        {
                        } // will fail because we need to resolve

                        Options rFlags = new Options(
                            ResolveFilesCmdFlags.AutomaticForceMergeMode | ResolveFilesCmdFlags.PreviewOnly, -1);
                        IList<FileResolveRecord> records = con.Client.ResolveFiles(rFlags, fromFile);
                        Assert.IsNotNull(records);

                        rFlags = new Options(
                            ResolveFilesCmdFlags.AutomaticForceMergeMode, -1);
                        records = con.Client.ResolveFiles(rFlags, fromFile);
                        Assert.IsNotNull(records);

                        Options opts = new Options(GetResolvedFilesCmdFlags.IncludeBaseRevision);
                        IList<FileResolveRecord> rFiles = con.Client.GetResolvedFiles(opts, null);

                        Assert.IsNotNull(rFiles);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for GetResolveFilesjob085495A
        ///</summary>
        [TestMethod()]
        public void GetResolvedFilesTestjob085495A()
        {
            GetResolvedFilesTestjob085495(Utilities.CheckpointType.A);
        }

        /// <summary>
        ///A test for GetResolveFiles
        ///</summary>
        public void GetResolvedFilesTestjob085495(Utilities.CheckpointType cptype)
        {
            string uri = configuration.ServerPort;;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            Process p4d = null;
            Repository rep = null;

            try
            {
                p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                Assert.IsNotNull(p4d, "Setup Failure");

                var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                var adminSpace = Path.Combine(clientRoot, "admin_space");
                Directory.CreateDirectory(adminSpace);
                Server server = new Server(new ServerAddress(uri));
                rep = new Repository(server);
                Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                using (Connection con = rep.Connection)
                {
                    con.UserName = user;
                    con.Client = new Client();
                    con.Client.Name = ws_client;
                    Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                    Assert.AreEqual(con.Server.State, ServerState.Unknown);
                    Assert.IsTrue(con.Connect(null));
                    Assert.AreEqual(con.Server.State, ServerState.Online);
                    Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                    Assert.AreEqual("admin", con.Client.OwnerName);

                    FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                        null);
                    Options sFlags = new Options(
                        SubmitFilesCmdFlags.None,
                        -1,
                        null,
                        "Check It In!",
                        null
                    );
                    SubmitResults sr = null;
                    try
                    {
                        sr = con.Client.SubmitFiles(sFlags, fromFile);
                    }
                    catch
                    {
                    } // will fail because we need to resolve

                    Options rFlags = new Options(
                        ResolveFilesCmdFlags.AutomaticForceMergeMode | ResolveFilesCmdFlags.PreviewOnly, -1);
                    IList<FileResolveRecord> records = con.Client.ResolveFiles(rFlags, fromFile);
                    Assert.IsNotNull(records);

                    rFlags = new Options(
                        ResolveFilesCmdFlags.AutomaticForceMergeMode, -1);
                    records = con.Client.ResolveFiles(rFlags, fromFile);
                    Assert.IsNotNull(records);

                    Options opts = new Options(GetResolvedFilesCmdFlags.IncludeBaseRevision);
                    IList<FileResolveRecord> rFiles = con.Client.GetResolvedFiles(opts, null);

                    Assert.IsNotNull(rFiles);
                    Assert.AreEqual(rFiles.Count, 2);
                    Assert.AreEqual(rFiles[0].FromFileSpec.Version, new VersionRange(1, 2));
                    Assert.AreEqual(rFiles[0].FromFileSpec.Version, new VersionRange(1, 2));
                    Assert.AreEqual(rFiles[1].FromFileSpec.Version, new VersionRange(1, 2));
                    Assert.AreEqual(rFiles[1].FromFileSpec.Version, new VersionRange(1, 2));
                }
            }
            finally
            {
                Utilities.RemoveTestServer(p4d, TestDir);
                p4d?.Dispose();
                rep?.Dispose();
            }
        }


        /// <summary>
        ///A test for ReconcileFiles
        ///</summary>
        [TestMethod()]
        public void ReconcileFilesTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run only once for ascii
            {
                var cptype = (Utilities.CheckpointType)i;
                
                var adminSpace = Path.Combine(Utilities.TestClientRoot(TestDir, cptype), "admin_space");
                
            Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Utilities.DeleteDirectory(adminSpace);  // get rid of workspace cruft
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep,TestDir,cptype,ws_client,false);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec folderRoot = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "...")),
                            null);
                        
                        // force sync files
                        Options sFlags = new Options(SyncFilesCmdFlags.Force, -1);
                        IList<FileSpec> syncedFiles = con.Client.SyncFiles(sFlags, folderRoot);

                        // touch files under Perforce control without opening for
                        // add, edit or delete

                        System.IO.File.SetAttributes(Path.Combine(adminSpace, "MyCode" ,"pup.txt"),
                            System.IO.File.GetAttributes(Path.Combine(adminSpace, "MyCode", "pup.txt"))
                            & ~FileAttributes.ReadOnly);
                        System.IO.File.SetAttributes(Path.Combine(adminSpace, "MyCode", "ReadMe.txt"),
                            System.IO.File.GetAttributes(Path.Combine(adminSpace, "MyCode" ,"ReadMe.txt"))
                            & ~FileAttributes.ReadOnly);
                        System.IO.File.SetAttributes(Path.Combine(adminSpace, "MyCode", "Silly.bmp"),
                            System.IO.File.GetAttributes(Path.Combine(adminSpace, "MyCode", "Silly.bmp"))
                            & ~FileAttributes.ReadOnly);

                        // edit a file
                        var lines = System.IO.File.ReadAllLines(Path.Combine(adminSpace, "MyCode", "pup.txt"));
                        lines[0] = "some value";
                        System.IO.File.WriteAllLines(Path.Combine(adminSpace, "MyCode", "pup.txt"), lines);

                        // do nothing with ReadMe.txt

                        // delete a file
                        System.IO.File.Delete(Path.Combine(adminSpace, "MyCode", "Silly.bmp"));

                        // create a file
                        System.IO.File.Create(Path.Combine(adminSpace, "MyCode", "new.txt")).Close();

                        // status check for all added files
                        sFlags = new Options(ReconcileFilesCmdFlags.NotControlled, -1);
                        IList<FileSpec> rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(1, rFiles.Count);

                        // status check for all edited files
                        sFlags = new Options(ReconcileFilesCmdFlags.ModifiedOutside, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(1, rFiles.Count);

                        // status check for all deleted files
                        sFlags = new Options(ReconcileFilesCmdFlags.DeletedLocally, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(1, rFiles.Count);

                        // status check for all files
                        sFlags = new Options(ReconcileFilesCmdFlags.None, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(4, rFiles.Count);

                        // test reconcile against all files in a directory with no changelist specified
                        sFlags = new Options(ReconcileFilesCmdFlags.Preview, -1);
                        rFiles = con.Client.ReconcileFiles(sFlags, folderRoot);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(3, rFiles.Count);

                        sFlags = new Options(ReconcileFilesCmdFlags.None, -1);
                        rFiles = con.Client.ReconcileFiles(sFlags, folderRoot);

                        sFlags = new Options(GetFileMetadataCmdFlags.None, null,null,0,
                            null,null, null);
                        syncedFiles = new List<FileSpec>();
                        syncedFiles.Add(folderRoot);

                        // get the filemetadata to confirm reconcile opened the
                        // modified files with the correct action
                        IList<FileMetaData> fmd = rep.GetFileMetaData(syncedFiles,sFlags);
                        Assert.IsNotNull(fmd);
                        Assert.AreEqual(5, fmd.Count);
                        Assert.AreEqual(fmd[0].Action, FileAction.Add);
                        Assert.AreEqual(fmd[1].Action, FileAction.Add);
                        Assert.AreEqual(fmd[2].Action, FileAction.Edit);
                        Assert.AreEqual(fmd[3].Action, FileAction.None);
                        Assert.AreEqual(fmd[4].Action, FileAction.Delete);

                        // status with -A, which should return null since all
                        // files in this directory have been reconciled and
                        // p4 status -A == p4 reconcile -e -a -d
                        sFlags = new Options(ReconcileFilesCmdFlags.NotOpened, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.IsNull(rFiles);
                    }
                }
                finally
                {
                    // delete created file
                    System.IO.File.Delete(Path.Combine(adminSpace, "MyCode", "new.txt"));

                    // set untouched file back to read only
                    System.IO.File.SetAttributes(Path.Combine(adminSpace, "MyCode", "ReadMe.txt"),
                        FileAttributes.ReadOnly);

                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for ReconcileFiles with a renamed file
        ///</summary>
        [TestMethod()]
        public void ReconcileRenamedFilesTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run only once for ascii
            {
                var cptype = (Utilities.CheckpointType)i;
                var adminSpace = Path.Combine(Utilities.TestClientRoot(TestDir, cptype), "admin_space");
                
            Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Utilities.DeleteDirectory(adminSpace);  // get rid of workspace cruft
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    
                    Utilities.SetClientRoot(rep,TestDir,cptype,ws_client,false);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec folderRoot = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "...")),
                            null);
                        // force sync files

                        Options sFlags = new Options(SyncFilesCmdFlags.Force, -1);
                        IList<FileSpec> syncedFiles = con.Client.SyncFiles(sFlags, folderRoot);

                        // touch file under Perforce control without opening for
                        // add, edit or delete
                        string readme = Path.Combine(adminSpace, "MyCode", "ReadMe.txt");
                        string renamedreadme = Path.Combine(adminSpace, "MyCode", "RenamedReadMe.txt");
                        System.IO.File.SetAttributes(readme,
                            System.IO.File.GetAttributes(readme)
                            & ~FileAttributes.ReadOnly);

                        // rename a file
                        System.IO.File.Move(readme, renamedreadme);

                        // status check for all added files
                        sFlags = new Options(ReconcileFilesCmdFlags.NotControlled, -1);
                        IList<FileSpec> rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(1, rFiles.Count);

                        // status check for all edited files
                        sFlags = new Options(ReconcileFilesCmdFlags.ModifiedOutside, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.IsNull(rFiles);

                        // status check for all deleted files
                        sFlags = new Options(ReconcileFilesCmdFlags.DeletedLocally, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(1, rFiles.Count);

                        // status check for all files
                        sFlags = new Options(ReconcileFilesCmdFlags.None, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(3, rFiles.Count);

                        // test reconcile against all files in a directory with no changelist specified
                        sFlags = new Options(ReconcileFilesCmdFlags.Preview, -1);
                        rFiles = con.Client.ReconcileFiles(sFlags, folderRoot);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(2, rFiles.Count);

                        // change folderRoot to limit to the renamed file
                        folderRoot = new FileSpec(new LocalPath(Path.Combine(adminSpace, "MyCode", "*ReadMe.txt")), null);

                        // reconcile the renamed file
                        sFlags = new Options(ReconcileFilesCmdFlags.None, -1);
                        rFiles = con.Client.ReconcileFiles(sFlags, folderRoot);

                        sFlags = new Options(GetFileMetadataCmdFlags.None, null, null, 0,
                            null, null, null);
                        syncedFiles = new List<FileSpec>();
                        syncedFiles.Add(folderRoot);

                        // get the filemetadata to confirm reconcile opened the
                        // modified files with the correct action
                        IList<FileMetaData> fmd = rep.GetFileMetaData(syncedFiles, sFlags);
                        Assert.IsNotNull(fmd);
                        Assert.AreEqual(2, fmd.Count);
                        Assert.AreEqual(fmd[0].Action, FileAction.MoveDelete);
                        Assert.AreEqual(fmd[1].Action, FileAction.MoveAdd);

                        // status with -A, which should return null since all
                        // files in this directory have been reconciled and
                        // p4 status -A == p4 reconcile -e -a -d
                        sFlags = new Options(ReconcileFilesCmdFlags.NotOpened, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.IsNull(rFiles);
                    }
                }
                finally
                {
                    // set renamed file back to read only and original name
                    System.IO.File.Move(Path.Combine(adminSpace, "MyCode", "RenamedReadMe.txt"),
                        Path.Combine(adminSpace, "MyCode", "ReadMe.txt"));

                    System.IO.File.SetAttributes(Path.Combine(adminSpace, "MyCode", "ReadMe.txt"),
                        FileAttributes.ReadOnly);

                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for ReconcileClean
        ///</summary>
        [TestMethod()]
        public void ReconcileCleanTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run only once for ascii
            {
                var cptype = (Utilities.CheckpointType)i;
            Process p4d = null;
                Repository rep = null;
                var adminSpace = Path.Combine(Utilities.TestClientRoot(TestDir, cptype), "admin_space");
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Utilities.DeleteDirectory(adminSpace);  // get rid of workspace cruft
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client, false);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec folderRoot = new FileSpec(new LocalPath(
                                Path.Combine(adminSpace, "MyCode", "...")),
                            null);

                        // force sync files
                        Options sFlags = new Options(SyncFilesCmdFlags.Force, -1);
                        IList<FileSpec> syncedFiles = con.Client.SyncFiles(sFlags, folderRoot);

                        // touch files under Perforce control without opening for
                        // add, edit or delete
                        string pup = Path.Combine(adminSpace, "MyCode", "pup.txt");
                        string readme = Path.Combine(adminSpace, "MyCode", "ReadMe.txt");
                        string silly = Path.Combine(adminSpace, "MyCode", "Silly.bmp");
                        string renamedreadme = Path.Combine(adminSpace, "MyCode", "RenamedReadMe.txt");
                        System.IO.File.SetAttributes(pup,
                            System.IO.File.GetAttributes(pup)
                            & ~FileAttributes.ReadOnly);
                        System.IO.File.SetAttributes(readme,
                            System.IO.File.GetAttributes(readme)
                            & ~FileAttributes.ReadOnly);
                        System.IO.File.SetAttributes(silly,
                            System.IO.File.GetAttributes(silly)
                            & ~FileAttributes.ReadOnly);

                        // edit a file
                        var lines = System.IO.File.ReadAllLines(pup);
                        lines[0] = "some value";
                        System.IO.File.WriteAllLines(pup, lines);

                        // rename a file
                        System.IO.File.Move(readme, renamedreadme);

                        // delete a file
                        System.IO.File.Delete(silly);

                        // create a file
                        System.IO.File.Create(Path.Combine(adminSpace, "MyCode", "new.txt")).Close();

                        // check file differences (there should be some)
                        string[] args = new string[1];
                        args[0] = folderRoot.LocalPath.ToString();
                        Options options = new Options();
                        options["-f"] = null;
                        options["-sl"] = null;
                        using (P4Command cmd = new P4Command(con, "diff", true, args))
                        {
                        P4CommandResult results = cmd.Run(options);

                        // confirm diffs on edits or deletes
                        // (diff will not check for files existing
                        // locally that are not yet added
                        string value= "";
                        results.TaggedOutput[0].TryGetValue("status", out value);
                        Assert.AreEqual(value, "diff");
                        results.TaggedOutput[1].TryGetValue("status", out value);
                        Assert.AreEqual(value, "missing");
                        results.TaggedOutput[2].TryGetValue("status", out value);
                        Assert.AreEqual(value, "missing");

                        // status check for all files
                        sFlags = new Options(ReconcileFilesCmdFlags.None, -1);
                        IList<FileSpec> rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(6, rFiles.Count);

                        // test reconcile against all files in a directory with no changelist specified
                        sFlags = new Options(ReconcileFilesCmdFlags.Preview, -1);
                        rFiles = con.Client.ReconcileFiles(sFlags, folderRoot);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(5, rFiles.Count);

                        // run p4 clean on the directory
                        sFlags = new Options(ReconcileFilesCmdFlags.None, -1);
                        rFiles = con.Client.ReconcileClean(sFlags, folderRoot);

                        // check file differences (there should be none)
                        options = new Options();
                        options["-f"] = null;
                        options["-sl"] = null;
                        results = cmd.Run(options);

                        // confirm that all files match the depot file
                        results.TaggedOutput[0].TryGetValue("status", out value);
                        Assert.AreEqual(value, "same");
                        results.TaggedOutput[1].TryGetValue("status", out value);
                        Assert.AreEqual(value, "same");
                        results.TaggedOutput[2].TryGetValue("status", out value);
                        Assert.AreEqual(value, "same");

                        // status check for all files
                        // only the 2 unadded files should be returned
                        sFlags = new Options(ReconcileFilesCmdFlags.None, -1);
                        rFiles = con.Client.ReconcileStatus(sFlags, folderRoot);
                        Assert.AreEqual(1, rFiles.Count);
                    }
                }
                }
                finally
                {
                    // delete created file
                    System.IO.File.Delete(Path.Combine(adminSpace, "MyCode", "new.txt"));

                    // set renamed file back to read only
                    System.IO.File.SetAttributes(Path.Combine(adminSpace, "MyCode", "ReadMe.txt"),
                        FileAttributes.ReadOnly);

                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for RevertFiles
        ///</summary>
        [TestMethod()]
        public void RevertFilesPreviewTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii
            {
                Utilities.CheckpointType cptype = (Utilities.CheckpointType)i;
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        // test revert against all .txt files in a directory with no changelist specified
                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);

                        // test revert against all files in changelist 5 (1 marked for add)
                        fromFile = new FileSpec(new DepotPath("//..."), null);

                        // Revert Preview
                        var sFlags = new Options(
                            RevertFilesCmdFlags.Preview,
                            5);
                        var rFiles = con.Client.RevertFiles(sFlags, fromFile);
                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(1, rFiles.Count);


                        // Revert Actual
                        sFlags = new Options(
                            RevertFilesCmdFlags.None,
                            5);
                        rFiles = con.Client.RevertFiles(sFlags, fromFile);
                        Assert.IsNotNull(rFiles); // That means In earlier "Revert Preview" nothing was reverted.
                        Assert.AreEqual(1, rFiles.Count);

                        // Verify nothing left after "Revert Actual"
                        sFlags = new Options(
                            RevertFilesCmdFlags.Preview,
                            5);
                        rFiles = con.Client.RevertFiles(sFlags, fromFile);
                        Assert.IsNull(rFiles); // That means In earlier "Revert Actual" all files in changelist got reverted.
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for RevertFiles
        ///</summary>
        [TestMethod()]
        public void RevertFilesTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii
            {
                Utilities.CheckpointType cptype = (Utilities.CheckpointType) i;
            Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        // test revert against all .txt files in a directory with no changelist specified
                        FileSpec fromFile = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*.txt")),
                            null);
                        Options sFlags = new Options(
                            RevertFilesCmdFlags.None,
                            -1
                        );
                        IList<FileSpec> rFiles = con.Client.RevertFiles(sFlags, fromFile);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(3, rFiles.Count);

                        // test revert against all files in changelist 5 (1 marked for add)
                        fromFile = new FileSpec(new DepotPath("//..."), null);
                        sFlags = new Options(
                            RevertFilesCmdFlags.None,
                            5);
                        rFiles = con.Client.RevertFiles(sFlags, fromFile);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(1, rFiles.Count);

                        con.Client.EditFiles(null, new FileSpec(new DepotPath("//depot/TestData/WingDings.txt"), null));

                        // test revert against all files in the default changelist (1 file needs to be opened now)
                        con.Client.EditFiles(null, new FileSpec(new DepotPath("//depot/TestData/WingDings.txt"), null));

                        fromFile = new FileSpec(new DepotPath("//..."), null);
                        sFlags = new Options(
                            RevertFilesCmdFlags.None,
                            0);
                        rFiles = con.Client.RevertFiles(sFlags, fromFile);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(1, rFiles.Count);

                        con.Client.EditFiles(null, new FileSpec(new DepotPath("//depot/TestData/WingDings.txt"), null));

                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for ShelveFiles
        ///</summary>
        [TestMethod()]
        public void ShelveFilesTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Utilities.CheckpointType cptype = (Utilities.CheckpointType) i;
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        Changelist change = new Changelist();
                        change.Description = "On the fly built change list";
                        FileMetaData file = new FileMetaData();
                        file.DepotPath = new DepotPath("//depot/TestData/Letters.txt");
                        change.Files.Add(file);

                        Options sFlags = new Options(
                            ShelveFilesCmdFlags.None,
                            change,
                            -1
                        );

                        IList<FileSpec> rFiles = con.Client.ShelveFiles(sFlags);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(1, rFiles.Count);

                        FileSpec fromFile = new FileSpec(new DepotPath("//depot/TestData/Numbers.txt"), null);
                        Options ops = new Options(9, null);
                        rFiles = con.Client.ReopenFiles(ops, fromFile);
                        Assert.AreEqual(1, rFiles.Count);

                        sFlags = new Options(
                            ShelveFilesCmdFlags.None,
                            null,
                            9 // created by last shelve command
                        );
                        rFiles = con.Client.ShelveFiles(sFlags, fromFile);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(1, rFiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for ShelveFiles with new options
        ///</summary>
        [TestMethod()]
        public void ShelveFilesTest2()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";


            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        Changelist change = new Changelist();
                        change.Description = "On the fly built change list";
                        FileMetaData file = new FileMetaData();
                        file.DepotPath = new DepotPath("//depot/MyCode/pup.txt");
                        FileSpec fs = new FileSpec(file.DepotPath, null, null, null);
                        EditCmdOptions editOpts = new EditCmdOptions(EditFilesCmdFlags.None,
                            -1, null);
                        rep.Connection.Client.EditFiles(editOpts, fs);
                        change.Files.Add(file);
                        rep.SaveChangelist(change, null);
                        IList<FileSpec> rFiles = new List<FileSpec>();
                        ShelveFilesCmdOptions opts = new ShelveFilesCmdOptions(
                            ShelveFilesCmdFlags.LeaveUnchanged, null, 9);
                        try
                        {
                            rFiles = con.Client.ShelveFiles(opts, fs);
                        }
                        catch (P4Exception ex)
                        {
                            Assert.AreEqual(806428054, ex.ErrorCode,
                                "No files to shelve.\n");

                        }
                        // no file should be shelved here, so the list should be null or have
                        // no items
                        Assert.IsTrue((rFiles == null) || (0 == rFiles.Count));

                        opts = new ShelveFilesCmdOptions(
                            ShelveFilesCmdFlags.SubmitUnchanged | ShelveFilesCmdFlags.Force, null, 9);

                        try
                        {
                        rFiles = con.Client.ShelveFiles(opts, fs);
                        }
                        catch (P4Exception ex)
                        {
                            string msg = ex.Message;
                            Assert.Fail();
                        }
                        

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(1, rFiles.Count);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        ///A test for Parallel ShelveFiles
        ///</summary>
        [TestMethod()]
        public void ParallelShelveFilesTestA()
        {
            ParallelShelveFilesTest(Utilities.CheckpointType.A);
        }

        [TestMethod()]
        public void ParallelShelveFilesTestU()
        {
            ParallelShelveFilesTest(Utilities.CheckpointType.U);
        }

        public void ParallelShelveFilesTest(Utilities.CheckpointType cptype)
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            int fileCount = 500;  // remember that this is an unlicensed server

            Process p4d = null;
            Repository rep = null;

            try
            {
                p4d = Utilities.DeployP4TestServer(TestDir, 8, cptype);
                Assert.IsNotNull(p4d, "Setup Failure");

                Server server = new Server(new ServerAddress(uri));
                rep = new Repository(server);

                string clientDir = Path.Combine(Utilities.TestClientRoot(TestDir, cptype), ws_client);
                string syncFilesDir = Path.Combine(clientDir, "parallel");
                FileSpec parallelFileSpec = FileSpec.ClientSpec(Path.Combine(syncFilesDir, "..."));

                var parallelFileSpecArray = new FileSpec[] { parallelFileSpec };

                FileSpec parallelFileSpecZero = FileSpec.ClientSpec(Path.Combine(syncFilesDir, "..."), VersionSpec.None);

                Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                using (Connection con = rep.Connection)
                {
                    
                    con.UserName = user;
                    con.Client = new Client();
                    con.Client.Name = ws_client;

                    Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                    Assert.AreEqual(con.Server.State, ServerState.Unknown);

                    Assert.IsTrue(con.Connect(null));

                    Assert.AreEqual(con.Server.State, ServerState.Online);

                    Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                    Assert.AreEqual("admin", con.Client.OwnerName);

                    // Set up a bunch of files in the workspace
                    PrepSyncFiles(syncFilesDir);
                    CreateSyncFiles(syncFilesDir, fileCount);

                    Changelist change = new Changelist();
                    change.Description = "On the fly built change list";
                    Changelist newChange = rep.CreateChangelist(change, null);

                    // Add them to the server
                    Options addFlags = new Options(AddFilesCmdFlags.None, newChange.Id, null);
                    IList<FileSpec> addFiles = con.Client.AddFiles(addFlags, parallelFileSpecArray);


                    Options sFlags = new ShelveFilesCmdOptions(
                        ShelveFilesCmdFlags.DisableParallel, null,
                        newChange.Id);

                    IList<FileSpec> rFiles = con.Client.ShelveFiles(sFlags);

                    // confirm that cmd flags were passed to disable parallel
                    string[] lastResults = con.LastResults.CmdArgs;
                    Assert.IsTrue((Array.IndexOf(lastResults, "--parallel")) > -1);
                    Assert.IsTrue((Array.IndexOf(lastResults, "0")) > -1);

                    // delete the shelved files
                    sFlags = new ShelveFilesCmdOptions(
                        ShelveFilesCmdFlags.Delete, null
                        ,
                        newChange.Id);
                    con.Client.ShelveFiles(sFlags);

                    // now shelve them with parallel
                    sFlags = new ShelveFilesCmdOptions(
                        ShelveFilesCmdFlags.None, null
                        ,
                        newChange.Id, 0, 4, 10, 0, 100
                                    );

                    rFiles = con.Client.ShelveFiles(sFlags);

                    // confirm that no cmd flags were passed to disable parallel
                    lastResults = con.LastResults.CmdArgs;
                    Assert.IsFalse((Array.IndexOf(lastResults, "--parallel")) > -1);
                    Assert.IsFalse((Array.IndexOf(lastResults, "0")) > -1);
                }
            }
            finally
            {
                Utilities.RemoveTestServer(p4d, TestDir);
                p4d?.Dispose();
                rep?.Dispose();
            }
        }

        /// <summary>
        ///A test for SyncFiles
        ///</summary>
        [TestMethod()]
        public void SyncFilesTest()
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "Alex";
            string pass = string.Empty;
            string ws_client = "alex_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
    		    p4d = Utilities.DeployP4TestServer(TestDir, 8, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

				    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client, false);

					using (Connection con = rep.Connection)
					{
						con.UserName = user;
						con.Client = new Client();
						con.Client.Name = ws_client;
						Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

						Assert.AreEqual(con.Server.State, ServerState.Unknown);

						Assert.IsTrue(con.Connect(null));

						Assert.AreEqual(con.Server.State, ServerState.Online);

						Assert.AreEqual(con.Status, ConnectionStatus.Connected);

						Assert.AreEqual("Alex", con.Client.OwnerName);

						FileSpec fromFile = new FileSpec(new DepotPath("//depot/..."), null);

						Options sFlags = new Options(
							SyncFilesCmdFlags.Preview,
							100
						);

						IList<FileSpec> rFiles = con.Client.SyncFiles(sFlags, fromFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(6, rFiles.Count);

						fromFile = new FileSpec(new DepotPath("//depot/MyCode2/*"), null);

						sFlags = new Options(
							SyncFilesCmdFlags.Force,
							1
						);

						rFiles = con.Client.SyncFiles(sFlags, fromFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(1, rFiles.Count);
					}
				}
				finally
				{
					Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
				}
                cptype = Utilities.CheckpointType.U;
			}
		}

        /// <summary>
        ///A test for Parallel Sync
        ///</summary>
        [TestMethod()]
        public void ParallelSyncFilesTestA()
        {
            ParallelSyncFilesTest(Utilities.CheckpointType.A);
        }

        /// <summary>
        ///A test for Parallel Sync
        ///</summary>
        [TestMethod()]
        public void ParallelSyncFilesTestU()
        {
            ParallelSyncFilesTest(Utilities.CheckpointType.U);
        }

        public void ParallelSyncFilesTest(Utilities.CheckpointType cptype)
        {
            string uri = configuration.ServerPort;
            string user = "Alex";
            string pass = string.Empty;
            string ws_client = "alex_space";

            int fileCount = 500;  // remember that this is an unlicensed server

            Process p4d = null;
            Repository rep = null;

                try
                {
#if ! _WINDOWS
                // Avoid something bad going on with GetCharSet()
                if (cptype == Utilities.CheckpointType.U)
                    P4Server.Update("P4CHARSET", "utf8");
#endif 
                p4d = Utilities.DeployP4TestServer(TestDir, 8, cptype);
                Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                rep = new Repository(server);

                string clientDir = Path.Combine(Utilities.TestClientRoot(TestDir, cptype), ws_client);
                    string syncFilesDir = Path.Combine(clientDir, "parallel");
                FileSpec parallelFileSpec = FileSpec.ClientSpec(Path.Combine(syncFilesDir, "..."));

                    var parallelFileSpecArray = new FileSpec[] { parallelFileSpec };

                FileSpec parallelFileSpecZero = FileSpec.ClientSpec(Path.Combine(syncFilesDir, "..."), VersionSpec.None);
                Client c;
                  
               // Create / Update client 
               using (Connection con1 = rep.Connection)
               {
                   con1.UserName = "admin";
                   con1.Connect(null);

                   c = rep.GetClient(ws_client, null);
                   c.Root = clientDir;
                   c.OwnerName = user;
                   c.ViewMap = new ViewMap(new string[]
                   {
                       "	//depot/parallel/... //alex_space/parallel/..."
                   });
                   rep.UpdateClient(c);
                   rep.Connection.Server.SetState(ServerState.Unknown);
               }

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                    con.Client = c;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("Alex", con.Client.OwnerName);

                        // Set up a bunch of files in the workspace
                        PrepSyncFiles(syncFilesDir);
                        CreateSyncFiles(syncFilesDir, fileCount);

                        // Add them to the server
                        Options addFlags = new Options(AddFilesCmdFlags.None, -1, null);
                        IList<FileSpec> addFiles = con.Client.AddFiles(addFlags, parallelFileSpecArray);

                        // Submit them
                        Options submitFlags = new Options(SubmitFilesCmdFlags.None,-1, null,"initial submit test files", null);
                        con.Client.SubmitFiles(submitFlags, parallelFileSpec);

                        // Now Sync them all to NONE
                        Options syncFlags = new Options(SyncFilesCmdFlags.Quiet);
                        IList<FileSpec> syncFiles = con.Client.SyncFiles(syncFlags, parallelFileSpecZero);

                        bool setRv = P4ConfigureSetParallel(con, 4);
                        Assert.IsTrue(setRv);
			           
                        // Finally, we Sync them again using parallel.
                        Options pFlags = new SyncFilesCmdOptions(SyncFilesCmdFlags.None, 0, 4, 10, 0, 100);
                        IList<FileSpec> pFiles = con.Client.SyncFiles(pFlags, parallelFileSpecArray);

                        Assert.IsNotNull(pFiles);
                        Assert.AreEqual(fileCount, pFiles.Count);
                    }
                }
                catch (P4Exception ex)
                {
                    logger.Error("ParallelSyncFilesTest " + ex.Message, ex);
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                p4d?.Dispose();
                rep?.Dispose();
                }
        }

        /// <summary>
        /// Create random byte array for file contents
        /// </summary>
        /// <param name="i">seed</param>
        /// <returns>contents</returns>
        private static byte[] GetFileContent(int i)
        {
            Random r = new Random(i);
            byte[] buffer = new byte[1024];
            r.NextBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Create a lot of random files
        /// </summary>
        /// <param name="folder">folder for files</param>
        /// <param name="count">number of files to create</param>
        private static void CreateSyncFiles(string folder, int count)
        {
            var sw = new Stopwatch();
            sw.Start();

            Parallel.For(0, count, (i) =>
            {
                string path = Path.Combine(folder, string.Format("file{0}.dat", i));
                System.IO.File.WriteAllBytes(path, GetFileContent(i));
            });
            Console.WriteLine("Create {0} files, Time: {1}ms", count, sw.ElapsedMilliseconds);
        }

        private static void PrepSyncFiles(string folder)
        {
            if (Directory.Exists(folder))
            {
                var directory = new DirectoryInfo(folder) { Attributes = FileAttributes.Normal };
                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }
                directory.Delete(true);
            }
            Directory.CreateDirectory(folder);
        }

        /// <summary>
        /// Configure the server to use N threads
        /// Log in as "admin" and run "p4 configure set net.parallel.max=N"
        /// </summary>
        /// <param name="con">Connection to server</param>
        /// <param name="threads">Max number of parallel threads</param>
        /// <returns>true if no error</returns>
        private bool P4ConfigureSetParallel(Connection con, int threads)
        {
            string oldUser = con.UserName;
            con.UserName = "admin";
            con.Login("");

            // Tell the server to support parallel
            string[] args = {"set", "net.parallel.max=" + threads };
            using (var cmd = new P4Command(con, "configure", false, args))
            {
			var cmdr = cmd.Run(new Options());
            // now force a reconnect, or p4d might not use the new config
            con.getP4Server().Reconnect();

            con.UserName = oldUser;
            return cmdr.Success;
        }
        }

        private bool P4Configure(Connection con, string[] args)
        {
            string oldUser = con.UserName;
            con.UserName = "admin";
            con.Login("");

            using (var cmd = new P4Command(con, "configure", false, args))
            {
            var cmdr = cmd.Run(new Options());

            con.UserName = oldUser;
            return cmdr.Success;
        }
        }

        /// <summary>
        ///A test for Parallel Sync error handling
        /// Job085941
        ///</summary>
        [TestMethod()]
        public void ParallelSyncJob085941A()
        {
            ParallelSyncJob085941(Utilities.CheckpointType.A);
        }

        /// <summary>
        /// A test for Parallel Sync error handling (don't run for unicode, u.tar lacks an alex_space that the test depends on)
        /// Job085941
        ///</summary>
        /*
        [TestMethod()]
        public void ParallelSyncJob085941U()
        {
            // ParallelSyncJob085941(Utilities.CheckpointType.U);
        }
        */

        public void ParallelSyncJob085941(Utilities.CheckpointType cptype)
        {
            string uri = configuration.ServerPort;
            string user1 = "Alex";
            string pass = string.Empty;
            string ws_client1 = "alex_space";
            string user2 = "Alice";
            string ws_client2 = "alice_space";

            string targetFile = "file59.dat";

            int fileCount = 500;  // remember that this is an unlicensed server

            Process p4d = null;
            Repository rep = null;
            Connection con = null;
            P4CommandResult cmdResult;

                try
                {
                string clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                
                p4d = Utilities.DeployP4TestServer(TestDir, 8, cptype);
                Assert.IsNotNull(p4d, "Setup Failure");
                
                    Server server = new Server(new ServerAddress(uri));
                rep = new Repository(server);

                    // Information about Alex's workspace
                Utilities.SetClientRoot(rep, TestDir, cptype, ws_client1, false);
                string clientDir1 = Path.Combine(clientRoot, ws_client1);
                    string syncFilesDir1 = Path.Combine(clientDir1, "parallel");
                    string testFile1 = Path.Combine(syncFilesDir1, targetFile);
                FileSpec parallelFileSpec1 = FileSpec.ClientSpec(Path.Combine(syncFilesDir1, "..."));
                    var parallelFileSpecArray1 = new FileSpec[] { parallelFileSpec1 };
                FileSpec parallelFileSpecZero1 = FileSpec.ClientSpec(Path.Combine(syncFilesDir1, "..."), VersionSpec.None);
                    FileSpec testFileSpec1 = FileSpec.ClientSpec(testFile1);
                    var testFileSpecArray1 = new FileSpec[] { testFileSpec1 };

                    // Information about Alice's workspace
                Utilities.SetClientRoot(rep, TestDir, cptype, ws_client2, false);
                string clientDir2 = Path.Combine(clientRoot, ws_client2);
                    string syncFilesDir2 = Path.Combine(clientDir2, "parallel");
                    string testFile2 = Path.Combine(syncFilesDir2, targetFile);
                FileSpec parallelFileSpec2 = FileSpec.ClientSpec(Path.Combine(syncFilesDir2, "..."));
                    FileSpec testFileSpec2 = FileSpec.ClientSpec(testFile2);
                    var testFileSpecArray2 = new FileSpec[] { testFileSpec2 };

                    using (con = rep.Connection)
                    {
                        con.UserName = user1;
                        con.Client = new Client {Name = ws_client1};

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("Alex", con.Client.OwnerName);

                        // Set up a bunch of files in the workspace
                        PrepSyncFiles(syncFilesDir1);
                        CreateSyncFiles(syncFilesDir1, fileCount);

                        // Add them to the server
                        Options addFlags = new Options(AddFilesCmdFlags.None, -1, null);
                        IList<FileSpec> addFiles = con.Client.AddFiles(addFlags, parallelFileSpecArray1);

                        // Submit them
                        Options submitFlags = new Options(SubmitFilesCmdFlags.None, -1, null, "initial submit test files", null);
                        con.Client.SubmitFiles(submitFlags, parallelFileSpec1);

                        // Remove all Workspace Files (Sync to NONE)
                        Options syncFlags = new Options(SyncFilesCmdFlags.Quiet);
                        IList<FileSpec> syncFiles = con.Client.SyncFiles(syncFlags, parallelFileSpecZero1);

                        // Sync just the target file
                        syncFlags = new Options(SyncFilesCmdFlags.Quiet | SyncFilesCmdFlags.Force);
                        IList<FileSpec> syncOneFile = con.Client.SyncFiles(syncFlags, testFileSpecArray1);

                        // Change Target file to Read Only
                        var fileInfo = new System.IO.FileInfo(testFile1){ IsReadOnly = false };
                       
                        // Now set up the other user / client
                        con.UserName = user2;
                        con.Client = new Client { Name = ws_client2 };

                        // Sync just the target file
                        syncFlags = new Options(SyncFilesCmdFlags.Quiet | SyncFilesCmdFlags.Force);
                        IList<FileSpec> syncOneFile2 = con.Client.SyncFiles(syncFlags, testFileSpecArray2);

                        // Edit the TargetFile
                        Options editFlags = new Options();
                        IList<FileSpec> editFiles = con.Client.EditFiles(editFlags, testFileSpecArray2 );
                        System.IO.File.WriteAllBytes(testFile2, GetFileContent(99));

                        // Submit the targetfile2
                        submitFlags = new Options(SubmitFilesCmdFlags.None, -1, null, "update test file", null);
                        con.Client.SubmitFiles(submitFlags, parallelFileSpec2);

                        // Back to the original user / workspace
                        con.UserName = user1;
                        con.Client = new Client { Name = ws_client1 };

                         // Add debugging to server log
                         string[] cargs = new string[]{"set", "dmc=3"};
                         P4Configure(con, cargs);

                         bool setRv = P4ConfigureSetParallel(con, 4);
                         Assert.IsTrue(setRv);

                        // Finally, we Sync them again using parallel.
                        Options pFlags = new SyncFilesCmdOptions(SyncFilesCmdFlags.None, 0, 4, 8, 2000, 9, 2000);
                        IList<FileSpec> pFiles = con.Client.SyncFiles(pFlags, parallelFileSpecArray1);

                        cmdResult = con.LastResults;

                        Assert.IsNotNull(pFiles);
                        Assert.AreEqual(fileCount, pFiles.Count);
                    }
                }
                catch (P4Exception ex)
                {
                    logger.Error("ParallelSyncJob085941: " + ex.Message, ex);
                    Assert.AreEqual(0, ex.ErrorCode, "Error undetected during parallel sync\n");
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                p4d?.Dispose();
                rep?.Dispose();
                }
            }

        /// <summary>
        ///A test for SyncFiles using a label
        ///</summary>
        [TestMethod()]
        public void SyncToLabelTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
            Process p4d = null;
                Repository rep = null;
                Utilities.CheckpointType cptype = (Utilities.CheckpointType) i;

                try 
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 8, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

                        Assert.AreEqual(con.Server.State, ServerState.Unknown);

                        Assert.IsTrue(con.Connect(null));

                        Assert.AreEqual(con.Server.State, ServerState.Online);

                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);

                        Assert.AreEqual("admin", con.Client.OwnerName);

                        FileSpec fromFile = new FileSpec(new DepotPath("//depot/..."), new LabelNameVersion("admin_label"));

                        Options sFlags = new Options(
                            SyncFilesCmdFlags.Preview,
                            100
                        );

                        IList<FileSpec> rFiles = con.Client.SyncFiles(sFlags, fromFile);

                        Assert.IsNotNull(rFiles);
                        if (i == 0)
                        {
                            Assert.AreEqual(10, rFiles.Count);
                        }
                        else
                        {
                            Assert.AreEqual(6, rFiles.Count);
                        }
                        IList<FileMetaData> fmd = rep.GetFileMetaData(null, fromFile);
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for SyncParallelErrorjob095320
        ///</summary>
        [TestMethod()]
        public void SyncParallelErrorTestjob095320()
        {
            var cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            Process p4d = null;
            Repository rep = null;

            try
            {
                p4d = Utilities.DeployP4TestServer(TestDir, 13, cptype);
                Assert.IsNotNull(p4d, "Setup Failure");

                Server server = new Server(new ServerAddress(uri));
                rep = new Repository(server);

                Utilities.SetClientRoot(rep, TestDir, cptype, ws_client,false);

                using (Connection con = rep.Connection)
                {
                con.UserName = user;
                con.Client = new Client();
                con.Client.Name = ws_client;

                con.Connect(null);

                FileSpec fs = new FileSpec();
                fs.DepotPath = new DepotPath("//depot/...");

                Options sFlags = new Options(
                    SyncFilesCmdFlags.DisableParallel,
                    100
                );

                // Some of the files we need to work with are
                // checked out, so revert them.
                IList<FileSpec> rFiles = con.Client.RevertFiles(null, fs);

                // Make one of the files at Rev 1 of 2 writeable
                System.IO.File.SetAttributes(rFiles[5].LocalPath.Path, FileAttributes.Normal);
                // modify the file
                System.IO.File.WriteAllText(rFiles[5].LocalPath.Path, DateTime.Now.ToLongDateString());

                // Now attempt to sync that file
                try
                {
                    var resp = rep.Connection.Client.SyncFiles(sFlags, new FileSpec(new DepotPath(@"//depot/TestData/Numbers.txt")));
                }
                catch (Exception e)
                {
                    // This sync should fail on a "can't clobber" error
                    Assert.IsTrue(e.Message.Contains("Can't clobber writable file"));
                }

                // Confirm the other reverted file to still be at rev 1
                IList<FileMetaData> fmd = rep.GetFileMetaData(null, new FileSpec(new DepotPath(@"//depot/TestData/WingDings.txt")));
                    Assert.AreEqual("1",fmd[0].HaveRev.ToString());

                try
                {
                    rep.Connection.Client.SyncFiles(sFlags, new FileSpec(new DepotPath(@"//depot/TestData/WingDings.txt")));
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("parallel"))
                    {
                            throw;
                    }
                }

                // Confirm the other reverted file to be at rev 2 (sync succeeded)
                fmd = rep.GetFileMetaData(null, new FileSpec(new DepotPath(@"//depot/TestData/WingDings.txt")));
                    Assert.AreEqual("2",fmd[0].HaveRev.ToString());

                // now disconnect since this is not wrapped in a using statement
                con.Disconnect();
            }
            }
            finally
            {
                Utilities.RemoveTestServer(p4d, TestDir);
                p4d?.Dispose();
                rep?.Dispose();
            }
        }

        /// <summary>
        ///A test for SyncFiles syncing a file that has been deleted from the depot
        ///</summary>
        [TestMethod()]
        public void SyncDeletedFileTest()
        {
            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";

            for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
            {
                var cptype = (Utilities.CheckpointType)i;
            Process p4d = null;
                Repository rep = null;

                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 8, cptype);  
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;
                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual(user, con.Client.OwnerName);

                        // mark the file for delete and submit the change
                        FileSpec fromFile = new FileSpec(new DepotPath("//depot/Modifiers/Silly.bmp"), null);
                        FileSpec toFile = new FileSpec(new DepotPath("//depot/Modifiers/Silly.bmp"), new Revision(1));
                        con.Client.DeleteFiles(null, fromFile);
                        Options submitOpts = new Options(SubmitFilesCmdFlags.None, -1, null, "Delete a file",
                            null);
                        con.Client.SubmitFiles(submitOpts, fromFile);

                        // sync to a version of the file that was note deleted
                        IList<FileSpec> rFiles = con.Client.SyncFiles(null, toFile);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(1, rFiles.Count);

                        // sync to the head version (deleted)
                        rFiles = con.Client.SyncFiles(null, fromFile);

                        Assert.IsNotNull(rFiles);
                        Assert.AreEqual(1, rFiles.Count);

                        // make sure the file was removed from the workspace
                        Assert.IsFalse(System.IO.File.Exists(Path.Combine(adminSpace, "Modifiers", "Silly.bmp")));
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }
            }
        }

        /// <summary>
        ///A test for UnlockFiles
        ///</summary>
        [TestMethod()]
		public void UnlockFilesTest()
		{
                string uri = configuration.ServerPort;
			string user = "admin";
			string pass = string.Empty;
			string ws_client = "admin_space";

                for (int i = 0; i < 2; i++) // run once for ascii, once for unicode
                {
                    var cptype = (Utilities.CheckpointType)i;
		    Process p4d = null;
                    Repository rep = null;

                try {
			p4d = Utilities.DeployP4TestServer(TestDir, 5, cptype);
                        Assert.IsNotNull(p4d, "Setup Failure");
                        
                        var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
				    Server server = new Server(new ServerAddress(uri));
                        rep = new Repository(server);

					using (Connection con = rep.Connection)
					{
						con.UserName = user;
						con.Client = new Client();
						con.Client.Name = ws_client;

						Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

						Assert.AreEqual(con.Server.State, ServerState.Unknown);

						Assert.IsTrue(con.Connect(null));

						Assert.AreEqual(con.Server.State, ServerState.Online);

						Assert.AreEqual(con.Status, ConnectionStatus.Connected);

						Assert.AreEqual("admin", con.Client.OwnerName);

						IList<FileSpec> oldfiles = con.Client.LockFiles(null);

						Assert.AreNotEqual(null, oldfiles);

						oldfiles = con.Client.UnlockFiles(null);

						Assert.AreNotEqual(null, oldfiles);
					}
				}
				finally
				{
					Utilities.RemoveTestServer(p4d, TestDir);
                        p4d?.Dispose();
                        rep?.Dispose();
				}
			}
		}

		/// <summary>
		///A test for UnshelveFiles
		///</summary>
		[TestMethod()]
		public void UnshelveFilesTest()
		{
            string uri = configuration.ServerPort;
			string user = "admin";
			string pass = string.Empty;
			string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                var cptype = (Utilities.CheckpointType)i;
		    Process p4d = null;
                Repository rep = null;

			    try
			    {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype); 
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);
                    
				    Server server = new Server(new ServerAddress(uri));
		    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

					using (Connection con = rep.Connection)
					{
						con.UserName = user;
						con.Client = new Client();
						con.Client.Name = ws_client;
						Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

						Assert.AreEqual(con.Server.State, ServerState.Unknown);

						Assert.IsTrue(con.Connect(null));

						Assert.AreEqual(con.Server.State, ServerState.Online);

						Assert.AreEqual(con.Status, ConnectionStatus.Connected);

						Assert.AreEqual("admin", con.Client.OwnerName);

						Changelist change = new Changelist();
						change.Description = "On the fly built change list";
						FileMetaData file = new FileMetaData();
						file.DepotPath = new DepotPath("//depot/TestData/Letters.txt");
						change.Files.Add(file);

						Options sFlags = new Options(
							ShelveFilesCmdFlags.None,
							change,
							-1
						);

						IList<FileSpec> rFiles = con.Client.ShelveFiles(sFlags);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(1, rFiles.Count);

						FileSpec fromFile = new FileSpec(new DepotPath("//depot/TestData/Numbers.txt"), null);
						Options ops = new Options(9, null);
						rFiles = con.Client.ReopenFiles(ops, fromFile);
						Assert.AreEqual(1, rFiles.Count);

						sFlags = new Options(
							ShelveFilesCmdFlags.None,
							null,
							9   // created by last shelve command
						);
						rFiles = con.Client.ShelveFiles(sFlags, fromFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(1, rFiles.Count);

    			FileSpec revertFiles = new FileSpec(new LocalPath(Path.Combine(adminSpace, "TestData", "*")), null);
						Options rFlags = new Options(
							RevertFilesCmdFlags.None,
							9
						);
						rFiles = con.Client.RevertFiles(rFlags, revertFiles);

						Options uFlags =
							new Options(UnshelveFilesCmdFlags.None, 9, -1);

						rFiles = con.Client.UnshelveFiles(uFlags, fromFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(1, rFiles.Count);

						rFiles = con.Client.UnshelveFiles(uFlags);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(1, rFiles.Count);
					}
				}
				finally
				{
					Utilities.RemoveTestServer(p4d, TestDir);
                p4d?.Dispose();
                rep?.Dispose();
				}
			}
		}

		/// <summary>
		///A test for GetClientFileMappings
		///</summary>
		[TestMethod()]
		public void GetClientFileMappingsTest()
		{
            string uri = configuration.ServerPort;
			string user = "admin";
			string pass = string.Empty;
			string ws_client = "admin_space";

            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
		    Process p4d = null;
                Repository rep = null;
                var cptype = (Utilities.CheckpointType) i;

                try 
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

				    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);

					using (Connection con = rep.Connection)
					{
						con.UserName = user;
						con.Client = new Client();
						con.Client.Name = ws_client;
						Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

						Assert.AreEqual(con.Server.State, ServerState.Unknown);

						Assert.IsTrue(con.Connect(null));

						Assert.AreEqual(con.Server.State, ServerState.Online);

						Assert.AreEqual(con.Status, ConnectionStatus.Connected);

						Assert.AreEqual("admin", con.Client.OwnerName);

						FileSpec fromFile = new FileSpec(new DepotPath("//depot/TestData/Numbers.txt"), null);
						
						IList<FileSpec> rFiles = con.Client.GetClientFileMappings(fromFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(1, rFiles.Count);
					}
				}
				finally
				{
					Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
				}
			}
		}

		/// <summary>
		///A test for CopyFiles
		///</summary>
		[TestMethod()]
		public void CopyFilesTest()
		{
            string uri = configuration.ServerPort;
			string user = "admin";
			string pass = string.Empty;
			string ws_client = "admin_space";


			for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
			{
                Process p4d = null;
                Repository rep = null;
                Utilities.CheckpointType cptype = (Utilities.CheckpointType) i;
                
                try
                {
		    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

				    Server server = new Server(new ServerAddress(uri));
		    rep = new Repository(server);

                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);
                    
					using (Connection con = rep.Connection)
					{
						con.UserName = user;
						con.Client = new Client();
						con.Client.Name = ws_client;
						Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

						Assert.AreEqual(con.Server.State, ServerState.Unknown);

						Assert.IsTrue(con.Connect(null));

						Assert.AreEqual(con.Server.State, ServerState.Online);

						Assert.AreEqual(con.Status, ConnectionStatus.Connected);

						Assert.AreEqual("admin", con.Client.OwnerName);

						FileSpec fromFile = new FileSpec(new DepotPath("//depot/TestData/Numbers.txt"), null);
						FileSpec toFile = new FileSpec(new DepotPath("//depot/TestData42/Numbers.txt"), null);

						IList<FileSpec> rFiles = con.Client.CopyFiles(null, fromFile, toFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(1, rFiles.Count);

						fromFile = new FileSpec(new DepotPath("//depot/TestData/*"), null);
						toFile = new FileSpec(new DepotPath("//depot/TestData44/*"), null);

						Options cFlags = new Options(
							CopyFilesCmdFlags.Virtual, null, null, null, -1, 2
							);
						rFiles = con.Client.CopyFiles(cFlags, fromFile, toFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(2, rFiles.Count);
					}
				}
				finally
				{
					Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
				}
			}
		}

		/// <summary>
		///A test for MergeFiles
		///</summary>
		[TestMethod()]
		public void MergeFilesTest()
		{
            string uri = configuration.ServerPort;
			string user = "admin";
			string pass = string.Empty;
			string ws_client = "admin_space";

			for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
			{
                Utilities.CheckpointType cptype = (Utilities.CheckpointType) i;
                Process p4d = null;
                Repository rep = null;
                try
                {
		    p4d = Utilities.DeployP4TestServer(TestDir, 2, cptype);
                    Assert.IsNotNull(p4d, "Setup Failure");

				    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);

					using (Connection con = rep.Connection)
					{
						con.UserName = user;
						con.Client = new Client();
						con.Client.Name = ws_client;
						Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);

						Assert.AreEqual(con.Server.State, ServerState.Unknown);

						Assert.IsTrue(con.Connect(null));

						Assert.AreEqual(con.Server.State, ServerState.Online);

						Assert.AreEqual(con.Status, ConnectionStatus.Connected);

						Assert.AreEqual("admin", con.Client.OwnerName);

						FileSpec fromFile = new FileSpec(new DepotPath("//depot/TestData/Numbers.txt"), null);
						FileSpec toFile = new FileSpec(new DepotPath("//depot/TestData42/Numbers.txt"), null);

						IList<FileSpec> rFiles = con.Client.MergeFiles(null, fromFile, toFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(1, rFiles.Count);

						fromFile = new FileSpec(new DepotPath("//depot/TestData/*"), null);
						toFile = new FileSpec(new DepotPath("//depot/TestData44/*"), null);

						Options cFlags = new Options(
							MergeFilesCmdFlags.Force, null, null, null, -1, 2
							);
						rFiles = con.Client.MergeFiles(cFlags, fromFile, toFile);

						Assert.IsNotNull(rFiles);
						Assert.AreEqual(2, rFiles.Count);
					}
				}
				finally
				{
					Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
				}
			}
		}

        /// <summary>
        /// Test creation of new attribute on file and displays attributes using fstat command to check usage of new configurable trait.storagedepot.min.
        /// </summary>
        [TestMethod()]
        [DynamicData(nameof(GetAttributeAndFstatTestData), DynamicDataSourceType.Method)]
        public void AttributeAndFstatTest(int config_trait_storagedepot_min, string attributeName, object attributeValue, GetFileMetadataCmdFlags getFileMetadataCmdFlags, string expectedValue, bool useDbTraitTable = false)
        {
            Utilities.CheckpointType cptype = Utilities.CheckpointType.A;

            string uri = configuration.ServerPort;
            string user = "admin";
            string pass = string.Empty;
            string ws_client = "admin_space";


            for (int i = 0; i < 1; i++) // run once for ascii, once for unicode
            {
                Process p4d = null;
                Repository rep = null;
                try
                {
                    p4d = Utilities.DeployP4TestServer(TestDir, cptype, TestContext.TestName);
                    Assert.IsNotNull(p4d, "Setup Failure");

                    var clientRoot = Utilities.TestClientRoot(TestDir, cptype);
                    var adminSpace = Path.Combine(clientRoot, "admin_space");
                    Directory.CreateDirectory(adminSpace);

                    Server server = new Server(new ServerAddress(uri));
                    rep = new Repository(server);
                    Utilities.SetClientRoot(rep, TestDir, cptype, ws_client);

                    using (Connection con = rep.Connection)
                    {
                        con.UserName = user;
                        con.Client = new Client();
                        con.Client.Name = ws_client;

                        Assert.AreEqual(con.Status, ConnectionStatus.Disconnected);
                        Assert.AreEqual(con.Server.State, ServerState.Unknown);
                        Assert.IsTrue(con.Connect(null));
                        Assert.AreEqual(con.Server.State, ServerState.Online);
                        Assert.AreEqual(con.Status, ConnectionStatus.Connected);
                        Assert.AreEqual("admin", con.Client.OwnerName);

                        var codePath = Path.Combine(clientRoot, "admin_space", "MyCode");

                        Directory.CreateDirectory(codePath);

                        FileSpec fs = FileSpec.DepotSpec("//depot/MyCode/ReadMe.txt");

                        bool setRv;

                        if (config_trait_storagedepot_min >= 0)
                        {
                            logger.Debug("Set new configurable trait.storagedepot.min");
                            setRv = P4ConfigureSetAttributeStorageConfigurable(con, config_trait_storagedepot_min);
                            Assert.IsTrue(setRv);
                        }

                        logger.Debug("Checkout file {0} to set attribute.", fs.DepotPath.Path);

                        using (P4Command editCmd = new P4Command(rep, "edit", true, fs.DepotPath.Path))
                        {

                            P4CommandResult attributeResult = editCmd.Run();

                        }

                        logger.Debug("Adding new Attribute to the file {0}", fs.DepotPath.Path);

                        using (P4Command attributeCmd = new P4Command(rep, "attribute", true, fs.DepotPath.Path))
                        {

                            Options opts = new Options();
                            opts["-f"] = null;
                            opts["-n"] = attributeName;
                            opts["-v"] = attributeValue.ToString();

                            if (useDbTraitTable)
                            {
                                opts["-T0"] = null;
                            }

                            if (config_trait_storagedepot_min < 0)
                            {
                                opts["-T1"] = null;
                            }

                            string value = "";

                            try
                            {
                                //run command and get results
                                P4CommandResult results = attributeCmd.Run(opts);
                                results.TaggedOutput[0].TryGetValue("status", out value);
                            }
                            catch (Exception ex)
                            {
                                // Check if we got expected exception while trying to add attribute using -T1 flag without setting new configurable. 
                                Assert.AreEqual(ex.Message, expectedValue);
                            }

                            if (config_trait_storagedepot_min >= 0)
                            {
                                // confirm if attribute is added successfully to the file.
                                Assert.AreEqual(value, "set");
                                Options ops = new Options();
                                switch (getFileMetadataCmdFlags)
                                {
                                    case GetFileMetadataCmdFlags.Attributes:
                                        ops["-Oa"] = null;
                                        break;

                                    case GetFileMetadataCmdFlags.AttributesProp:
                                        ops["-On"] = null;
                                        break;
                                }

                                IList<FileMetaData> actual = rep.GetFileMetaData(ops, fs);

                                Assert.AreEqual(true, actual?.Count > 0);
                                object attributeActualValue;
                                Assert.IsTrue(actual[0].Attributes.ContainsKey(attributeName));
                                Assert.IsTrue(actual[0].Attributes.TryGetValue(attributeName, out attributeActualValue));

                                switch (getFileMetadataCmdFlags)
                                {
                                    // -Oa flag
                                    case GetFileMetadataCmdFlags.Attributes:
                                        Assert.AreEqual(opts["-v"], attributeActualValue);
                                        break;

                                    // -On flag 
                                    case GetFileMetadataCmdFlags.AttributesProp:
                                        Assert.AreEqual(expectedValue, attributeActualValue);
                                        break;
                                }
                            }
                            logger.Debug("Finished fstat");
                        }
                    }
                }
                finally
                {
                    Utilities.RemoveTestServer(p4d, TestDir);
                    p4d?.Dispose();
                    rep?.Dispose();
                }

                cptype = Utilities.CheckpointType.U;
            }
        }

        /// <summary>
        /// Configure the server to set trait.storagedepot.min
        /// Log in as "admin" and run "p4 configure set trait.storagedepot.min = value"
        /// </summary>
        /// <param name="con">Connection to server</param>
        /// <param name="value">If set to a value greater than 0, the value of a newly created attribute is stored in the trait depot if its size in bytes matches or exceeds the setting of this configurable.</param>
        /// <returns>true if no error</returns>
        private bool P4ConfigureSetAttributeStorageConfigurable(Connection con, int value)
        {
            string oldUser = con.UserName;
            con.UserName = "admin";
            con.Login("");

            // Tell the server to support parallel
            string[] args = { "set", "trait.storagedepot.min=" + value };
            using (var cmd = new P4Command(con, "configure", false, args))
            {
                var cmdr = cmd.Run(new Options());
                // now force a reconnect, or p4d might not use the new config
                con.getP4Server().Reconnect();

                con.UserName = oldUser;
                return cmdr.Success;
            }
        }

        /// <summary>
        /// Method is used by MSTest holds test data.
        /// </summary>
        /// <returns>Returns IEnumerable collection of test data</returns>
        private static IEnumerable<object[]> GetAttributeAndFstatTestData()
        {
            // config_trait_storagedepot_min : trait.storagedepot.min=0
            // attributeName: attribute name : FileID
            // attributeValue: attribute value : 10
            // getFileMetadataCmdFlags: fstat command flag : -Oa
            // expectedValue: expected value from command result: 10 
            yield return new object[]
            {
                0,
                "FileID",
                10,
                GetFileMetadataCmdFlags.Attributes,
                "10"
            };
            yield return new object[]
            {
                 1,
                 "InitialFileSize",
                 100,
                 GetFileMetadataCmdFlags.AttributesProp,
                 ".p4traits"
            };
            yield return new object[]
            {
                  -3,
                  "fileVersion",
                  100,
                  GetFileMetadataCmdFlags.AttributesProp,
                  "Trait depot storage has not been configured by 'trait.storagedepot.min'. This value must be greater than 0 to enable trait depot storage.\n"
            };
            yield return new object[]
            {
                 3,
                 "IsFileReadOnly",
                 1,
                 GetFileMetadataCmdFlags.AttributesProp,
                 "db.traits"
            };
            yield return new object[]
            {
                 3,
                 "FileSize",
                 "4000KB",
                 GetFileMetadataCmdFlags.AttributesProp,
                 ".p4traits"
            };
            yield return new object[]
            {
                 2,
                 "FileUsers",
                 "admin,super user",
                 GetFileMetadataCmdFlags.AttributesProp,
                 "db.traits",
                 true
            };
        }
    }
}
