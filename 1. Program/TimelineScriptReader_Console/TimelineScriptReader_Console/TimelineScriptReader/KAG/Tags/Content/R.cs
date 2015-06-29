using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Content
{
    class R : Tag
    {
        public R() : base("r")
        {

        }
        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            ScriptReader sr = a_data.Page.Script;
            // RetrieveModule
            ConsoleModule cm = (ConsoleModule)sr.RetrieveModule(ConsoleModule.NAME);
            if (cm != null)
            {
                cm.ChangeLine();
            }
        }
    }
}
