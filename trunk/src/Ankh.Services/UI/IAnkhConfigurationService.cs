using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Configuration;

namespace Ankh.UI
{
    public interface IAnkhConfigurationService
    {
        Configuration.AnkhConfig Instance
        {
            get;
        }

        void LoadConfig();

        AnkhConfig GetNewConfigInstance();

        void SaveConfig(AnkhConfig config);

        void LoadDefaultConfig();
    }
}
