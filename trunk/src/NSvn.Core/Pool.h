#include <svn_pools.h>

namespace NSvn
{
    namespace Core
    {
        // Represents an svn/apr pool
        class Pool
        {
            // ctor creates a new apr pool
            Pool()
            {
                this->pool = svn_pool_create( 0 );
            }

            // ctor destroys the existing pool
            ~Pool()
            {
                svn_pool_destroy( this->pool );
            }

            // implicit conversion to an apr_pool_t*
            operator apr_pool_t*()
            {
                return this->pool; 
            }
                

        private:
            apr_pool_t* pool;
        };
    }
}