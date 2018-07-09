using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace SpeechAssistant
{
    [Serializable]
    public class DataManager : ISerializable
    {
        public void addLocation(Environment.SpecialFolder folder,string extraLocation)
        {
            if (!directoryLocations.ContainsKey(folder))
                directoryLocations[folder] = new List<string>();
            directoryLocations[folder].Add(extraLocation);
        }
        public void removeLocation(Environment.SpecialFolder folder, string removeLocation)
        {
            try
            {
                directoryLocations[folder].Remove(removeLocation);
            }
            catch(KeyNotFoundException ex)
            {
                // yes while this is empty this is bad
            }
        }

        public Dictionary<Environment.SpecialFolder,List<string>> directoryLocations { get; private set; }

        public static DataManager getManager(string containingDir)
        {
            string existingManager = recursiveFind(containingDir, ".dm");
            if (existingManager != null)
                return Load(existingManager);
            else
                return new DataManager();
        }

        public static string recursiveFind(string containtingDir,string extension)
        {
            foreach(string file in Directory.GetFiles(containtingDir))
            {
                if (Path.GetExtension(file).Equals(extension))
                    return file;
            }
            foreach(string subDir in Directory.GetDirectories(containtingDir))
            {
                recursiveFind(subDir, extension);
            }
            return null;
        }
        public DataManager()
        {
            this.directoryLocations = new Dictionary<Environment.SpecialFolder, List<string>>();
        }
        public DataManager(SerializationInfo info, StreamingContext context)
        {
            this.directoryLocations = (Dictionary<Environment.SpecialFolder, List<string>>)info.GetValue("locations", typeof(Dictionary<Environment.SpecialFolder, List<string>>));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("locations", directoryLocations, typeof(Dictionary<Environment.SpecialFolder, List<string>>));
        }

        public static void Save(DataManager toSave,string saveLocation)
        {
            
            using (FileStream s = new FileStream(saveLocation, FileMode.Create))
            {
                new BinaryFormatter().Serialize(s, toSave);
                s.Close();
            }
        }
        public static DataManager Load(string loadLocation)
        {
            DataManager toReturn;
            using (FileStream s = new FileStream(loadLocation, FileMode.Open))
            {
                toReturn = (DataManager)new BinaryFormatter().Deserialize(s);
                s.Close();
            }
            return toReturn;
        }
    }
}
