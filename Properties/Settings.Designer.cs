﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoMuteUs_Portable.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string EnvPath {
            get {
                return ((string)(this["EnvPath"]));
            }
            set {
                this["EnvPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("theGreatSchism")]
        public string AUTOMUTEUS_TAG {
            get {
                return ((string)(this["AUTOMUTEUS_TAG"]));
            }
            set {
                this["AUTOMUTEUS_TAG"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("theGreatSchism")]
        public string GALACTUS_TAG {
            get {
                return ((string)(this["GALACTUS_TAG"]));
            }
            set {
                this["GALACTUS_TAG"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("main")]
        public string WINGMAN_TAG {
            get {
                return ((string)(this["WINGMAN_TAG"]));
            }
            set {
                this["WINGMAN_TAG"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("v7")]
        public string ARCHITECTURE {
            get {
                return ((string)(this["ARCHITECTURE"]));
            }
            set {
                this["ARCHITECTURE"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public string AUTOMUTEUS_AUTORESTART {
            get {
                return ((string)(this["AUTOMUTEUS_AUTORESTART"]));
            }
            set {
                this["AUTOMUTEUS_AUTORESTART"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public string GALACTUS_AUTORESTART {
            get {
                return ((string)(this["GALACTUS_AUTORESTART"]));
            }
            set {
                this["GALACTUS_AUTORESTART"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public string WINGMAN_AUTORESTART {
            get {
                return ((string)(this["WINGMAN_AUTORESTART"]));
            }
            set {
                this["WINGMAN_AUTORESTART"] = value;
            }
        }
    }
}
