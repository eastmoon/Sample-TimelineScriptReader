using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.KAG.Tags;
using TimelineScriptReader.KAG.Modules;

namespace TimelineScriptReader.KAG.Tags.Logic
{
    class ConditionOperateIf : KAGPairsTag
    {
        // Member
        private ArrayList m_conditionList;
        private ScriptData m_exceptionCondition;

        // Constructor
        public ConditionOperateIf()
            : base("if")
        {
            this.m_conditionList = new ArrayList();
            this.m_exceptionCondition = null;
        }

        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // describe variable
            KAGReader kag = (KAGReader)a_data.Page.Script;
            JavaScriptModule jsm = (JavaScriptModule)kag.RetrieveModule(JavaScriptModule.NAME);
            ScriptData condition = null;
            string expression = "";
            bool isRight = false;
            int i = 0;

            // Retrieve Attribute
            foreach (string key in a_data.Attribute.Keys)
            {
                switch (key)
                {
                    case "exp":
                        {
                            expression = a_data.Attribute[key].ToString();
                        }
                        break;
                }
            }

            // Pre-capture, clear old data.
            if (this.m_conditionList.Count > 0)
                this.m_conditionList.Clear();
            this.m_exceptionCondition = null;

            // new IF condition object.
            condition = new ScriptData("IF", expression, null);
            this.m_conditionList.Add(condition);

            // Capture content
            this.CaptureDataList = condition.Content;
            this.CaptureContent(a_data.Page);

            // Retrieve module and setting action
            // Check condition list
            isRight = false;
            for (i = 0; i < this.m_conditionList.Count && !isRight; i++)
            {
                condition =(ScriptData)this.m_conditionList[i];
                expression = condition.Name;
                isRight = jsm.Emb<bool>(expression);
                if (isRight)
                    kag.AddDynamicQueue(condition.Content);
            }

            // Check exception condition object, if it is exist and isRight is false.
            if( !isRight && this.m_exceptionCondition != null )
            {
                kag.AddDynamicQueue(this.m_exceptionCondition.Content);
            }
        }

        // Check capture action.
        protected override bool IsCapture(IScriptData a_data)
        {
            bool result = base.IsCapture(a_data);

            // if nesert condition info, ignore.
            if (this.Counter > 0)
                return result;

            // do action.
            switch(a_data.Name)
            {
                case "elseif" :
                    {
                        string expression = "";
                        // Retrieve Attribute
                        foreach (string key in a_data.Attribute.Keys)
                        {
                            switch (key)
                            {
                                case "exp":
                                    {
                                        expression = a_data.Attribute[key].ToString();
                                    }
                                    break;
                            }
                        }

                        // New condition object.
                        ScriptData condition = new ScriptData("ELSEIF", expression, null);
                        // Change capture list
                        this.CaptureDataList = condition.Content;
                        // Ignore this capture
                        result = false;
                    }
                    break;
                case "else":
                    {
                        // New else condition object.
                        this.m_exceptionCondition = new ScriptData("ELSE", "", null);
                        this.CaptureDataList = this.m_exceptionCondition.Content;
                        // Ignore this capture
                        result = false;
                    }
                    break;
            }
            return result;
        }
    }
}
