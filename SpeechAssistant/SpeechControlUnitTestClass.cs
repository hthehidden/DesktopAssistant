using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace SpeechAssistant
{
    public class SpeechControlUnitTestClass : SpeechControl
    {
        public static int stateChangeValue { get; set; }
        public override void setUp(SpeechWrapper speechWrapper)
        {
            stateChangeValue = 0;
            addPhraseCommand(new PhraseCommandTest().create(speechWrapper.initWord));
        }

        class PhraseCommandTest : PhraseCommand
        {
            public override void commandProcess(string commandInput)
            {
                stateChangeValue = 1;
            }

            public override void Grammar_SpeechRecognized(SpeechRecognizedEventArgs e)
            {
                if ((bool)e.Result.Semantics["true_false"].Value)
                    stateChangeValue = 2;
            }

            protected override void defineCommands(string initWord)
            {
                addCommand("run unit test");
                Choices cs = new Choices();
                cs.Add(new SemanticResultValue("true", true));
                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(new SemanticResultKey("true_false", cs));
                this.grammar = new Grammar(gb);
            }
        }
    }
}
