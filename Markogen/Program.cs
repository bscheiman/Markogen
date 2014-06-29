#region
using System;
using System.IO;
using System.Reflection;
using ClariusLabs.NuDoc;

#endregion

namespace Markogen {
    internal class Program {
        private static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("Please specify input assembly (*.dll).");

                return;
            }

            var asm = args[0];
            var xmlDoc = asm.Replace(".dll", ".xml");

            if (!File.Exists(asm)) {
                Console.WriteLine("Invalid assembly: {0}", asm);

                return;
            }

            if (!File.Exists(xmlDoc)) {
                Console.WriteLine("Invalid XML documentation file: {0}", xmlDoc);

                return;
            }

            var realAssembly = Assembly.LoadFile(asm);
            var doc = DocReader.Read(realAssembly, xmlDoc);
            var visitor = new Visitor();

            doc.Accept(visitor);
            visitor.ReplaceMethods();

            Console.WriteLine(visitor.Output.ToString());
        }
    }
}