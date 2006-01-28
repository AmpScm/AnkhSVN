using System;
using System.Text;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace Utils.Tests
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [Guid("AB88F6EC-42E9-4054-8924-D9A5D3FBFC1D")]
    public interface IFoo
    {
        void Method();
    }

    // These are the same interface
    [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    [ComVisible( true )]
    [Guid( "AB88F6EC-42E9-4054-8924-D9A5D3FBFC1D" )]
    public interface IOtherFoo
    {
        void Method();
    }

    public class Foo : IFoo
    {
        public void Method()
        {
            Called = true;
        }

        public bool Called = false;
    }

    [TestFixture]
    public class ComUtilsTests
    {
        [Test]
        public void Wrap()
        {
            Foo foo = new Foo();
            object wrapper = ComUtils.Wrap( foo );
            Assert.IsTrue( wrapper is IOtherFoo, 
                "Wrapped object does not respond to QI for IOtherFoo interface" );
            IOtherFoo otherFoo = wrapper as IOtherFoo;
            otherFoo.Method();
            Assert.IsTrue( foo.Called, "Foo's method Method() not called" );
        }

        
    }
}
