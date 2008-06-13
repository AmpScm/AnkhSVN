// dllmain.h : Declaration of module class.

class CCppAtlProjectModule : public CAtlDllModuleT< CCppAtlProjectModule >
{
public :
	DECLARE_LIBID(LIBID_CppAtlProjectLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_CPPATLPROJECT, "{FD6C6F22-63B6-4708-ABC7-A8DE19115DED}")
};

extern class CCppAtlProjectModule _AtlModule;
