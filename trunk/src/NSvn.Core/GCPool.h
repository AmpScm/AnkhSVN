// $Id$
#pragma once


#include "Pool.h"
#include "Stdafx.h"
#ifndef POST_DOTNET11
#include "_vcclrit.h"
#endif

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
                InitializeCrt();
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
            static bool _initialized = false;
            static System::Object *_lock = new System::Object();
            static void InitializeCrt()
            {
                System::Threading::Monitor::Enter(_lock);
                __try
                {
                    if(!_initialized)
                    {
#ifndef POST_DOTNET11
                        __crt_dll_initialize();
#endif

                        _initialized = true;
                    }
                }
                __finally
                {
                    System::Threading::Monitor::Exit(_lock);
                }
            };
        };

    }
}
