using System;
using System.IO;
using System.Linq;

namespace RPCS3_NoDiag {
    class Program {
        static byte[] Unks = new byte[] {
            0x84, 0x85, 0xF6, 0xF7, 0x80, 0xA3, 0xA4, 0xA7,

            0xDB, 0xE4
        };
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

            for (uint i = 10; i < EXE.LongLength; i++) {
                if (EXE[i] == 0x85) {
                    if (EqualsAt(EXE, new byte[] { 0x85, 0xFF, 0x0F, 0x84, 0xFF, 0x01, 0x00, 0x00, 0x8B }, i)) {
                        if (!EqualsAt(EXE, new byte[] { 0x48 }, i - 8) && !EqualsAt(EXE, new byte[] { 0x49 }, i - 9))
                            continue;
                        Patchs++;
                        Console.WriteLine("[X1] Patching At {0:X8}", i + 3);
                        EXE[i + 3] = 0x85;
                        break;
                    }
                } else if (EXE[i] == 0x84) {
                    if (EqualsAt(EXE, new byte[] { 0x84, 0xFF, 0x0F, 0x84, 0xFF, 0x01, 0x00, 0x00, 0x8B }, i)) {
                        if (!EqualsAt(EXE, new byte[] { 0x48 }, i - 8) && !EqualsAt(EXE, new byte[] { 0x49 }, i - 9))
                            continue;
                        Patchs++;
                        Console.WriteLine("[X2] Patching At {0:X8}", i + 3);
                        EXE[i + 3] = 0x85;
                        break;
                    }
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
            for (uint i = 0; i < DataToCompare.LongLength; i++) {
                if (DataToCompare[i] == 0xFF && Unks.Contains(Data[i + At]))
                    continue;
                if (Data[i + At] != DataToCompare[i])
                    return false;
            }
            return true;
        }
    }
}
