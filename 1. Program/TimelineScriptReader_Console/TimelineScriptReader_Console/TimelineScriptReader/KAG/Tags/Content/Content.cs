using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Content
{
    class Content : Tag
    {
        public Content() : base("content")
        {

        }

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            // RetrieveModule
            ConsoleModule cm = (ConsoleModule)kag.RetrieveModule(ConsoleModule.NAME);
            if (cm != null)
            {
                cm.AddContent((string)a_data.Content[0]);
            }
            // Reader execute
            kag.ExecuteModules(ConsoleModule.NAME, false);
        }
    }
}
