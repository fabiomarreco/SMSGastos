using MerrequinhaAPI;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;

namespace Test.MerrequinhaAPI
{
    
    
    /// <summary>
    ///This is a test class for ConversorSMSTest and is intended
    ///to contain all ConversorSMSTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConversorSMSTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
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

        private static string CarregaResource(string nomeArquivo)
        {
            string txt = string.Empty;
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Test.MerrequinhaAPI.Inputs." + nomeArquivo))
            {
                var sr = new StreamReader(s);
                txt = sr.ReadToEnd();
            }
            return txt;
        }
        /// <summary>
        ///A test for ProcessaDocumento
        ///</summary>
        [TestMethod()]
        [DeploymentItem("MerrequinhaAPI.dll")]
        public void ProcessaDocumentoTest()
        {
            var str = CarregaResource("Banco1.txt");

            ConversorSMS_Accessor target = new ConversorSMS_Accessor(); // TODO: Initialize to an appropriate value
            var actual = target.ProcessaDocumento(str).ToArray();
            Assert.AreEqual(62, actual.Length);
            
            var porSender = actual.GroupBy (s=> s.Sender).ToDictionary(s=> s.Key, s=>s);
            Assert.AreEqual(2, porSender.Count);
            Assert.AreEqual(53, porSender["Itau"].Count());
            Assert.AreEqual(9, porSender["Santander"].Count());
        }

    }
}
