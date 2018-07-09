using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace SpeechAssistant
{
    /// <summary>
    /// An object containing relating phrase commands, and data that is shared between them
    /// </summary>
    [Export]
    public abstract class SpeechControl
    {
        /// <summary>
        /// commands and their linked phrase commands
        /// </summary>
        public Dictionary<string, PhraseCommand> phraseCommands { get; set; }

        /// <summary>
        /// grammar created by the commands from loaded in phrase commands
        /// </summary>
        protected Grammar basicGrammar { get; set; }

        /// <summary>
        /// grammars provided by loaded in phrase commands
        /// </summary>
        protected Dictionary<Grammar,PhraseCommand> grammars { get; set; }

        [ImportingConstructor]
        public SpeechControl()
        {
            phraseCommands = new Dictionary<string, PhraseCommand>();
            grammars = new Dictionary<Grammar, PhraseCommand>();
        }

        /// <summary>
        /// add phrase command and grammar to speech control, if a phrase already exists it is not added again
        /// </summary>
        /// <param name="phraseCommandInput"></param>
        public void addPhraseCommand(PhraseCommand phraseCommandInput)
        {
            if(phraseCommandInput.commandPhrases != null)
                foreach(string command in phraseCommandInput.commandPhrases)
                {
                    if(!phraseCommands.ContainsKey(command))
                    {
                        phraseCommands.Add(command, phraseCommandInput);
                    }
                }
            if (phraseCommandInput.grammar != null) grammars.Add(phraseCommandInput.grammar,phraseCommandInput);
        }

        /// <summary>
        /// send command to phrase command and have it perform its command Process method
        /// </summary>
        /// <param name="commandInput"></param>
        /// <returns></returns>
        public bool processCommand(string commandInput)
        {
            if(phraseCommands.ContainsKey(commandInput))
            {
                phraseCommands[commandInput].commandProcess(commandInput);
                return true;
            }
            return false;
        }

        /// <summary>
        /// send the recognized  to the corresponding phrase command and have it perform its grammar speech recognized method
        /// </summary>
        /// <param name="gram"></param>
        /// <returns></returns>
        public bool processGrammar(SpeechRecognizedEventArgs gram)
        {
            if(grammars.ContainsKey(gram.Result.Grammar))
            {
                grammars[gram.Result.Grammar].Grammar_SpeechRecognized(gram);
                return true;
            }
            return false;
        }

        /// <summary>
        /// create basic grammar, then load basic grammar and other grammars into the speech wrapper
        /// </summary>
        /// <param name="speechWrapper"></param>
        public void loadDictionaryGrammar(SpeechWrapper speechWrapper)
        {
            if (phraseCommands.Count > 0)
            {
                Choices dicChoices = new Choices(phraseCommands.Keys.ToArray());
                GrammarBuilder gBuilder = new GrammarBuilder();
                gBuilder.Append(dicChoices);
                basicGrammar = new Grammar(gBuilder);
                speechWrapper.loadGrammar(basicGrammar);
            }
            foreach (Grammar gram in grammars.Keys)
            {
                speechWrapper.loadGrammar(gram);
            }
        }

        /// <summary>
        /// use this method to add phrase commands through the addPhraseCommandProcess
        /// </summary>
        /// <param name="speechWrapper"></param>
        public abstract void setUp(SpeechWrapper speechWrapper);
    }

    
    /// <summary>
    /// a pairing of a phrase/grammar and a command/method that is run when that phrase/grammar is detected
    /// </summary>
    public abstract class PhraseCommand
    {
        /// <summary>
        /// commands that will trigger this phrase commands processCommand method
        /// </summary>
        public List<string> commandPhrases { get; private set; }
        /// <summary>
        /// custom grammar for this phrase command object, will trigger the Grammar_SpeechRecognized method
        /// </summary>
        public Grammar grammar { get; protected set; }
        
        public PhraseCommand create(string initWord)
        {
            commandPhrases = new List<string>();
            defineCommands(initWord);
            return this;
        }

        /// <summary>
        /// override this method when implementing a customer grammar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void Grammar_SpeechRecognized(SpeechRecognizedEventArgs e);

        /// <summary>
        /// define the commands that will trigger the commandProcess method here, including the custom grammar
        /// </summary>
        protected abstract void defineCommands(string initWord);

        /// <summary>
        /// method to make it easier to add commands to the phrase command list
        /// </summary>
        /// <param name="commandInput"></param>
        protected void addCommand(string commandInput)
        {
            commandPhrases.Add(commandInput);
        }

        /// <summary>
        /// method that will run when a command from the commandPhrases List is detected
        /// </summary>
        /// <param name="commandInput"></param>
        public abstract void commandProcess(string commandInput);
    }


}
