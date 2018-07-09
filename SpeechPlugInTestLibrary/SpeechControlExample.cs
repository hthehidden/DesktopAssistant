using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpeechAssistant;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Speech.Recognition;

namespace SpeechPlugInTestLibrary
{
    [Export(typeof(SpeechControl))]
    public class SpeechControlExample : SpeechControl
    {
        [ImportingConstructor]
        public SpeechControlExample() : base() { }
        public override void setUp(SpeechWrapper speechWrapper)
        {
            addPhraseCommand(new TestExample().create(speechWrapper.initWord));
        }
    }


    public class TestExample : PhraseCommand
    {
        public override void commandProcess(string commandInput)
        {
            MessageBox.Show("successful test");
        }

        protected override void defineCommands(string initWord)
        {
            addCommand("run test");

            string[] choic = new string[]{"grammar test"};
            Choices dicChoices = new Choices(choic);
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(dicChoices);
            grammar = new Grammar(gBuilder);
        }

        public override void Grammar_SpeechRecognized( SpeechRecognizedEventArgs e)
        {
            MessageBox.Show("Grammar event raised");
        }
    }
}
