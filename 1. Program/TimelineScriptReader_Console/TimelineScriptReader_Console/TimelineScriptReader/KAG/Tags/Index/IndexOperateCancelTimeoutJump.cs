using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG;
using TimelineScriptReader.KAG.Trigger;

namespace TimelineScriptReader.KAG.Tags.Index
{
    class IndexOperateCancelTimeoutJump : Tag
    {
        public IndexOperateCancelTimeoutJump()
            : base("ctimeout")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;

            // Retrieve module and setting action
            TimeoutJumpTrigger tjt = (TimeoutJumpTrigger)kag.RetrieveTrigger(TimeoutJumpTrigger.NAME);
            if (tjt != null)
            {
                tjt.Enabled = false;
            }
        }
    }
}
