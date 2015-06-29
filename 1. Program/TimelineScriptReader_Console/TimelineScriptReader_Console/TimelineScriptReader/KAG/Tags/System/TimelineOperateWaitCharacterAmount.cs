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
    class TimelineOperateWaitCharacterAmount : Tag
    {
        public TimelineOperateWaitCharacterAmount()
            : base("wc")
        { }

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            int amount = 1;
            bool canskip = true;
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
                                amount = Int32.Parse(value);
                        }
                        break;
                    case "canskip":
                        {
                            value = a_data.Attribute[key].ToString();
                            if (this.isBooleanAttribute(value))
                                canskip = value.Equals("true");
                            else if (this.isDefaultAttribute(value))
                                canskip = true;
                        }
                        break;
                }
            }

            // Retrieve module and setting action
            TimerModule timer = (TimerModule)kag.RetrieveModule(TimerModule.NAME);
            timer.SetTime(kag.GetWaitingTimeByCharacter(amount));
            timer.CanSkip(canskip);
            kag.ExecuteModules(TimerModule.NAME, false);
        }
    }
}
