using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.Modules
{
    class TimerModule : ScriptModule
    {
        // static variable
        public const string NAME = "TIMER_MODULE";

        // Timer member
        int m_intervalTime;
        bool m_canSkip;

        // Constructure
        public TimerModule()
            : base(TimerModule.NAME)
        {
            // Initial timer
            this.timer.Elapsed += new ElapsedEventHandler(this.Timeout);

            // Initial member
            this.m_intervalTime = 1;
        }

        // Attribute
        public override bool IsComplete
        {
            get { return !this.timer.Enabled; }
        }

        // Method
        public override void Execute()
        {
            this.timer.Interval = ((KAGReader)this.Reader).GetWaitingTime(this.m_intervalTime);
            this.timer.Start();
        }

        public override void ExecuteSkip()
        {
            if (this.m_canSkip)
                this.Timeout();
        }

        // 
        public void SetTime( int a_millisecond )
        {
            this.m_intervalTime = a_millisecond;
            if (this.m_intervalTime < 1)
                this.m_intervalTime = 1;
        }

        public void CanSkip( bool a_canSkip )
        {
            this.m_canSkip = a_canSkip;
        }
        // Private Method
        private void Timeout(object sender = null, System.Timers.ElapsedEventArgs e = null)
        {
            this.timer.Stop();
            if (this.Reader != null)
                ((KAGReader)this.Reader).CheckExecuteModulesComplete();
        }
    }
}
