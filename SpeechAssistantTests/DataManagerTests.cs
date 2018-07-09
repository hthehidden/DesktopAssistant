using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeechAssistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpeechAssistant.Tests
{
    [TestClass()]
    public class DataManagerTests
    {
        [TestMethod()]
        public void addLocationTest()
        {
            DataManager manager = new DataManager();
            manager.addLocation(Environment.SpecialFolder.AdminTools, "admin tools");
            Assert.IsTrue(manager.directoryLocations[Environment.SpecialFolder.AdminTools].Contains("admin tools"));

        }

        [TestMethod()]
        public void removeLocationTest()
        {
            DataManager manager = new DataManager();
            manager.addLocation(Environment.SpecialFolder.AdminTools, "admin tools");
            Assert.IsTrue(manager.directoryLocations[Environment.SpecialFolder.AdminTools].Contains("admin tools"));
            manager.removeLocation(Environment.SpecialFolder.AdminTools, "admin tools");
            Assert.IsTrue(!manager.directoryLocations[Environment.SpecialFolder.AdminTools].Contains("admine tools"));
        }

        [TestMethod()]
        public void getManagerTest()
        {

            DirectoryInfo dirInfo = Directory.CreateDirectory("DataManagerUnitTestDirectory");
            string file = string.Format("{0}\\{1}", dirInfo.Name, "DataManagerUnitTestFile.dm");
            try
            {
                DataManager manager = new DataManager();
                DataManager.Save(manager, file);
                manager = null;
                manager = DataManager.getManager(dirInfo.Name);
                Assert.IsTrue(manager != null);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                File.Delete(file);
                dirInfo.Delete();
            }
        }


        [TestMethod()]
        public void DatamanagerConstructorTest()
        {
            try
            {
                new DataManager();
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void SaveTest()
        {
            DataManager manager = new DataManager();
            string file = "DataManagerUnitTestFile.dam";
            bool written = false;
            try
            {
                DataManager.Save(manager, file);
                written = true;
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (written)
                    File.Delete(file);
            }
        }
        [TestMethod()]
        public void IsSerializable()
        {
            DataManager manager = new DataManager();
            manager.addLocation(Environment.SpecialFolder.AdminTools, "admin tools");
            using (MemoryStream mem = new MemoryStream())
            {
                BinaryFormatter b = new BinaryFormatter();
                try
                {
                    b.Serialize(mem,manager);
                }
                catch(Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
        }

        [TestMethod()]
        public void LoadTest()
        {
            DataManager manager = new DataManager();
            manager.addLocation(Environment.SpecialFolder.AdminTools, "admin tools");
            Assert.IsTrue(manager.directoryLocations[Environment.SpecialFolder.AdminTools].Contains("admin tools"));
            string file = "DataManagerUnitTestFile.dam";
            bool written = false;
            try
            {
                DataManager.Save(manager, file);
                written = true;
                manager = new DataManager();
                Assert.IsFalse(manager.directoryLocations.Keys.Contains(Environment.SpecialFolder.AdminTools));
                manager = DataManager.Load(file);
                Assert.IsTrue(manager.directoryLocations[Environment.SpecialFolder.AdminTools].Contains("admin tools"));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (written)
                    File.Delete(file);
            }
        }

    }
}