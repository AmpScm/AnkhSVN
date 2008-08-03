using System;
using System.Collections.Generic;
using System.Text;

namespace Fines.IssueZillaLib
{
    public interface IMetadataSource
    {
        IList<MetadataItem<string, string>> Versions { get;}
        IList<MetadataItem<string, string>> IssueTypes { get;}
        IList<MetadataItem<string, string>> Components { get;}
        IList<MetadataItem<string, string>> SubComponents { get;}
        IList<MetadataItem<string, string>> OperatingSystems { get;}
        IList<MetadataItem<string, string>> Platforms { get;}
        IList<MetadataItem<string, string>> Priorities { get;}
        IList<MetadataItem<string, string>> Resolutions { get;}


        void LoadMetaData();
    }
}
