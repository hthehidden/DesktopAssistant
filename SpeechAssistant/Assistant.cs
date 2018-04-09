using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechAssistant
{
    /// <summary>
    /// Object that stores assistant's name and a speech wrapper for recognizing commands
    /// </summary>
    public class Assistant
    {
        public string userName { get; private set; }

        /// <summary>
        /// assistant name for commands in the recognizer, acts as the initword
        /// </summary>
        public string assistantName { get; private set; }
        public SpeechWrapper recognizer { get; private set; }
        
        public Assistant(string userName, string assistantName,string pluginDir,float confidence)
        {
            this.userName = userName;
            this.assistantName = assistantName;
            recognizer = new SpeechWrapper(assistantName,confidence);
            recognizer.getPlugins(pluginDir);
            recognizer.beginRecognition();
        }
    }
}
