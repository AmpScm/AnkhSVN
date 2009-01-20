using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    public interface IAnkhEditorResolver
    {
        /// <summary>
        /// Tries to get the language service for the specified extension
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="languageService">The language service.</param>
        /// <returns></returns>
        bool TryGetLanguageService(string extension, out Guid languageService);
    }
}
