#pragma once
#include <svn_pools.h>
#include <new>


namespace
{
    template<class T>
    static apr_status_t CallDestructor( void* data );
}



namespace NSvn
{
    
    namespace Core
    {

        // Represents an svn/apr pool
        class Pool
        {
        public:
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
            operator apr_pool_t*() const
            {
                return this->pool; 
            }

            template<class T>
            T* Allocate( const T& t ) const
            {
                void* raw = apr_palloc( this->pool, sizeof( T ) );
                T* newT = new(raw)T(t);

                // make sure the dtor is called when the pool is destroyed
                apr_pool_cleanup_register( this->pool, newT, CallDestructor<T>, 
                    apr_pool_cleanup_null );

                return newT;
            }

                

        private:

            // don't allow copying or assignment
            Pool( Pool& other ){;}
            Pool& operator=( Pool& other){ return *this; }        

            

            apr_pool_t* pool;
        };
    }
}

namespace
{
    template <class T>
    static apr_status_t CallDestructor( void* data )
    {
        //call the dtor explicitly
        T* t = static_cast<T*>(data);
        t->~T();

        return APR_SUCCESS;
    }
}