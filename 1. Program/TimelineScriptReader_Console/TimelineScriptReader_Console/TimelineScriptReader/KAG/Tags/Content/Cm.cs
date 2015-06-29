using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Content
{
    class Cm : Tag
    {
        public Cm() : base("cm")
        {

        }
        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            ScriptReader sr = a_data.Page.Script;
            // RetrieveModule
            ConsoleModule cm = (ConsoleModule)sr.RetrieveModule(ConsoleModule.NAME);
            cm.ClearContent();
        }
    }
}
