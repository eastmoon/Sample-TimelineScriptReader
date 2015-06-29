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
    class IndexOperateTimeoutJump : Tag
    {
        public IndexOperateTimeoutJump()
            : base("timeout")
        {}

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            int interval = 1;
            string filename = "";
            string pagename = "";
            string expression = "";
            string value = "";

            // Retrieve Attribute
            foreach (string key in a_data.Attribute.Keys)
            {
                switch (key)
                {
                    case "time":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (this.isNumberAttribute(value))
                                interval = Int32.Parse(value);
                        }
                        break;
                    case "storage":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (!this.isDefaultAttribute(value))
                                filename = value;
                            else
                                filename = kag.CurrentReadFileName;
                        }
                        break;
                    case "target":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (!this.isDefaultAttribute(value))
                                pagename = value;
                        }
                        break;
                    case "exp":
                        {
                            expression = a_data.Attribute[key].ToString();
                        }
                        break;
                }
            }

            // Retrieve module and setting action
            TimeoutJumpTrigger tjt = (TimeoutJumpTrigger)kag.RetrieveTrigger(TimeoutJumpTrigger.NAME);
            if (tjt != null)
            {
                tjt.JumpTo(interval, pagename, filename, expression);
                tjt.Enabled = true;
            }
        }
    }
}
