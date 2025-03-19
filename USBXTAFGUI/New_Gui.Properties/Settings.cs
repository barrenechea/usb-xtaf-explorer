using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace New_Gui.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool Firstrun
	{
		get
		{
			return (bool)this["Firstrun"];
		}
		set
		{
			this["Firstrun"] = value;
		}
	}
}
