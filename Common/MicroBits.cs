using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonnectUI.Common
{
    static class MicroBits
    {
        private static Dictionary<String, Guid> UUIDS = new Dictionary<string, Guid>()
        {
            {"Accelerometer Service", new Guid("E95D0753251D470AA062FA1922DFA9A8") },
            { "Accelerometer Data", new Guid("E95DCA4B251D470AA062FA1922DFA9A8")},
            {"Accelerometer Period", new Guid("E95DFB24251D470AA062FA1922DFA9A8")},
            {"Magnetometer Service", new Guid("E95DF2D8251D470AA062FA1922DFA9A8") },
            {"Magnetometer Data", new Guid("E95DFB11251D470AA062FA1922DFA9A8")},
            {"Magnetometer Period", new Guid("E95D386C251D470AA062FA1922DFA9A8")},
            {"Magnetometer Bearing", new Guid("E95D9715251D470AA062FA1922DFA9A8")},
            { "Button Service", new Guid("E95D9882251D470AA062FA1922DFA9A8")},
            {"Button A State", new Guid("E95DDA90251D470AA062FA1922DFA9A8")},
            {"Button B State", new Guid("E95DDA91251D470AA062FA1922DFA9A8")},
            {"Event Service", new Guid("E95D93AF251D470AA062FA1922DFA9A8") },
            {"Temperature Service", new Guid("E95D6100251D470AA062FA1922DFA9A8") },
            {"Temperature Data", new Guid("E95D9250251D470AA062FA1922DFA9A8")},
            {"Temperature Period", new Guid("E95D1B25251D470AA062FA1922DFA9A8")}
        };

        public static string UUIDToName (Guid guid)
        {
            var foundedService = UUIDS.FirstOrDefault(t => t.Value.Equals(guid));
            return foundedService.Key;
        }

        public static Guid NameToUUID(String name)
        {
            var founded = UUIDS.FirstOrDefault(t => t.Key.Equals(name));
            return founded.Value;
        }


    }
}
