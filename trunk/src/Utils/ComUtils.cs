using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Utils
{
    public sealed class ComUtils
    {
        private ComUtils()
        {
            // Nothing to see here, move along
        }

        static ComUtils()
        {
            AggregatorType = Type.GetTypeFromProgID( "Ankh.ComAggregator" );
        }

        public static object Wrap( object obj )
        {
            IAnkhComAggregator aggr = (IAnkhComAggregator)Activator.CreateInstance( AggregatorType );
            aggr.SetObject( obj );
            return aggr;
        }

        [Guid( "7CEE7D1B-5E02-4546-9283-1A49408EFD51" )]
        [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
        public interface IAnkhComAggregator
        {
            //void Foo();
            void SetObject( [MarshalAs( UnmanagedType.IUnknown )]object o );
        }

        private static readonly Type AggregatorType;

    }
}
