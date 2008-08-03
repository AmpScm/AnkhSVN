//using System;
//using System.Collections.Generic;
//using System.Text;
//using MAPI;
//using System.Runtime.InteropServices;

//namespace ErrorReportExtractor
//{
//    class MapiFields
//    {
//        public MapiFields( Fields fields )
//        {
//            this.fields = fields;
//        }

//        public string AsString( object id )
//        {
//            return (string)this.Get( id );
//        }

//        public void TestMethod()
//        {
//            for ( int j = 1; j <= ( (int)fields.Count ); j++ )
//            {
//                Field field = (Field)fields.get_Item( j, Type.Missing );
//                Console.WriteLine( "MAPI Field {0:X}: {1}", field.ID, field.Value );
//            }
//        }


//        private object Get( object id )
//        {
//            try
//            {
//                Field field = (Field)fields.get_Item( id, Type.Missing );
//                return field.Value;
//            }
//            catch ( COMException )
//            {
//                return null;
//            }
//        }

//        private Fields fields;
//        public const int InReplyTo = 0x1042001E;

//    }
//}
