using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG.Tags.System;
using TimelineScriptReader.KAG.Tags.Content;
using TimelineScriptReader.KAG.Tags.Index;
using TimelineScriptReader.KAG.Tags.Logic;
using TimelineScriptReader.KAG.Tags.Macro;

namespace TimelineScriptReader.KAG
{
    class Parser : TagContainer
    {
        // Member 
        private Hashtable m_macroList;

        // Constructure
        public Parser() : base("Parser")
        {
            // Inisial macro container
            this.m_macroList = new Hashtable();

            // System.TimelineOperate
            this.Register(new TimelineOperateDelay());
            this.Register(new TimelineOperateWait());
            this.Register(new TimelineOperateWaitclick());
            this.Register(new TimelineOperateWaitCharacterAmount());
            // System.ReaderOperate
            this.Register(new ReaderOperateStop());
            this.Register(new ReaderOperateEnabledClickSkip());
            this.Register(new ReaderOperateEnabledAutoskip());
            this.Register(new ReaderOperateStartAutoskip());
            this.Register(new ReaderOperateCancelAutoSkip());
            this.Register(new ReaderOperateEnabledAutomode());
            this.Register(new ReaderOperateStartAutomode());
            this.Register(new ReaderOperateCancelAutomode());

            // Index Control
            this.Register(new IndexOperateJump());
            this.Register(new IndexOperateCall());
            this.Register(new IndexOperateReturn());
            // Trigger Indext control
            this.Register(new IndexOperateClickJump());
            this.Register(new IndexOperateCancelClickJump());
            this.Register(new IndexOperateTimeoutJump());
            this.Register(new IndexOperateCancelTimeoutJump());

            // Logic, variable operate
            this.Register(new VariableOperateClearSystemVar());
            this.Register(new VariableOperateClearGameVar());
            // Logic, expression operate
            this.Register(new ExpressionOperateEvaluation());
            this.Register(new ExpressionOperateEmbed());
            // Logic, condition operate
            this.Register(new ConditionOperateIf());
            this.Register(new ConditionOperateIgnore());

            // Macro
            this.Register(new MacroOperate());
            this.Register(new MacroOperateErase());
            // Content
            this.Register(new Content());
            this.Register(new Cm());
            this.Register(new L());
            this.Register(new R());
        }

        // Attribute

        // Method
        public override void Execute(DataStruct.IScriptData a_data)
        {
            base.Execute(a_data);

            // Macro tag
            if( this.m_macroList[a_data.Name] != null )
            {
                this.ExecuteMacro(a_data);
            }
            else
            // Standard tag
            { 
                ITag target = this.Retrieve(a_data.Name);
                if (target != null)
                    target.Execute(a_data);
            }
        }

        // Add Macro tag
        public void AddMacro( string a_macroName, IScriptData a_macroData )
        {
            if (this.m_macroList[a_macroName] != null)
                this.m_macroList.Add(a_macroName, a_macroData);
            else
                this.m_macroList[a_macroName] = a_macroData;
        }
        // Remove Macro tag
        public void RemovMacro( string a_macroName )
        {
            if (this.m_macroList[a_macroName] != null)
                this.m_macroList.Remove(a_macroName);
        }
        // Execute Macro tag
        public void ExecuteMacro(IScriptData a_data)
        {
            // Copy array list
            int i = 0, j = 0, k = 0;
            IScriptData macroData = (IScriptData)this.m_macroList[a_data.Name];
            ArrayList targetMacroList = macroData.Content;
            ArrayList resultMacroList = new ArrayList();
            IScriptData target = null;
            IScriptData clone = null;
            string value = "";

            for (i = 0; i < targetMacroList.Count; i++)
            { 
                // clone object
                target = (IScriptData)targetMacroList[i];
                clone = null;
                if (target != null)
                {
                    clone = target.Clone(null);
                    resultMacroList.Add(clone);
                }
            }

            // Replace attribute
            foreach (int index in macroData.Attribute.Keys)
            {
                value = (string)macroData.Attribute[index];
                if (index < resultMacroList.Count)
                {
                    clone = (IScriptData)resultMacroList[index];
                    if( value.Equals("*") )
                    {
                        // copy all macro attribute
                        foreach (string key in a_data.Attribute.Keys)
                        {
                            if (clone.Attribute[key] == null)
                                clone.Attribute.Add(key, a_data.Attribute[key]);
                            else
                                clone.Attribute[key] = a_data.Attribute[key];
                        }
                    }
                    else
                    {
                        // Split string by symbol '|'. struct is [attribute key]|[default value]
                        string key = ((string)clone.Attribute[value]).Substring(1);
                        string defaultValue = "";
                        string[] keyInfo = key.Split('|');
                        if (keyInfo.Length > 0)
                            key = keyInfo[0];
                        if (keyInfo.Length > 1)
                            defaultValue = keyInfo[1];

                        // if key exist in macro attribute, copy attribute value to clone object.
                        if(a_data.Attribute[key] != null)
                        {
                            clone.Attribute[value] = a_data.Attribute[key];
                        }
                        // if key not exist, use default value.
                        else
                        {
                            if (defaultValue != "")
                                clone.Attribute[value] = defaultValue;
                            else
                                clone.Attribute[value] = Tag.DEFAULT_ATTRIBUTE_VALUE;
                        }
                    }
                }
            }
            // Save array to dynamic queue
            KAGReader kag = (KAGReader)a_data.Page.Script;
            kag.AddDynamicQueue(resultMacroList);
        }
    }
}
