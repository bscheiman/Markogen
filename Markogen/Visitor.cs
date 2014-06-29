#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ClariusLabs.NuDoc;

#endregion

namespace Markogen {
    internal class Visitor : ClariusLabs.NuDoc.Visitor {
        public string CurrentClass { get; set; }
        public bool FirstMember { get; set; }
        public bool FirstParam { get; set; }
        public List<string> Methods { get; set; }
        public StringBuilder Output { get; set; }

        public Visitor() {
            Output = new StringBuilder();
            FirstMember = true;

            Methods = new List<string>();
        }

        private string NormalizeLink(string cref) {
            return cref.Replace(":", "-").Replace("(", "-").Replace(")", "");
        }

        public void ReplaceMethods() {
            if (Methods.Count <= 0)
                return;

            var tmp = Output.ToString();
            var list = string.Join(Environment.NewLine, Methods.Select(a => string.Format(" - [{0}](#{0})", a)));

            tmp = tmp.Replace("$METHODS$", string.Format("{0}{1}{1}<br/>", list, Environment.NewLine));
            Output = new StringBuilder(tmp);

            Methods = new List<string>();
        }

        public override void VisitAssembly(AssemblyMembers assembly) {
            Output.WriteLine("# Assembly");
            Output.WriteLine(assembly.Assembly.FullName);
            Output.WriteLine();

            base.VisitAssembly(assembly);
        }

        public override void VisitC(C code) {
            Output.Write(" `{0}` ", code.Content);

            base.VisitC(code);
        }

        public override void VisitClass(Class type) {
            CurrentClass = type.Id.Substring(type.Id.IndexOf(':') + 1);
            Output.WriteLine("## {0} <a id='{0}'></a>", CurrentClass);

            FirstMember = true;
            base.VisitClass(type);
        }

        public override void VisitCode(Code code) {
            Output.WriteLine();
            Output.WriteLine();

            foreach (var line in code.Content.Split(new[] {
                Environment.NewLine
            }, StringSplitOptions.None)) {
                Output.Write("    ");
                Output.WriteLine(line);
            }

            Output.WriteLine();

            base.VisitCode(code);
        }

        public override void VisitMember(Member member) {
            Output.WriteLine();

            if (!FirstMember) {
                Output.WriteLine();
                Output.WriteLine(new string('-', 50));
            }

            var name = member.Id.Substring(member.Id.IndexOf(':') + 1).Replace(string.Format("{0}.", CurrentClass), "");
            name = name.Replace("#ctor", "Constructor");
            name = Regex.Replace(name, @"\(.*?\)", "");

            if (name != CurrentClass) {
                Output.WriteLine("### {0} <a id='{0}'></a>", name);
                Methods.Add(name);
            }

            FirstParam = true;

            base.VisitMember(member);

            FirstMember = false;
        }

        public override void VisitPara(Para para) {
            Output.WriteLine();
            Output.WriteLine();
            base.VisitPara(para);
            Output.WriteLine();
            Output.WriteLine();
        }

        public override void VisitParam(Param param) {
            if (FirstParam) {
                Output.WriteLine();
                Output.WriteLine("#### Parameters");
            }

            Output.WriteLine(" - **{0}**: {1}<br/>", param.Name, param.ToText());

            base.VisitParam(param);

            FirstParam = false;
        }

        public override void VisitRemarks(Remarks remarks) {
            Output.WriteLine();
            Output.WriteLine("#### Remarks");

            base.VisitRemarks(remarks);
        }

        public override void VisitReturns(Returns returns) {
            Output.WriteLine();
            Output.WriteLine("#### Returns");
            Output.WriteLine(returns.ToText());
        }

        public override void VisitSee(See see) {
            var cref = NormalizeLink(see.Cref);

            Output.Write(" [{0}]({1}) ", cref.Substring(2), cref);
        }

        public override void VisitSeeAlso(SeeAlso seeAlso) {
            var cref = NormalizeLink(seeAlso.Cref);

            Output.WriteLine("[{0}]({1})", cref.Substring(2), cref);
        }

        public override void VisitSummary(Summary summary) {
            Output.WriteLine(summary.ToText());

            if (FirstMember) {
                ReplaceMethods();

                Output.WriteLine();
                Output.WriteLine("$METHODS$");
            }

            base.VisitSummary(summary);
        }
    }
}