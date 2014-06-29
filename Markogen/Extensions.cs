#region
using System.Text;

#endregion

namespace Markogen {
    public static class Extensions {
        public static void Write(this StringBuilder sb, string txt, params object[] args) {
            sb.AppendFormat(txt, args);
        }

        public static void WriteLine(this StringBuilder sb, string txt, params object[] args) {
            sb.AppendFormat(txt, args);
            sb.AppendLine();
        }

        public static void WriteLine(this StringBuilder sb) {
            sb.AppendLine();
        }
    }
}