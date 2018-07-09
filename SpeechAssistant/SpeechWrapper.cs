using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;


namespace SpeechAssistant
{

    /// <summary>
    /// a wrapper for the speech recognition engine, makes loading in grammars and recognition easier.
    /// Also allows for other speech controls to be loaded in through a DLL that defines objects that inherit from the speech control abstract class
    /// </summary>
    public class SpeechWrapper
    {
        /// <summary>
        /// the minimum confidence to recognize speech
        /// </summary>
        private float minimumConfidence { get; set; }

        /// <summary>
        /// a word that is used to sperate a command from normal speech, i.e. 'Alexa' for amazon echo
        /// </summary>
        public string initWord { get; private set; }
        /// <summary>
        /// speech recognition that recognizes the speech
        /// </summary>
        private SpeechRecognitionEngine speechEngine { get; set; }

        /// <summary>
        /// controls that are loaded in through plugins
        /// </summary>
        [ImportMany(typeof(SpeechControl))]
        private List<SpeechControl> speechControls { get; set; }

        public SpeechWrapper(string initWord, float minConfidence = 0)
        {
            this.minimumConfidence = minConfidence;
            this.initWord = initWord;
            speechEngine = new SpeechRecognitionEngine();
            speechEngine.SetInputToDefaultAudioDevice();

        }

        /// <summary>
        /// load in default dictation, spelling, and question grammars
        /// </summary>
        public void loadBaseSrGrammars()
        {
            DictationGrammar defaultGrammar = new DictationGrammar();
            defaultGrammar.Name = "default dictation";
            defaultGrammar.Enabled = true;

            DictationGrammar spelling = new DictationGrammar("grammar:dictation#spelling");
            spelling.Name = "spelling dictation";
            spelling.Enabled = true;

            DictationGrammar question = new DictationGrammar("grammar:dictation");
            question.Name = "question dictation";
            question.Enabled = true;

            speechEngine.LoadGrammar(defaultGrammar);
            speechEngine.LoadGrammar(spelling);
            speechEngine.LoadGrammar(question);
        }

        /// <summary>
        /// load a grammar into the speech recognition engine
        /// </summary>
        /// <param name="gIn"></param>
        public void loadGrammar(Grammar gIn)
        {
            speechEngine.LoadGrammar(gIn);
        }

        /// <summary>
        /// get all of the plugins of speech control from a directory and load them in
        /// </summary>
        /// <param name="pluginDirectory">directory containing the plugins</param>
        public void getPlugins(string pluginDirectory)
        {
            PluginProvider provider = PluginProvider.instance;
            provider.getPlugins<SpeechControl>(pluginDirectory).ComposeParts(this);
            setupPlugins();
        }

        /// <summary>
        /// foreach plugin run their setup method and load their grammars
        /// </summary>
        public void setupPlugins()
        {
            foreach (SpeechControl control in speechControls)
            {
                control.setUp(this);
                control.loadDictionaryGrammar(this);
            }
        }


        /// <summary>
        /// unsubscribe from speech recognized event
        /// </summary>
        public void stopRecognition()
        {
            speechEngine.SpeechRecognized -= SpeechEngine_SpeechRecognized;
        }

        /// <summary>
        /// subscribe to speech recognized event and start recognizing
        /// </summary>
        public void beginRecognition()
        {
            speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized;
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// pass the recognized speech to each control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= minimumConfidence)
            {
                foreach (SpeechControl control in speechControls)
                {
                    if (control.processCommand(e.Result.Text)) return;
                    if (control.processGrammar(e)) return;
                }
            }
        }

    }
}
