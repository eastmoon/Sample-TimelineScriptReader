using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Logic
{
    class VariableOperateClearGameVar : Tag
    {
        public VariableOperateClearGameVar()
            : base("clearvar")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            JavaScriptModule jsm = (JavaScriptModule)kag.RetrieveModule(JavaScriptModule.NAME);
            // Retrieve module and setting action
            if (jsm != null)
            {
                jsm.ClearGameVariable();
            }
        }
    }
}
