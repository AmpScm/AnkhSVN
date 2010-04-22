using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Fines.Utils.Collections;
using System.Text.RegularExpressions;

namespace ErrorReportExtractor
{
    public class Pop3ReadItemsCache
    {
        private Pop3ReadItemsCache( string file )
        {
            this.file = file;
            this.items = new Dictionary<string, object>();
            this.ReadItemsFromFile();
        }

       
        public static Pop3ReadItemsCache LoadFromFile( string file )
        {
            if ( !File.Exists(file) )
            {
                CreateEmptyCacheFile( file );
            }
            return new Pop3ReadItemsCache( file );
        }

        public IEnumerable<Pop3Entry> GetUnreadItems( string uidlResponse )
        {
            IList<Pop3Entry> entries = Pop3Entry.Parse( uidlResponse );
            foreach ( Pop3Entry entry in entries )
            {
                if ( entry != Pop3Entry.Invalid && !this.items.ContainsKey(entry.uidl))
                {
                    yield return entry;
                }
            }
        }

        public void MarkAsRead( Pop3Entry entry )
        {
            this.items[ entry.uidl ] = Type.Missing;
        }

        public void Save()
        {
            string[] uidls = ListUtils.ToArray( this.items.Keys );
            Serialize( this.file, uidls );
        }

        private static void CreateEmptyCacheFile( string file )
        {
            string[] empty = new string[] { };
            Serialize( file, empty );
        }

        private static void Serialize( string file, string[] items )
        {
            XmlSerializer serialiser = new XmlSerializer( typeof( string[] ) );
            using ( StreamWriter writer = new StreamWriter( file ) )
            {
                serialiser.Serialize( writer, items );
            }
        }

        private void ReadItemsFromFile()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( string[] ) );
            string[] stringItems = null;
            using ( StreamReader reader = new StreamReader( this.file ) )
            {
                stringItems = serializer.Deserialize( reader ) as string[];
            }
            if ( stringItems == null )
            {
                throw new Exception( "Could not read items from file " + this.file );
            }
            
            foreach ( string item in stringItems )
            {
                this.items[ item ] = Type.Missing;
            }
        }

        public class Pop3Entry
        {
            public int MessageId { get { return this.messageId; } }

            public Pop3Entry( int messageId, string uidl )
            {
                this.messageId = messageId;
                this.uidl = uidl;
            }

            public static IList<Pop3Entry> Parse( string uidlResponse )
            {
                string[] uidlResponseLines = uidlResponse.Split( new string[] { Environment.NewLine }, 
                    StringSplitOptions.RemoveEmptyEntries );
                return ListUtils.Map<string, Pop3Entry>( uidlResponseLines, delegate( string s )
                {
                    return ParseSingleEntry( s );
                } );
            }

            private static Pop3Entry ParseSingleEntry( string s )
            {
                Match match = UidlRegex.Match( s );
                if ( match == Match.Empty )
                {
                    return Pop3Entry.Invalid;
                }
                int messageId = Int32.Parse( match.Groups[ 1 ].Value );
                return new Pop3Entry( messageId, match.Groups[ 2 ].Value );
            }

            public static Pop3Entry Invalid = new Pop3Entry( -1, "" );

            private int messageId;
            internal string uidl;
            private static readonly Regex UidlRegex = new Regex(@"(\d+)\s+(.+)");
        }

        private string file;
        private Dictionary<string, object> items;
    }

    
}
