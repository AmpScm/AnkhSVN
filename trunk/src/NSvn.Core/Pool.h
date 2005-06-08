// $Id$
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
            // ctor creates a new top level apr pool
            Pool()
            {
                this->pool = svn_pool_create( 0 );
            }          


            // ctor destroys the existing pool if it owns it
            ~Pool()
            {
                if ( this->ownsPool )
                    svn_pool_destroy( this->pool );
            }

            // implicit conversion to an apr_pool_t*
            operator apr_pool_t*() const
            {
                return this->pool; 
            }

            /// <summary>Allocate memory on this pool</summary>
            void* Alloc( int size ) const
            { 
                return apr_palloc( this->pool, size );
            }

            /// <summary>Allocate memory on this pool, zeroing it before use</summary>
            void* PCalloc( int size ) const
            { 
                return apr_pcalloc( this->pool, size );
            }

            void Clear()
            {
                svn_pool_clear( this->pool );                    
            }



            /// <summary>Allocate an object on this pool. The object will be copied onto the 
            /// pool, so it must define a copy constructor. The pool will call the destructor
            /// of the object when the pool is destroyed</summary>
            template<class T>
                T* AllocateObject( const T& t ) const
            {
                return AllocateObject( t, this->pool );
            }

            /// <summary>Allocate an object in the given apr pool. The object will be copied onto the 
            /// pool, so it must define a copy constructor. The pool will call the destructor
            /// of the object when the pool is destroyed</summary>
            template<class T>
                static T* AllocateObject( const T& t, apr_pool_t* pool )
            {
                void* raw = apr_palloc( pool, sizeof( T ) );
                T* newT = new(raw)T(t);

                // make sure the dtor is called when the pool is destroyed
                apr_pool_cleanup_register( pool, newT, CallDestructor<T>, 
                    apr_pool_cleanup_null );

                return newT;
            }
        protected:
             // uses an existing pool as the parent - 
            Pool( apr_pool_t* parentPool )
            {
                this->pool = svn_pool_create( parentPool );
            }

        private:

            // don't allow copying or assignment
            Pool( Pool& other ){;}
            Pool& operator=( Pool& other){ return *this; }        

            bool ownsPool;

            apr_pool_t* pool;
        };

        class SubPool : public Pool
        {
        public:
            SubPool( apr_pool_t* parentPool ) : Pool(parentPool)
            {;}

            ~SubPool()
            {
                this->Clear();
            }
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
