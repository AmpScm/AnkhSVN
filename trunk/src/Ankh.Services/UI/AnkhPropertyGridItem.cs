using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Ankh.Scc
{
    /// <summary>
    /// Base class for classes that are designed to be shown in the VS Property grid
    /// </summary>
    public abstract class AnkhPropertyGridItem : CustomTypeDescriptor
    {
        /// <summary>
        /// Gets the light/second name shown in the gridview header
        /// </summary>
        [Browsable(false)]
        protected abstract string ClassName
        {
            get;
        }

        /// <summary>
        /// Gets the bold/first name shown in the gridview header
        /// </summary>
        [Browsable(false)]
        protected abstract string ComponentName
        {
            get;
        }


        /// <summary>
        /// Returns the name of the class represented by this type descriptor.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing the name of the component instance this type descriptor is describing. The default is null.
        /// </returns>
        public override sealed string GetComponentName()
        {
            return ComponentName;
        }

        /// <summary>
        /// Returns the fully qualified name of the class represented by this type descriptor.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing the fully qualified class name of the type this type descriptor is describing. The default is null.
        /// </returns>
        public override sealed string GetClassName()
        {
            return ClassName;
        }

        TypeConverter _rawDescriptor;
        TypeConverter Raw
        {
            get { return _rawDescriptor ?? (_rawDescriptor = TypeDescriptor.GetConverter(this, true)); }
        }

        /// <summary>
        /// Returns a collection of property descriptors for the object represented by this type descriptor.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            return Raw.GetProperties(this);
        }

        /// <summary>
        /// Returns a type converter for the type represented by this type descriptor.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.TypeConverter"/> for the type represented by this type descriptor. The default is a newly created <see cref="T:System.ComponentModel.TypeConverter"/>.
        /// </returns>
        public override TypeConverter GetConverter()
        {
            return Raw;
        }

        /// <summary>
        /// Returns a filtered collection of property descriptors for the object represented by this type descriptor.
        /// </summary>
        /// <param name="attributes">An array of attributes to use as a filter. This can be null.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return Raw.GetProperties(null, null, attributes);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return ClassName;
        }

        /// <summary>
        /// Returns an object that contains the property described by the specified property descriptor.
        /// </summary>
        /// <param name="pd">The property descriptor for which to retrieve the owning object.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that owns the given property specified by the type descriptor. The default is null.
        /// </returns>
        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }
}
