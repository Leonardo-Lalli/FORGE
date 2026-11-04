using Android.App;
using Android.Runtime;

namespace Forge;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    // Keep startup on the concrete JNI wrapper so the packaged entry point
    // can be checked before distributing a Release APK.
    public override void OnCreate() => base.OnCreate();
}
