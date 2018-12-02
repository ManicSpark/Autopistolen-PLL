using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PulsarPluginLoader;

namespace WeaponsPlus
{
    public class Plugin : PulsarPlugin
    {
        protected override string HarmonyIdentifier()
        {
            // Make this unique to your plugin!
            return "Kell.EngBot.pulsar.Weapons+";
        }
    }
}
