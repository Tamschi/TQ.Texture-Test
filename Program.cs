using DDS;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TQ.Texture_Test
{
    static class Program
    {
        static void Main(string[] args)
        {
            Span<byte> file = File.ReadAllBytes("texture.tex");
            var texture = new Texture.Texture(file);
            Console.WriteLine($"File format version: {texture.Version}");
            Console.WriteLine($"FPS: {texture.FPS}");
            foreach (var frame in texture)
            {
                Console.WriteLine("=== Frame ===");
                var dds = new DDS.DDS(frame.Data);
                Console.WriteLine($"Variant: {dds.Variant}");
                Console.WriteLine($"Header:");
                Console.WriteLine($"  Header size: {dds.Header.Size} (Expected: {Marshal.SizeOf<Header>()})");
                Console.WriteLine($"  Flags: {dds.Header.Flags.ToFlagsString()}");
                Console.WriteLine($"  Height: {dds.Header.Height}");
                Console.WriteLine($"  Width: {dds.Header.Width}");
                Console.WriteLine($"  LinearSize: {dds.Header.LinearSize}");
                Console.WriteLine($"  Depth: {dds.Header.Depth}");
                Console.WriteLine($"  MipmapCount: {dds.Header.MipmapCount}");
                Console.WriteLine($"  PixelFormat:");
                Console.WriteLine($"    Size: {dds.Header.PixelFormat.Size} (Expected: {Marshal.SizeOf<PixelFormat>()})");
                Console.WriteLine($"    Flags: {dds.Header.PixelFormat.Flags.ToFlagsString()}");
                Console.WriteLine($"    FourCC: {dds.Header.PixelFormat.FourCC}");
                Console.WriteLine($"    RgbBitCount: {dds.Header.PixelFormat.RgbBitCount}");
                Console.WriteLine($"    RBitMask: {dds.Header.PixelFormat.RBitMask}");
                Console.WriteLine($"    GBitMask: {dds.Header.PixelFormat.GBitMask}");
                Console.WriteLine($"    BBitMask: {dds.Header.PixelFormat.BBitMask}");
                Console.WriteLine($"    ABitMask: {dds.Header.PixelFormat.ABitMask}");
                Console.WriteLine($"  Capabilities: {dds.Header.Capabilities}");
                foreach (var layer in dds)
                {
                    Console.WriteLine($"--- Layer ---");
                    foreach (var mip in layer)
                    {
                        Console.WriteLine($"Mipmap: [ {mip.Data.Length} bytes ]");
                    }
                }
            }
        }

        static string ToFlagsString<T>(this T flags) where T : unmanaged, Enum
        {
            var result = new StringBuilder();
            var seen = default(T);
            foreach (var value in (T[])Enum.GetValues(typeof(T)))
            {
                if (flags.HasFlag(value))
                {
                    if (result.Length != 0) result.Append(" | ");
                    result.Append(Enum.GetName(typeof(T), value));
                    var seenData = seen.ViewData();
                    var valueCopy = value;
                    var valueCopyData = valueCopy.ViewData();
                    for (int i = 0; i < valueCopyData.Length; i++)
                    { seenData[i] |= valueCopyData[i]; }
                }
            }
            {
                var seenData = seen.ViewData();
                var flagsData = flags.ViewData();
                bool anyUnseen = false;
                for (int i = 0; i < seenData.Length; i++)
                {
                    flagsData[i] ^= seenData[i];
                    if (flagsData[i] != default)
                    { anyUnseen = true; }
                }
                if (anyUnseen)
                {
                    if (result.Length != 0) result.Append(" | ");
                    result.Append(flags);
                }
            }
            return result.Length > 0 ? result.ToString() : "[ None ]";
        }

        static Span<byte> ViewData<T>(this ref T value) where T : unmanaged
        => MemoryMarshal.Cast<T, byte>(MemoryMarshal.CreateSpan(ref value, 1));
    }
}
