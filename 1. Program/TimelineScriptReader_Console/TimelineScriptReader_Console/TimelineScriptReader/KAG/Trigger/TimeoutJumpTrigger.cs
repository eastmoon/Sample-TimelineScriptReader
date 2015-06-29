using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Trigger
{
    class TimeoutJumpTrigger : ScriptTrigger
    {
        // static variable
        public const string NAME = "TIMEOUT_JUMP_TRIGGER";

        // member variable
        private int m_interval;
        private string m_pagename;
        private string m_filename;
        private string m_expression;
        private bool m_isEnabled;

        // Constructure
        public TimeoutJumpTrigger()
            : base(TimeoutJumpTrigger.NAME)
        {
            this.timer.Elapsed += new ElapsedEventHandler(this.Timeout);
        }

        // Method
        public override void Execute()
        {
            this.timer.Interval = this.m_interval;
            this.timer.Start();
        }

        // Protected Method
        protected override void InitialTriggerEvent()
        {
            base.InitialTriggerEvent();

            // When click jump activate, have 2 state.
            // 1. When Reader change state to STOP, then trigger event.
            // 2. Execute trigger event , first reader.state is STOP, and isEnabled is true. 
            // 3. When timer is timeout, execute express and jump.

            if (this.Reader != null)
            {
                KAGReader kag = (KAGReader)this.Reader;
                if (kag != null)
                    kag.OnStateChange += new KAGReader.ReaderEventHandler(this.ReaderOnStateChange);
            }
        }

        // Public Method, Setting Jump information.
        public void JumpTo(int a_interval, string a_pagename, string a_filename, string a_expression)
        {
            this.m_interval = a_interval;
            this.m_filename = a_filename;
            this.m_pagename = a_pagename;
            this.m_expression = a_expression;

            if (this.m_interval <= 0)
                this.m_interval = 1;
        }

        // Attribute
        public bool Enabled
        {
            set { this.m_isEnabled = value; }
        }

        // Private method
        private void ReaderOnStateChange(KAGReader a_reader)
        {
            if (a_reader.CurrentReaderState == KAGReader.STATE_STOP && this.m_isEnabled)
                this.Execute();
        }

        private void Timeout(object sender = null, System.Timers.ElapsedEventArgs e = null)
        {
            // stop timer.
            this.timer.Stop();

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
    }
}
