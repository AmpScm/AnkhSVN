using System;
namespace ErrorReportExtractor
{
    public interface IServiceProvider
    {
        T GetService<T>() where T : IService;
        void ProfferService<T>( T t ) where T : IService;

        void SetProgressCallback( IProgressCallback callback );
    }
}
