using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RPCS3_NoDiag {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Drag&Drop the RPCS3 Executable");
                Console.ReadKey();
                return;
            }
            byte[] EXE = File.ReadAllBytes(args[0]);
            if (!File.Exists(args[0] + ".bak"))
                File.Copy(args[0], args[0] + ".bak");
            File.Delete(args[0]);
            uint Patchs = 0;

            for (uint i = 0; i < EXE.LongLength; i++) {
                if (EqualsAt(EXE, new byte[] { 0x85, 0xDB, 0x0F, 0x84, 0x85, 0x01, 0x00, 0x00, 0x8B }, i)) {
                    Patchs++;
                    Console.WriteLine("Patching At {0:X8}", i+3);
                    EXE[i + 3] = 0x85;
                }
            }

            if (Patchs != 1) {
                Console.WriteLine("Failed to Patch...");
            }

            Console.WriteLine("Press a Key to Exit.");

            File.WriteAllBytes(args[0], EXE);
            Console.ReadKey();
        }

        private static bool EqualsAt(byte[] Data, byte[] DataToCompare, uint At) {
            if (DataToCompare.Length + At >= Data.Length)
                return false;
            for (uint i = 0; i < DataToCompare.LongLength; i++)
                if (Data[i + At] != DataToCompare[i])
                    return false;
            return true;
        }
    }
}
