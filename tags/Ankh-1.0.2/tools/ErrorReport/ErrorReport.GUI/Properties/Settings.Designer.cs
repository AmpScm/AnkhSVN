﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ErrorReport.GUI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Lucida Console, 8.25pt")]
        public global::System.Drawing.Font ReplyFont {
            get {
                return ((global::System.Drawing.Font)(this["ReplyFont"]));
            }
            set {
                this["ReplyFont"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Lucida Console, 8.25pt")]
        public global::System.Drawing.Font MessageFont {
            get {
                return ((global::System.Drawing.Font)(this["MessageFont"]));
            }
            set {
                this["MessageFont"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Tahoma, 8.25pt")]
        public global::System.Drawing.Font BaseFont {
            get {
                return ((global::System.Drawing.Font)(this["BaseFont"]));
            }
            set {
                this["BaseFont"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=sherilyn\\sqlexpress;Initial Catalog=ErrorReports-test;Integrated Secu" +
            "rity=True")]
        public string ConnectionString {
            get {
                return ((string)(this["ConnectionString"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string OutlookFolder {
            get {
                return ((string)(this["OutlookFolder"]));
            }
            set {
                this["OutlookFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Data.DataTable LastOutlookReplyIndexChecked {
            get {
                return ((global::System.Data.DataTable)(this["LastOutlookReplyIndexChecked"]));
            }
            set {
                this["LastOutlookReplyIndexChecked"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Data.DataTable LastOutlookReportIndexChecked {
            get {
                return ((global::System.Data.DataTable)(this["LastOutlookReportIndexChecked"]));
            }
            set {
                this["LastOutlookReportIndexChecked"] = value;
            }
        }
    }
}
