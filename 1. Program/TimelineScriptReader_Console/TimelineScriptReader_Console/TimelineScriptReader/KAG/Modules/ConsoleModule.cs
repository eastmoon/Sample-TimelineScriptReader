using System;
using System.Timers;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.Modules
{
    class ConsoleModule : ScriptModule
    {
        // static variable
        public const string NAME = "CONSOLE_MODULE";
        // Member variable

        // Content member
        ArrayList m_content;
        int m_currentLineIndex;
        int m_currentCharacterIndex;

        // Constructure
        public ConsoleModule()
            : base(ConsoleModule.NAME)
        {
            // Initial content management
            this.m_content = new ArrayList();

            // Setting Console
            Console.OutputEncoding = Encoding.Unicode;

            // Initial timer
            this.timer.Elapsed += new ElapsedEventHandler(this.ReadContent);
        }

        // Attribute
        public override bool IsComplete
        {
            get { return !this.timer.Enabled; }
        }

        // Method
        public override void Execute()
        {
            this.timer.Interval = ((KAGReader)this.Reader).GetWaitingTimeByCharacter();
            this.timer.Start();
        }

        public override void ExecuteSkip()
        {
            this.timer.Interval = ((KAGReader)this.Reader).GetWaitingTimeByCharacter();
            this.timer.Start();
        }

        public void AddContent( string a_content )
        {
            // make sure have 1 line in content
            if (this.m_content.Count == 0)
                this.ChangeLine();

            // add string in current content
            this.m_content[(this.m_content.Count - 1)] += a_content;
        }

        public void ChangeLine()
        {
            this.m_content.Add("");
        }

        public void ClearContent()
        {
            // Clear console view
            Console.Clear();
            // Clear data array
            this.m_content.Clear();
            this.m_currentLineIndex = 0;
            this.m_currentCharacterIndex = 0;
        }

        // private method
        private void ReadContent(object sender, System.Timers.ElapsedEventArgs e)
        {
            string content = "";
            if( this.m_currentLineIndex < this.m_content.Count )
            { 
                content = (string)this.m_content[this.m_currentLineIndex];
                int index = this.m_currentCharacterIndex++;
                if(index < content.Length)
                    Console.Write(content.Substring(index, 1));
            }

            if (this.m_currentCharacterIndex >= content.Length)
            {
                if ((this.m_currentLineIndex + 1) < this.m_content.Count )
                { 
                    Console.Write("\n");
                    this.m_currentLineIndex++;
                    this.m_currentCharacterIndex = 0;
                }
                else
                {
                    this.timer.Stop();
                    if (this.Reader != null)
                        ((KAGReader)this.Reader).CheckExecuteModulesComplete();
                }
            }
        }
    }
}
