using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic;
using Jurassic.Library;
using TimelineScriptReader.DataStruct;

namespace TimelineScriptReader.KAG.Modules
{   
    class JavaScriptModule : ScriptModule
    {
        // static variable
        public const string NAME = "JAVASCRIPT_MODULE";

        // Script engine
        private Jurassic.ScriptEngine m_engine;
        private JavaScriptVariableClass m_systemVar;
        private JavaScriptVariableClass m_gameVar;
        private JavaScriptVariableClass m_tempVar;

        // Constructure
        public JavaScriptModule()
            : base(JavaScriptModule.NAME)
        {
            // Initial engine
            this.m_engine = new Jurassic.ScriptEngine();

            // Initial variable
            this.m_systemVar = new JavaScriptVariableClass(this.m_engine);
            this.m_gameVar = new JavaScriptVariableClass(this.m_engine);
            this.m_tempVar = new JavaScriptVariableClass(this.m_engine);

            this.m_engine.SetGlobalValue("sf", this.m_systemVar);
            this.m_engine.SetGlobalValue("f", this.m_gameVar);
            this.m_engine.SetGlobalValue("tf", this.m_tempVar);
        }


        // Attribute
        public override bool IsComplete
        {
            get
            {
                return !this.timer.Enabled;
            }
        }

        // Public override method
        public override void Execute()
        {
            
        }

        public override void ExecuteSkip()
        {

        }

        // Public method
        // Evaluation input expression.
        public void Eval(string a_expression)
        {
            try
            {
                this.m_engine.Execute(a_expression);
            }
            catch( Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // Take back variable.
        public T Emb<T>( string a_expression )
        {
            T result = default(T);
            try
            {
                result = (T)this.m_engine.Evaluate<T>(a_expression);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result;
        }

        // Check variabe exist
        public bool Exist(string a_vaiableName)
        {
            string[] varInfo = a_vaiableName.Split('.');
            // check variable is inside class or not.
            if(varInfo.Length > 1)
            {
                if (varInfo[0].Equals("sf"))
                    return this.m_systemVar.HasProperty(varInfo[1]);
                else if (varInfo[0].Equals("f"))
                    return this.m_gameVar.HasProperty(varInfo[1]);
                else if (varInfo[0].Equals("tf"))
                    return this.m_tempVar.HasProperty(varInfo[1]);
            }
                
            return this.m_engine.HasGlobalValue(a_vaiableName);
        }

        // Clear system variable
        public void ClearSystemVariable()
        {
            JavaScriptVariableClass temp = this.m_systemVar;
            this.m_systemVar = null;
            this.m_systemVar = new JavaScriptVariableClass(this.m_engine);
            temp = null;
        }

        // Clear game variable
        public void ClearGameVariable()
        {
            JavaScriptVariableClass temp = this.m_gameVar;
            this.m_gameVar = null;
            this.m_gameVar = new JavaScriptVariableClass(this.m_engine);
            temp = null;
        }

        // Clear temp variable
        public void ClearTempVariable()
        {
            JavaScriptVariableClass temp = this.m_tempVar;
            this.m_tempVar = null;
            this.m_tempVar = new JavaScriptVariableClass(this.m_engine);
            temp = null;
        }

        // Attribute
        public ObjectInstance SystemVar
        { get { return this.m_systemVar; } }

        public ObjectInstance GameVar
        { get { return this.m_gameVar; } }

        public ObjectInstance tempVar
        { get { return this.m_tempVar; } }

        // Internal class
        protected class JavaScriptVariableClass : ObjectInstance
        {
            public JavaScriptVariableClass(ScriptEngine a_engine)
                : base(a_engine)
            {
                
            }
        }
    }
}
