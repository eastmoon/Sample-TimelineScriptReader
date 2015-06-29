using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Content
{
    class L : Tag
    {
        public L() : base("l")
        {

        }
        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kr = (KAGReader)a_data.Page.Script;
            kr.ExecuteModules(ConsoleModule.NAME);
        }
    }
}
