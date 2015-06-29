using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.System
{
    class TimelineOperateWaitclick : Tag
    {
        public TimelineOperateWaitclick()
            : base("waitclick")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            

            // Retrieve Attribute
           
            // Retrieve module and setting action
            TimerModule timer = (TimerModule)kag.RetrieveModule(TimerModule.NAME);
            timer.SetTime(1);
            timer.CanSkip(true);
            kag.ExecuteModules(TimerModule.NAME);
        }
    }
}
