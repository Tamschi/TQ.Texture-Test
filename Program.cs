using System;
using System.IO;

namespace TQ.Texture_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Span<byte> file = File.ReadAllBytes("texture.tex");
            var texture = new Texture.Texture(file);
            Console.WriteLine($"File format version: {texture.Version}");
            Console.WriteLine($"FPS: {texture.FPS}");
            foreach (var frame in texture)
            { Console.WriteLine("Frame"); }
        }
    }
}
