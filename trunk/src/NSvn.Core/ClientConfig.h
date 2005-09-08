// $Id$
#using <mscorlib.dll>

#include <svn_config.h>
#include <apr_hash.h>
#include "SvnClientException.h"
#include "GCPool.h"

namespace NSvn
{
    namespace Core
    {
        using namespace System;

        public __gc class ClientConfig
        {
        public:
            ClientConfig()
            {
                this->Init( 0 );
            }

            ClientConfig( String* configDir )
            {
                this->Init( configDir );
            }

            /// <summary>Set a value for the given option in the given section</summary>
            void Set( String* section, String* option, String* value )
            {
				svn_config_set( this->config, StringToUtf8(section, this->pool->ToAprPool()), 
					StringToUtf8(option, this->pool->ToAprPool()), 
					StringToUtf8(value, this->pool->ToAprPool()) );
            }

            /// <summary>Retrieve the value for option in the given section</summary>
            String* Get( String* section, String* option, String* defaultValue )
            {
                const char* val;
				svn_config_get( this->config, &val, StringToUtf8(section, this->pool->ToAprPool()),
					StringToUtf8(option, this->pool->ToAprPool()), 
					StringToUtf8(defaultValue, this->pool->ToAprPool()) );

				return Utf8ToString(val, this->pool->ToAprPool());
            }

            /// <summary>Create a subversion configuration directory at path.</summary>
            static void CreateConfigDir( String* path )
            {
                Pool pool;
                HandleError( svn_config_ensure( StringToUtf8(path, pool), pool ) );
            }

        private public:
            apr_hash_t* GetHash()
            {
                return this->configs; 
            }

        private:

            void Init( String* dir )
            {
                this->pool = new GCPool();

				const char* configDir = StringToUtf8(dir, pool->ToAprPool());

                // get the hash containing all the configs
                apr_hash_t* configs;
                HandleError( svn_config_get_config( &configs, configDir, this->pool->ToAprPool() ) );

                this->configs = configs;

                // get the "config" config
                this->config = static_cast<svn_config_t*>( 
                      apr_hash_get( this->configs, SVN_CONFIG_CATEGORY_CONFIG,
                      APR_HASH_KEY_STRING ) );                
            }



            apr_hash_t* configs;
            svn_config_t* config;
            GCPool* pool;

        };
    }
}
