// $Id$
#pragma once


#include "Pool.h"
#include "Stdafx.h"


namespace NSvn
{
    namespace Core
    {
        /// <summary>A wrapper for apr_pool_t that is kept alive on the managed heap</summary>


        public __gc class GCPool : public System::IDisposable
        {
        private public:
            GCPool()
            {
                this->pool = new Pool();
            }
        
            apr_pool_t* ToAprPool()
            {
                return static_cast<apr_pool_t*>(*this->pool);
            }

            Pool* GetPool()
            {
                return this->pool; 
            }
       public:
            void Dispose()
            {
                delete this->pool;
                System::GC::SuppressFinalize( this );
            }

            ~GCPool()
            {
                this->Dispose();
            }
        
        private:


            Pool* pool;
        };

    }
}
