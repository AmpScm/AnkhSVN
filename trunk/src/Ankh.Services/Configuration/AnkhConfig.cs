﻿using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Microsoft.Win32;
using System.Security.AccessControl;
namespace Ankh.Configuration
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute("Config", Namespace = "http://ankhsvn.com/Config.xsd", IsNullable = false)]
    public class AnkhConfig : ICustomTypeDescriptor
    {
        private string mergeExePathField;
        private string diffExePathField;
        private string logMessageTemplateField = @"# All lines starting with a # will be ignored
# This template can be modified in Tools->Options->Source Control->Subversion.
*** # %path%";
        private bool chooseDiffMergeManualField = false;

        /// <remarks/>
        [Category("Diff/Merge")]
        [DefaultValue(null)]
        [Description(@"This command line will be used for spawning an external merge program. " +
    "The options are %base %theirs %mine %merged, which will be replaced with the respective paths when a merge is executed.")]
        [TypeConverter("Ankh.UI.UITypeEditors.NullableStringConverterProxy, Ankh.UI")]
        [Editor("Ankh.UI.UITypeEditors.MergeExeTypeEditor, Ankh.UI", typeof(UITypeEditor))]
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


        /// <remarks/>
        //[Editor( typeof( MultilineStringEditor ), typeof( UITypeEditor ) )]
        [DefaultValue(null)]
        [Category("Diff/Merge")]
        [Description(@"This command line will be used for spawning an external diff program. " +
            "The options are %base and %mine, which will be replaced with the respective paths when a diff is executed.")]
        [TypeConverter("Ankh.UI.UITypeEditors.NullableStringConverterProxy, Ankh.UI")]
        [Editor("Ankh.UI.UITypeEditors, Ankh.UI", typeof(UITypeEditor))]
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




        /// <remarks/>
        [Category("Log message")]
        [DefaultValue(@"# All lines starting with a # will be ignored
# This template can be modified in Tools->Options->Source Control->Subversion.
*** # %path%")]
        [Editor("Ankh.UI.UITypeEditors.LogMessageTypeEditor, Ankh.UI", typeof(UITypeEditor))]
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


        /// <remarks/>
        [DefaultValue(false)]
        [Category("Diff/Merge")]
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



        #region ICustomTypeDescriptor Members

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptorCollection GetEvents(System.Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
        {
            PropertyDescriptorCollection originalProps = TypeDescriptor.GetProperties(this, attributes, true);
            ConfigPropertyDescriptor[] newProps = new ConfigPropertyDescriptor[originalProps.Count];

            for (int i = 0; i < originalProps.Count; i++)
                newProps[i] = new ConfigPropertyDescriptor(originalProps[i]);

            return new PropertyDescriptorCollection(newProps);
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return GetProperties(null);
        }

        public object GetEditor(System.Type editorBaseType)
        {
            return null;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// A class to work around the problems with TypeConverters and Editors. Can also be used to localize/customize the property names.
        /// </summary>
        private class ConfigPropertyDescriptor : PropertyDescriptor
        {
            readonly PropertyDescriptor baseDescriptor;

            public ConfigPropertyDescriptor(PropertyDescriptor baseDescriptor)
                : base(baseDescriptor)
            {
                this.baseDescriptor = baseDescriptor;
            }

            public override TypeConverter Converter
            {
                get
                {
                    foreach (Attribute attr in this.AttributeArray)
                    {
                        TypeConverterAttribute typeConvAttr = attr as TypeConverterAttribute;
                        if (typeConvAttr != null)
                        {
                            Type converterType = this.GetTypeFromName(typeConvAttr.ConverterTypeName);
                            if (converterType != null)
                                return CreateInstance(converterType) as TypeConverter;
                        }
                    }

                    return baseDescriptor.Converter;
                }
            }


            public override object GetEditor(System.Type editorBaseType)
            {
                object e = base.GetEditor(editorBaseType);
                if (e == null)
                {
                    foreach (Attribute attr in this.AttributeArray)
                    {
                        EditorAttribute editorAttr = attr as EditorAttribute;
                        if (editorAttr != null && this.GetTypeFromName(editorAttr.EditorBaseTypeName) == editorBaseType)
                        {
                            Type type = this.GetTypeFromName(editorAttr.EditorTypeName);
                            if (type != null)
                            {
                                e = this.CreateInstance(type);
                                break;
                            }
                        }
                    }
                }
                return e;
            }

            /// <summary>
            /// Override the base implementation with this one, which is essentially the same, but fixes the
            /// Type.GetType search that goes wrong in the base implementation
            /// </summary>
            /// <param name="typeName"></param>
            /// <returns></returns>
            protected new Type GetTypeFromName(string typeName)
            {
                if (typeName == null || typeName.Length == 0)
                    return null;

                Type t = null;
                if (typeName.IndexOf(',') == -1)
                    t = ComponentType.Module.Assembly.GetType(typeName);
                if (t == null)
                    t = Type.GetType(typeName);
                return t;
            }

            public override bool CanResetValue(object component)
            {
                return this.baseDescriptor.CanResetValue(component);
            }

            public override System.Type ComponentType
            {
                get { return typeof(AnkhConfig); }
            }

            public override object GetValue(object component)
            {
                return this.baseDescriptor.GetValue(component);
            }

            public override bool IsReadOnly
            {
                get { return this.baseDescriptor.IsReadOnly; }
            }

            public override System.Type PropertyType
            {
                get { return this.baseDescriptor.PropertyType; }
            }

            public override void ResetValue(object component)
            {
                this.baseDescriptor.ResetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(baseDescriptor.PropertyType);
                if (converter == null || !converter.CanConvertFrom(value.GetType()))
                    this.baseDescriptor.SetValue(component, value);
                else
                {
                    baseDescriptor.SetValue(component, converter.ConvertFrom(value));
                }
            }

            public override bool ShouldSerializeValue(object component)
            {
                return this.baseDescriptor.ShouldSerializeValue(component);
            }
        }
        #endregion
    }
}