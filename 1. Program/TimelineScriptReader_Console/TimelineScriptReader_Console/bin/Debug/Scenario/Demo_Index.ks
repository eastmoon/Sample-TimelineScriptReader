*title
本腳本為索引操作測試檔
*start
[cm]
跳躍命令測試，下面一句將被忽略[waitclick]
[jump storage target="start:1"]
本句被忽略
*|
[cm]
跳躍至start1成功[r]
呼叫Call[waitclick]
[call storage target="call1"]
[cm]
[click storage target="call2" exp="f.var = f.var1 + f.var2"]
點擊跳躍測試，點擊後才跳躍。
[s]
本句被忽略
*call1
[cm]
這是Call function[l]
[return]
*call2
[cm]
點擊跳躍完成
[cclick]
[cm]
[timeout storage time=2000 target="call3" exp="f.var = f.var1 + f.var2"]
時間軸跳躍，等待2秒。
[s]
本句被忽略
*call3
[ctimeout]
[cm]
點擊，讓腳本跳躍到範例操作首頁[waitclick]
[jump storage="Scenario\Demo.ks"]
