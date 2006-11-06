#include <stdafx.h>

namespace NSvn
{
    namespace Core
    {
        __gc __interface ICredential
        {
            void* GetCredential( const Pool& p );
        };
    }
}
