*title
本腳本為巨集操作測試檔[r]
*start
[macro name="testmacro1"]
[delay speed=%sp|100]
巨集段落1[r]
巨集段落2[r]
巨集段落3[r]
[endmacro]
[macro name="testmacro2"]
[delay *]
巨集段落1x[r]
巨集段落2x[r]
巨集段落3x[r]
[endmacro]
以下測試巨集命令[r]
[testmacro1]
[testmacro1 sp=200]
[testmacro2 speed=300]
[erasemacro name="testmacro1"]
[testmacro1]
