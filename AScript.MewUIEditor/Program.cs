using Aprillz.MewUI;
using AScript.MewUIEditor;
 
if (OperatingSystem.IsWindows())
{
    Win32Platform.Register();
    Direct2DBackend.Register();
}

Application
    .Create()
    .UseAccent(Accent.Blue) 
    .BuildMainWindow(() =>new ScriptEditor())
    .Run();
