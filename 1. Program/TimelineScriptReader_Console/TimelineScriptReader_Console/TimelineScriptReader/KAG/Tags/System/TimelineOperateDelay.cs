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
    class TimelineOperateDelay : Tag
    {
        public TimelineOperateDelay()
            : base("delay")
        {}
        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);
            
            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            int delay = kag.CharacterDelay;
            string value = "";

            // Retrieve Attribute
            foreach (string key in a_data.Attribute.Keys)
            {
                switch( key )
                {
                    case "speed":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (value.Equals("nowait"))
                                delay = kag.SystemDelay;
                            else if (this.isNumberAttribute(value))
                                delay = Int32.Parse(value);
                        }
                        break;
                }
            }

            // Retrieve module and setting action
            kag.CharacterDelay = delay;
        }
    }
}
