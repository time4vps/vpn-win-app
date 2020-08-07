using System.Collections.Generic;

namespace T4VPN.Core
{
    public interface ILanguageProvider
    {
        List<string> GetAll();
    }
}