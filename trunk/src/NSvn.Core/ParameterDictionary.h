#pragma once
#include "stdafx.h"
#include <apr_hash.h>
#include "Pool.h"
#include "ManagedPointer.h"

namespace NSvn
{
    namespace Core
    {
        static const char* DICTIONARYKEY = "NSVN:IDICTIONARY";

        using namespace System;
        using namespace System::Collections;

        __gc class ParameterDictionary : public NSvn::Common::ParameterDictionaryBase
        {
        public:
            ParameterDictionary( apr_hash_t* aprHash, apr_pool_t* pool ) : 
              aprHash( aprHash ), pool( pool ), innerDictionary(0)
            {;}           


        protected:
            __property IDictionary* get_InnerDictionary()
            {
                // is it already cached?
                if ( this->innerDictionary == 0 )
                {
                    // nope - we have to look in the apr hash
                    void* ptr = apr_hash_get( this->aprHash, DICTIONARYKEY,
                        APR_HASH_KEY_STRING );

                    // not there either? we have to create it
                    if ( ptr == 0 )
                        this->CreateInnerDictionary( );
                    else
                    {
                        // get it from the apr hash
                        this->innerDictionary = *static_cast<ManagedPointer<IDictionary*>*>(
                            ptr );
                    }
                }

                return this->innerDictionary;
            }

        private:            
            void CreateInnerDictionary( )
            {
                this->innerDictionary = new Hashtable();

                // put it in the apr hash
                ManagedPointer<IDictionary*>* ptr = Pool::AllocateObject( 
                    ManagedPointer<IDictionary*>(this->innerDictionary), this->pool );
                apr_hash_set( this->aprHash, DICTIONARYKEY, APR_HASH_KEY_STRING,
                    ptr );
            }

            IDictionary* innerDictionary;
            apr_hash_t* aprHash;
            apr_pool_t* pool;
            
        };
    }
}
