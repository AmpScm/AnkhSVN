#include <vcclr.h>
#using <mscorlib.dll>

namespace NSvn
{
    namespace Core
    {
        ///<summary>Class for wrapping a managed pointer and being able to 
        ///pass it to the svn APIs as a void pointer</summary>
        template <class T>
        class ManagedPointer
        {
        public:
            ManagedPointer( T ptr ) : ptr( ptr )
            { ; }

            ///<summary>Implicit conversion to the wrapped class</summary>
            operator T()
            {
                return ptr;
            }

            ///<summary>Implicit conversion to a void pointer</summary>
            operator void*()
            {
                return static_cast<void*>(this);
            }
        private:
            gcroot<T> ptr;

        };
    }
}