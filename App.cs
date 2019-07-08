using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AuroraServer
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
	internal sealed class App : ApplicationSettingsBase
	{
		private static App defaultInstance = (App)SettingsBase.Synchronized(new App());

		public static App Default => defaultInstance;

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool UseProtect
		{
			get
			{
				return (bool)this["UseProtect"];
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool UseOldMode
		{
			get
			{
				return (bool)this["UseOldMode"];
			}
			set
			{
				this["UseOldMode"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("127.0.0.1")]
		public string SQL_HOST
		{
			get
			{
				return (string)this["SQL_HOST"];
			}
			set
			{
				this["SQL_HOST"] = value;
			}
		}
	}
}
