using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using TimelineScriptReader;
using TimelineScriptReader.DataStruct;
using TimelineScriptReader.MarkupStruct;
using TimelineScriptReader.KAG;
using TimelineScriptReader.KAG.Modules;
using TimelineScriptReader.KAG.Trigger;
using TimelineScriptReader.KAG.ReaderCommand;

namespace TimelineScriptReader_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Script processing

            // Class test
            Program root = new Program();

            // Timeline Script Reader, KAG tag script test.
            root.Test_ScriptReader_Start();

            // Script Reader method and KAG module test function.
            //root.Test_ReaderEventHandler();
            //root.Test_ScriptReader();
            //root.Test_ConsoleModule();
            //root.Test_JavaScriptModule();

            // KAG Script parser method test function.
            //root.Test_ParserRow();
            //root.Test_ParserBookmark();
            //root.Test_ParserCommand();
            //root.Test_ParserTagAndContent();

            // Script structure test function.
            //root.Test_ScriptData();
            //root.Test_TagContainer();
            //root.Test_PairsTag();
        }

        // Timeline Script Reader, KAG tag script test.
        public void Test_ScriptReader_Start()
        {
            Console.WriteLine("\n=========== Test ScriptReader.Start ===========\n");

            KAGReader kag = new KAGReader();
            kag.AddModule(new ConsoleModule());
            kag.AddModule(new TimerModule());
            kag.AddModule(new BookmarkModule());
            kag.AddModule(new JavaScriptModule());
            kag.AddTrigger(new ClickJumpTrigger());
            kag.AddTrigger(new TimeoutJumpTrigger());
            kag.TagParser = new Parser();

            kag.Start("Scenario\\Demo.ks");
            do
            {
                string input = Console.ReadLine();
                kag.IsSkip = true;
            } while (kag.CurrentReaderState != KAGReader.STATE_FINISH);
        }

        // Script Reader method and KAG module test function.
        public void Test_ReaderEventHandler()
        {
            Console.WriteLine("\n=========== Test ReaderEventHandler ===========\n");
            Console.WriteLine("\nInput \"q\" to end the test.\n");

            KAGReader kag = new KAGReader();

            kag.OnSkipTrigger += new KAGReader.ReaderEventHandler(this.Test_ReaderEventHandler1);
            kag.OnSkipTrigger += new KAGReader.ReaderEventHandler(this.Test_ReaderEventHandler2);
            string input = "";
            
            do
            {
                input = Console.ReadLine();
                kag.IsSkip = true;
            } while (!input.ToLower().Equals("q"));
        }
        public void Test_ReaderEventHandler1(KAGReader a_reader)
        {
            Console.WriteLine("Trigger Event 1.");
        }
        public void Test_ReaderEventHandler2(KAGReader a_reader)
        {
            Console.WriteLine("Trigger Event 2.");
        }
        public void Test_ConsoleModule()
        {
            ConsoleModule cm = new ConsoleModule();

            cm.AddContent("xxx");
            cm.AddContent("yyy");
            cm.AddContent("zzz");
            cm.ChangeLine();
            cm.AddContent("xcz");
            cm.AddContent("zcx");
            cm.ChangeLine();
            cm.AddContent("123");

            cm.Execute();
        }
        public void Test_JavaScriptModule()
        {
            JavaScriptModule jsm = new JavaScriptModule();
            string exp = "";
            // 
            Console.WriteLine("Expression result : {0}", jsm.Emb<int>("20") == 20);
            Console.WriteLine("Expression result : {0}", jsm.Emb<string>("20").Equals("20"));
            Console.WriteLine("Expression result : {0}", jsm.Emb<bool>("20 == 20"));

            // System variable
            exp = "var1 = 1";
            jsm.Eval(exp);
            Console.WriteLine("{0} : {1}", exp, jsm.Exist("var1"));

            exp = "f.var1 = 1";
            jsm.Eval(exp);
            Console.WriteLine("{0} : {1}", exp, jsm.Exist("f.var1"));

            exp = "sf.var1 = 1";
            jsm.Eval(exp);
            Console.WriteLine("{0} : {1}", exp, jsm.Exist("sf.var1"));

            exp = "tf.var1 = 1";
            jsm.Eval(exp);
            Console.WriteLine("{0} : {1}", exp, jsm.Exist("tf.var1"));

            exp = "ktf.var1 = 1";
            jsm.Eval(exp);
            Console.WriteLine("{0} : {1}", exp, jsm.Exist("ktf.var1"));

            exp = "tf.var2 = (var1 + f.var1) * 2 + sf.var1";
            Console.WriteLine("{0} : {1}", exp, jsm.Emb<int>(exp));

            // List 
            Console.WriteLine("List temp class variable : ");
            foreach (Jurassic.Library.PropertyNameAndValue key in jsm.tempVar.Properties)
            {
                Console.WriteLine("{0} : {1}", key.Name, key.Value);
            }
            
            // Clear
            Console.WriteLine("Clear system variable");
            jsm.ClearSystemVariable();
            Console.WriteLine("List system class variable : ");
            foreach (Jurassic.Library.PropertyNameAndValue key in jsm.SystemVar.Properties)
            {
                Console.WriteLine("{0} : {1}", key.Name, key.Value);
            }
        }
        public void Test_ScriptReader()
        {
            Console.WriteLine("\n=========== Test ScriptReader.LoadFile ===========\n");

            ScriptReader sr = new KAGReader();
            Console.WriteLine("scenario.ks loading : {0}", sr.LoadFile("scenario.ks"));
            Console.WriteLine("scenario2.ks loading : {0}", sr.LoadFile("scenario2.ks"));
            Console.WriteLine("scenario3.ks loading : {0}", sr.LoadFile("scenario3.ks"));
            Console.WriteLine("scenario.ks loading : {0}", sr.LoadFile("scenario.ks"));
        }

        // KAG Script parser method test function.
        public void Test_ParserRow()
        {
            Console.WriteLine("\n=========== Test ScriptReader.PraserRow ===========\n");

            ScriptReader sr = new KAGReader();
            string row = "";

            Console.WriteLine("\n---------- Bookmark");
            row = "****start";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            row = "*start|first day.";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            Console.WriteLine("\n---------- comment");
            row = ";start|first day.";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            row = ";;;start|first day.";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            Console.WriteLine("\n---------- command");
            row = "@start attribute1=value1 attribute2=value2";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            row = "@@start attribute1=value1 attribute2=value2";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            Console.WriteLine("\n---------- tag");
            row = "[cm][[xxxx[];[r][l]xxxx[[[]";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            Console.WriteLine("\n---------- content");
            row = "xxxxasdasdx[[xx[]x";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            Console.WriteLine("\n---------- content");
            row = "[[xxxxxxx";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));

            Console.WriteLine("\n---------- content");
            row = "[adsdaxx[xxx";
            Console.WriteLine("Row : {0}", row);
            Console.WriteLine("Result : {0}", sr.PraserRow(row));
        }
        public void Test_ParserBookmark()
        {
            Console.WriteLine("\n=========== Test Bookmark ===========\n");
        
            // default.[filename], file start page name.
            // default.[filename].[pagename], sub page in the file.
            // default.system, system base page.

            KAGReader sr = new KAGReader();
            sr.PraserRow("*start|first day");
            sr.PraserRow("*|second day");
            sr.PraserRow("*|third day");
            sr.PraserRow("*chapter1|story model");
            sr.PraserRow("*chapter2|");
            sr.PraserRow("*chapter3|");
            sr.PraserRow("*|");

            Console.WriteLine("Total page count : {0}", sr.PagesCount);
            Console.WriteLine("Page {0}, exist : {1}", "start", sr.GetPage(sr.GetPageName("start")) != null);
            Console.WriteLine("Page {0}, exist : {1}", "start:1", sr.GetPage(sr.GetPageName("start:1")) != null);
            Console.WriteLine("Page {0}, exist : {1}", "start:2", sr.GetPage(sr.GetPageName("start:2")) != null);
            Console.WriteLine("Page {0}, exist : {1}", "start:3", sr.GetPage(sr.GetPageName("start:3")) != null);

            Console.WriteLine("Page {0}, exist : {1}, content : {2}", "chapter1", sr.GetPage(sr.GetPageName("chapter1")) != null, ((SDPage)sr.GetPage(sr.GetPageName("chapter1"))).Label);
            Console.WriteLine("Page {0}, exist : {1}, content : {2}", "chapter2", sr.GetPage(sr.GetPageName("chapter2")) != null, ((SDPage)sr.GetPage(sr.GetPageName("chapter2"))).Label);
            Console.WriteLine("Page {0}, exist : {1}, content : {2}", "chapter3", sr.GetPage(sr.GetPageName("chapter3")) != null, ((SDPage)sr.GetPage(sr.GetPageName("chapter3"))).Label);
            Console.WriteLine("Page {0}, exist : {1}, content : {2}", "chapter3:1", sr.GetPage(sr.GetPageName("chapter3:1")) != null, ((SDPage)sr.GetPage(sr.GetPageName("chapter3:1"))).Label);
        }
        public void Test_ParserCommand()
        {
            Console.WriteLine("\n=========== Test Command ===========\n");

            KAGReader sr = new KAGReader();
            Tag t = new Tag();
            sr.PraserRow("@cmd1 attr1=  \"321 xxxxx yyyy\" attr2=true");
            sr.PraserRow("cmd3");
            sr.PraserRow("@cmd2 attr2 =true");
            sr.PraserRow("@cmd3 attr2= true");
            sr.PraserRow("@cmd4 attr2 = true attr2= false");
            sr.PraserRow("@cmd1  attr1=321 attr3 attr2=&var.member1 attr4");
            sr.PraserRow("@cmd2 attr1");
            sr.PraserRow("@cmd3 *");
            sr.PraserRow("@cmd4 target=\"call2\" exp     =       \"f.var1=123, f.var2=456, f.var1 = (f.var2 + f.var3) * 2\"");

            SDPage page = (SDPage)sr.GetPage(ScriptReader.DEFAULT_PAGE);
            IScriptData command = null;
            page.JumpTo(0);
            do
            {
                command = page.Current();
                Console.WriteLine("Command name {0}", command.Name);
                foreach (string key in command.Attribute.Keys)
                {
                    Console.WriteLine("Attribute {0} value {1} size : {6}\nN : {2}, B : {3}, V:{4}, D:{5}", 
                        key, 
                        command.Attribute[key], 
                        t.isNumberAttribute((string)command.Attribute[key]),
                        t.isBooleanAttribute((string)command.Attribute[key]),
                        t.isVariableAttribute((string)command.Attribute[key]),
                        t.isDefaultAttribute((string)command.Attribute[key]),
                        ((string)command.Attribute[key]).Length);
                }
            }
            while (page.Next() != command);
            /*
            string pattern = "([^ ]*=(?:\"[^=\"]*\"|[^= ]*))";
            string row = "cmd attr1=\"321 xxxxx\" attr2=123";
            Match match = Regex.Match(row, pattern);
            while (match.Success)
            {
                Console.WriteLine(row.Substring(match.Index, match.Length));
                match = match.NextMatch();
            }
            */
        }
        public void Test_ParserTagAndContent()
        {
            Console.WriteLine("\n=========== Test Tag and Content ===========\n");
            KAGReader sr = new KAGReader();
            sr.PraserRow("[cmd1 attr1=\"321 xxxxx\" attr2=123]abcdefg[cmd2][cmd3][cmd1  attr1=321 attr2=12][cmd2]");
            sr.PraserRow("abcdefg");
            sr.PraserRow("[[xxx[] saysaysaysay");

            SDPage page = (SDPage)sr.GetPage(ScriptReader.DEFAULT_PAGE);
            IScriptData data = null;
            page.JumpTo(0);
            do
            {
                data = page.Current();
                if (data.Type == SDContent.TYPE)
                {
                    Console.WriteLine("Content : {0}", data.Name);
                }
                else if(data.Type == SDCommand.TYPE )
                {
                    Console.WriteLine("Command name {0}", data.Name);
                    foreach (string key in data.Attribute.Keys)
                    {
                        Console.WriteLine("Attribute {0} value {1}", key, data.Attribute[key]);
                    }
                }
            }
            while (page.Next() != data);
        }

        // Script structure test function.
        public void Test_ScriptData()
        {
            Console.WriteLine("\n=========== Test SDPage ===========\n");
            IScriptData page1 = new SDPage( null, "scene1", "" );
            IScriptData page2 = new SDPage( null, "scene2", "" );
            IPageReader pagereader = (IPageReader)page1;
            IScriptData command = null;
            command = new SDCommand("command1", pagereader);
            command = new SDCommand("command2", pagereader);
            command = new SDCommand("command3", pagereader);

            Console.WriteLine("SDCommand type is {0}", command.Type);
            Console.WriteLine("SDCommand name is {0}", command.Name);
            Console.WriteLine("SDCommand have page attribute : {0}, and Page.Name is {1}", (command.Page == page1), ((IScriptData)command.Page).Name);

            Console.WriteLine("SDPage type is {0}", page1.Type);
            Console.WriteLine("SDPage name is {0}", page1.Name);
            Console.WriteLine("SDPage have {0} data in content.", page1.Content.Count);
            Console.WriteLine("SDPage have page attribute : " + (page1.Page == page1));
            Console.WriteLine("PageReader index {1}, data name is {0}", pagereader.Current().Name, pagereader.Index);
            Console.WriteLine("PageReader index {1}, data name is {0}", pagereader.Next().Name, pagereader.Index);
            Console.WriteLine("PageReader index {1}, data name is {0}", pagereader.Prev().Name, pagereader.Index);
            Console.WriteLine("PageReader index {1}, data name is {0}", pagereader.JumpTo(5).Name, pagereader.Index);
            Console.WriteLine("PageReader index {1}, data name is {0}", pagereader.JumpTo(-5).Name, pagereader.Index);

            Console.WriteLine("\nClone data :");
            pagereader = (SDPage)page2;
            command = command.Clone(pagereader);
            pagereader.Add(command);
            Console.WriteLine("SDCommand type is {0}", command.Type);
            Console.WriteLine("SDCommand name is {0}", command.Name);
            Console.WriteLine("SDCommand have page attribute : {0}, and Page.Name is {1}", (command.Page == page2), ((IScriptData)command.Page).Name);
            Console.WriteLine("SDPage2 have {0} data in content.", page2.Content.Count);
        }
        public void Test_TagContainer()
        {
            Console.WriteLine("\n=========== Test TagContainer ===========\n");
            TagContainer tag = new TagContainer("table");

            // Register
            Console.WriteLine("\n---------- Register");
            Console.WriteLine("Register tag1 : " + tag.Register(new Tag("tag1")));
            Console.WriteLine("Register tag2 : " + tag.Register(new Tag("tag2")));
            Console.WriteLine("Register tag1 : " + tag.Register(new Tag("tag1")));

            // Retrieve
            Console.WriteLine("\n---------- Retrieve");
            Console.WriteLine("Retrieve tag1 : " + tag.Retrieve("tag1"));
            Console.WriteLine("Retrieve tag2 : " + tag.Retrieve("tag2"));
            Console.WriteLine("Retrieve tag3 : " + tag.Retrieve("tag3"));

            // Has
            Console.WriteLine("\n---------- Has");
            Console.WriteLine("Has tag1 : " + tag.Has("tag1"));
            Console.WriteLine("Has tag2 : " + tag.Has("tag2"));
            Console.WriteLine("Has tag3 : " + tag.Has("tag3"));

            // Remove
            Console.WriteLine("\n---------- Remove");
            Console.WriteLine("Remove tag2 : " + tag.Remove("tag2"));
            Console.WriteLine("Remove tag2 : " + tag.Remove("tag2"));
        }
        public void Test_PairsTag()
        {
            Console.WriteLine("\n=========== Test PairsTag ===========\n");
            int i = 0, j = 0, k = 0;
            PairsTag pt = new PairsTag("TestPairs");
            SDPage page = new SDPage(null, "TEST_PAGE", "TEST_LABEL");
            page.Add(new ScriptData("TEST", "DATA1", page));
            page.Add(new ScriptData("TEST", "DATA2", page));
            page.Add(new ScriptData("TEST", "DATA3", page));
            page.Add(new ScriptData("TEST", "DATA4", page));
            page.Add(new ScriptData("TEST", "DATA5", page));
            ArrayList captureDataList = new ArrayList();
            ArrayList cloneDataList = new ArrayList();
            
            Console.WriteLine("\n---------- Capture");
            pt.CaptureDataList = captureDataList;
            pt.CaptureContent(page);
            for (i = 0; i < captureDataList.Count; i++ )
            {
                Console.WriteLine("{0} : {1}", i, ((ScriptData)captureDataList[i]).Name);
            }

            
            Console.WriteLine("\n---------- Clone Capture");
            pt.CaptureDataList = cloneDataList;
            page.JumpTo(1);
            pt.CaptureContent(page);
            pt.CloneCaptureData(captureDataList);
            for (i = 0; i < cloneDataList.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, ((ScriptData)cloneDataList[i]).Name);
            }
        }
    }
}