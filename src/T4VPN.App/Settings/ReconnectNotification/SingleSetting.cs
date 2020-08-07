using System.Collections.Generic;
using T4VPN.Core.Settings;

namespace T4VPN.Settings.ReconnectNotification
{
    public class SingleSetting : Setting
    {
        public SingleSetting(string name, Setting parent, IAppSettings settings) : base(name, parent, settings)
        {
        }

        public override void Add(Setting s)
        {
            throw new System.NotImplementedException();
        }

        public override List<Setting> GetChildren()
        {
            return new List<Setting>();
        }
    }
}
