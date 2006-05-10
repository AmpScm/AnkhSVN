﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 



using Ankh.UI;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows.Forms.Design;
using System.Drawing.Design;
namespace Ankh.Config
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    [System.Xml.Serialization.XmlRootAttribute( Namespace = "http://ankhsvn.com/Config.xsd", IsNullable = false )]
    public class Config
    {

        private ConfigRepositoryExplorer repositoryExplorerField;

        private string mergeExePathField;

        private string diffExePathField;

        private string logMessageTemplateField = "";

        private bool autoAddNewFilesField = true;

        private bool autoReuseCommentField = false;

        private bool chooseDiffMergeManualField = false;

        private bool disableSolutionReloadField = false;

        private ConfigSubversion subversionField;

        /// <remarks/>
        [TypeConverter( typeof( ExpandableObjectConverter ) )]
        [Browsable( false )]
        public ConfigRepositoryExplorer RepositoryExplorer
        {
            get
            {
                return this.repositoryExplorerField;
            }
            set
            {
                this.repositoryExplorerField = value;
            }
        }

        /// <remarks/>
        [Category( "Diff/Merge" )]
        [DefaultValue( null )]
        [Description( @"This command line will be used for spawning an external merge program. " +
    "The options are %base %theirs %mine %merged, which will be replaced with the respective paths when a merge is executed." )]
        [TypeConverter( typeof( Config.NullableStringConverterProxy ) )]
        [Editor( typeof( Config.MergeExeTypeEditor ), typeof( UITypeEditor ) )]
        public string MergeExePath
        {
            get
            {
                return this.mergeExePathField;
            }
            set
            {
                this.mergeExePathField = value;
            }
        }

        private class MergeExeTypeEditor : StringTypeEditorWithTemplates
        {
            public MergeExeTypeEditor()
            {
                this.SetTitle( "MergeExe" );
            }

            protected override StringEditorTemplate[] GetTemplates()
            {
                return new StringEditorTemplate[]{
                        new StringEditorTemplate("%mine", "My version", "My version (%mine)"),
                        new StringEditorTemplate("%base", "The base version", "The base version (%base)"),
                        new StringEditorTemplate("%yours", "The other version", "The other version (%yours)"),
                        new StringEditorTemplate("%merged", "The output file", "The output file (%merged)")
                        };
            }
        }

        /// <remarks/>
        //[Editor( typeof( MultilineStringEditor ), typeof( UITypeEditor ) )]
        [DefaultValue( null )]
        [Category( "Diff/Merge" )]
        [Description( @"This command line will be used for spawning an external diff program. " +
            "The options are %base and %mine, which will be replaced with the respective paths when a diff is executed." )]
        [TypeConverter( typeof( Config.NullableStringConverterProxy) )]
        [Editor( typeof( Config.DiffExeTypeEditor ), typeof( UITypeEditor ) )]
        public string DiffExePath
        {
            get
            {
                return this.diffExePathField;
            }
            set
            {
                this.diffExePathField = value;
            }
        }

        private class DiffExeTypeEditor : StringTypeEditorWithTemplates
        {
            public DiffExeTypeEditor()
            {
                this.SetTitle( "DiffExe" );
            }

            protected override StringEditorTemplate[] GetTemplates()
            {
                return new StringEditorTemplate[]{
                        new StringEditorTemplate("%base", "The base version", "The base version (%base)"),
                        new StringEditorTemplate("%mine", "My version", "My version (%mine)"),
                        };
            }
        }


        /// <remarks/>
        [Editor( typeof( Config.LogMessageTypeEditor), typeof( UITypeEditor ) )]
        [Category( "Log message" )]
        [DefaultValue( "" )]
        [Editor( typeof( Config.LogMessageTypeEditor ), typeof( UITypeEditor ) )]
        public string LogMessageTemplate
        {
            get
            {
                return this.logMessageTemplateField;
            }
            set
            {
                this.logMessageTemplateField = value;
            }
        }

        private class LogMessageTypeEditor : StringTypeEditorWithTemplates
        {
            public LogMessageTypeEditor()
            {
                this.SetTitle( "LogMessage" );
            }

            protected override StringEditorTemplate[] GetTemplates()
            {
                return new StringEditorTemplate[]{
                        new StringEditorTemplate("#", "Comment", "Comment (#)"),
                        new StringEditorTemplate("***", "For each file", "For each file (***)"),
                        new StringEditorTemplate("%path%", "The file path", "The file path (%path%)")
                    };
            }
        }

        /// <remarks/>
        [DefaultValue( true )]
        [Description( @"Whether new files should be automatically added to Subversion." )]
        public bool AutoAddNewFiles
        {
            get
            {
                return this.autoAddNewFilesField;
            }
            set
            {
                this.autoAddNewFilesField = value;
            }
        }

        /// <remarks/>
        [DefaultValue( false )]
        [Category( "Log message" )]
        [Description( @"Whether to automatically reuse the last comment if a commit failed." )]
        public bool AutoReuseComment
        {
            get
            {
                return this.autoReuseCommentField;
            }
            set
            {
                this.autoReuseCommentField = value;
            }
        }
        /// <remarks/>
        [DefaultValue( false )]
        [Category( "Diff/Merge" )]
        public bool ChooseDiffMergeManual
        {
            get
            {
                return this.chooseDiffMergeManualField;
            }
            set
            {
                this.chooseDiffMergeManualField = value;
            }
        }

        /// <remarks/>
        [DefaultValue( false )]
        [Description( @"Whether AnkhSVN should offer to automatically reload a solution if a project file is modified." )]
        public bool DisableSolutionReload
        {
            get
            {
                return this.disableSolutionReloadField;
            }
            set
            {
                this.disableSolutionReloadField = value;
            }
        }

        /// <remarks/>
        [Browsable( false )]
        public ConfigSubversion Subversion
        {
            get
            {
                if ( this.subversionField == null )
                {
                    this.subversionField = new ConfigSubversion();
                }
                return this.subversionField;
            }
            set
            {
                this.subversionField = value;
            }
        }

        [Category( "Subversion" )]
        [XmlIgnore]
        [Description( @"The path to the directory to use for the Subversion configuration data. " +
            "If not specified, it will use the same config area as other Subversion tools, " +
            "usually %APPDATA%\\Subversion. You can use environment variables in the path, quoted with %." )]
        [Editor( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
        [TypeConverter( typeof( Config.NullableStringConverterProxy) )]
        [DefaultValue( (string)null )]
        public string ConfigDir
        {
            get { return this.Subversion.ConfigDir; }
            set { this.Subversion.ConfigDir = value; }
        }

        [Category( "Subversion" )]
        [XmlIgnore]
        [Description( @"This path will be used for spawning svn.exe through the public " +
    "svn command in the VS.NET command window. It is optional. " +
    "If left out, Ankh will attempt to spawn svn.exe from PATH." )]
        [Editor( typeof( FileNameEditor ), typeof( UITypeEditor ) )]
        [DefaultValue( (string)null )]
        [TypeConverter( typeof( Config.NullableStringConverterProxy ) )]
        public string SvnExePath
        {
            get { return this.Subversion.SvnExePath; }
            set { this.Subversion.SvnExePath = value; }
        }

        [Category( "Subversion" )]
        [XmlIgnore]
        [Description( @"This is the name of the administrative subdirectory used by " +
    "Subversion to maintain metadata for the working copy. You should " +
    "*ONLY* modify this if you know what you are doing and you are " +
    "experiencing specific problems with Subversion's use of \".svn\" for " +
    @"the meta-data directory. 
Since Subversion 1.3, there is an environment variable (called SVN_ASP_DOT_NET_HACK)" +
    "you can set in order to switch *ALL* svn clients to the _svn directory. Since that release " +
    "the AdminDirectoryName serves as an override (if set), and may *only* contain _svn or .svn." )]
        [TypeConverter( typeof( Config.AdminDirectoryNameTypeConverter ) )]
        [DefaultValue( (string)null )]
        public string AdminDirectoryName
        {
            get { return this.Subversion.AdminDirectoryName; }
            set
            {
                this.Subversion.AdminDirectoryName = value;
            }
        }

        private class AdminDirectoryNameTypeConverter : StandardStringsTypeConverter
        {
            public AdminDirectoryNameTypeConverter()
                : base( new string[] { ".svn", "_svn", null }, true )
            {

            }
        }

        // dunno why this is necessary, but it doesn't work to use TypeConverters defined
        // in another assembly inside VS.
        private class NullableStringConverterProxy : NullableStringTypeConverter {}
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    public class ConfigRepositoryExplorer
    {

        private ConfigRepositoryExplorerUrl[] mruUrlsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute( "Url" )]
        public ConfigRepositoryExplorerUrl[] MruUrls
        {
            get
            {
                return this.mruUrlsField;
            }
            set
            {
                this.mruUrlsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    public class ConfigRepositoryExplorerUrl
    {

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute( "code" )]
    public class ConfigSubversion
    {

        private string configDirField;

        private string svnExePathField;

        private string adminDirectoryNameField;

        /// <remarks/>
        [DefaultValue( null )]
        public string ConfigDir
        {
            get
            {
                return this.configDirField;
            }
            set
            {
                this.configDirField = value;
            }
        }

        /// <remarks/>
        [DefaultValue( null )]
        public string SvnExePath
        {
            get
            {
                return this.svnExePathField;
            }
            set
            {
                this.svnExePathField = value;
            }
        }

        /// <remarks/>
        [DefaultValue( null )]
        public string AdminDirectoryName
        {
            get
            {
                return this.adminDirectoryNameField;
            }
            set
            {
                this.adminDirectoryNameField = value;
            }
        }
    }
}