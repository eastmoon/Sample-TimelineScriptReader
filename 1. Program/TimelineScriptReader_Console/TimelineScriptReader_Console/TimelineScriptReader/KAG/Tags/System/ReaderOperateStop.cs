using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;

namespace TimelineScriptReader.KAG.Tags.System
{
    class ReaderOperateStop : Tag
    {
        public ReaderOperateStop()
            : base("s")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;

            // Retrieve module and setting action
            kag.CurrentReaderState = KAGReader.STATE_STOP;
        }
    }
}
