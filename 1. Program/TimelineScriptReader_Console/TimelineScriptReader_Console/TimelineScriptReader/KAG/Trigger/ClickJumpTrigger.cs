using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Trigger
{
    class ClickJumpTrigger : ScriptTrigger
    {
        // static variable
        public const string NAME = "CLICK_JUMP_TRIGGER";

        // member variable
        private string m_pagename;
        private string m_filename;
        private string m_expression;
        private bool m_isEnabled;

        // Constructure
        public ClickJumpTrigger()
            : base (ClickJumpTrigger.NAME)
        {
        }

        // Method
        public override void Execute()
        {
            base.Execute();

            // Execute exp

            // Execute Jump
            KAGReader kag = (KAGReader)this.Reader;
            BookmarkModule bm = (BookmarkModule)kag.RetrieveModule(BookmarkModule.NAME);
            if (bm != null)
            {
                bm.JumpTo(this.m_pagename, this.m_filename);
                kag.ExecuteModules(BookmarkModule.NAME, false);
            }
        }

        // Protected Method
        protected override void InitialTriggerEvent()
        {
            base.InitialTriggerEvent();
            // When click jump activate, have 2 step.
            // 1. When Reader on skip, then trigger event.
            // 2. Execute trigger event , first reader.state is STOP, and isEnabled is true. 
            if( this.Reader != null )
            {
                KAGReader kag = (KAGReader)this.Reader;
                if(kag != null )
                    kag.OnSkipTrigger += new KAGReader.ReaderEventHandler(this.ReaderOnSkip);
            }
        }

        // Public Method, Setting Jump information.
        public void JumpTo(string a_pagename, string a_filename, string a_expression)
        {
            this.m_filename = a_filename;
            this.m_pagename = a_pagename;
            this.m_expression = a_expression;
        }

        // Attribute
        public bool Enabled
        {
            set { this.m_isEnabled = value; }
        }

        // 
        private void ReaderOnSkip(KAGReader a_reader)
        {
            if (a_reader.CurrentReaderState == KAGReader.STATE_STOP && this.m_isEnabled)
                this.Execute();
        }
    }
}
