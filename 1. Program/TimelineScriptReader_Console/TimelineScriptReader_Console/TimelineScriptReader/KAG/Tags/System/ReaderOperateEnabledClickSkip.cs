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
    class ReaderOperateEnabledClickSkip : Tag
    {
        public ReaderOperateEnabledClickSkip()
            : base("clickskip")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            bool canSkip = true;
            string value = "";

            // Retrieve Attribute
            foreach (string key in a_data.Attribute.Keys)
            {
                switch (key)
                {
                    case "enabled":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (this.isBooleanAttribute(value))
                                canSkip = value.Equals("true");
                            else if (this.isDefaultAttribute(value))
                                canSkip = true;
                        }
                        break;
                }
            }

            // Retrieve module and setting action
            kag.EnabledSkip = canSkip;
        }
    }
}
