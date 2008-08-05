// dllmain.h : Declaration of module class.

class CAtlTestModule : public CAtlDllModuleT< CAtlTestModule >
{
public :
	DECLARE_LIBID(LIBID_AtlTestLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ATLTEST, "{41C35089-E170-4D76-BE92-C17FD9DCE767}")
};

extern class CAtlTestModule _AtlModule;
