﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SEOMacroscope {
	
	
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("ICSharpCode.SettingsEditor.SettingsCodeGeneratorTool", "5.1.0.5216")]
	internal sealed partial class MacroscopePreferences : global::System.Configuration.ApplicationSettingsBase {
		
		private static MacroscopePreferences defaultInstance = ((MacroscopePreferences)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new MacroscopePreferences())));
		
		public static MacroscopePreferences Default {
			get {
				return defaultInstance;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("False")]
		public bool CheckExternalLinks {
			get {
				return ((bool)(this["CheckExternalLinks"]));
			}
			set {
				this["CheckExternalLinks"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("False")]
		public bool CheckHreflangs {
			get {
				return ((bool)(this["CheckHreflangs"]));
			}
			set {
				this["CheckHreflangs"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("-1")]
		public int Depth {
			get {
				return ((int)(this["Depth"]));
			}
			set {
				this["Depth"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("True")]
		public bool FetchImages {
			get {
				return ((bool)(this["FetchImages"]));
			}
			set {
				this["FetchImages"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("True")]
		public bool FetchJavascripts {
			get {
				return ((bool)(this["FetchJavascripts"]));
			}
			set {
				this["FetchJavascripts"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("False")]
		public bool FetchPdfs {
			get {
				return ((bool)(this["FetchPdfs"]));
			}
			set {
				this["FetchPdfs"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("True")]
		public bool FetchStylesheets {
			get {
				return ((bool)(this["FetchStylesheets"]));
			}
			set {
				this["FetchStylesheets"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("True")]
		public bool FirstRun {
			get {
				return ((bool)(this["FirstRun"]));
			}
			set {
				this["FirstRun"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("True")]
		public bool FollowCanonicalLinks {
			get {
				return ((bool)(this["FollowCanonicalLinks"]));
			}
			set {
				this["FollowCanonicalLinks"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("False")]
		public bool FollowHrefLangLinks {
			get {
				return ((bool)(this["FollowHrefLangLinks"]));
			}
			set {
				this["FollowHrefLangLinks"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("False")]
		public bool FollowNoFollow {
			get {
				return ((bool)(this["FollowNoFollow"]));
			}
			set {
				this["FollowNoFollow"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("False")]
		public bool FollowRedirects {
			get {
				return ((bool)(this["FollowRedirects"]));
			}
			set {
				this["FollowRedirects"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("True")]
		public bool FollowRobotsProtocol {
			get {
				return ((bool)(this["FollowRobotsProtocol"]));
			}
			set {
				this["FollowRobotsProtocol"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("")]
		public string HttpProxyHost {
			get {
				return ((string)(this["HttpProxyHost"]));
			}
			set {
				this["HttpProxyHost"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("0")]
		public int HttpProxyPort {
			get {
				return ((int)(this["HttpProxyPort"]));
			}
			set {
				this["HttpProxyPort"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("32")]
		public int MaxFetchesPerWorker {
			get {
				return ((int)(this["MaxFetchesPerWorker"]));
			}
			set {
				this["MaxFetchesPerWorker"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("8")]
		public int MaxThreads {
			get {
				return ((int)(this["MaxThreads"]));
			}
			set {
				this["MaxThreads"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("-1")]
		public int PageLimit {
			get {
				return ((int)(this["PageLimit"]));
			}
			set {
				this["PageLimit"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("")]
		public string StartUrl {
			get {
				return ((string)(this["StartUrl"]));
			}
			set {
				this["StartUrl"] = value;
			}
		}
	}
}
