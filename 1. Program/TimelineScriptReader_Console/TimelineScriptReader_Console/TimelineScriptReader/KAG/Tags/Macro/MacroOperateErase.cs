using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.KAG.Tags;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Macro
{
    class MacroOperateErase : Tag
    {
        public MacroOperateErase()
            : base("erasemacro")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            Parser parser = (Parser)kag.TagParser;
            string name = "";

            // Retrieve Attribute
            foreach (string key in a_data.Attribute.Keys)
            {
                switch (key)
                {
                    case "name":
                        {
                            name = a_data.Attribute[key].ToString();
                        }
                        break;
                }
            }

            // Retrieve module and setting action
            if (name != "" && parser != null)
            {
                parser.RemovMacro(name);
            }
        }
    }
}
