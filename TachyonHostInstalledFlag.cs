using TachyonCommon;

namespace TachyonClientRPC {
    public class TachyonHostInstalledFlag : ITachyonInstalledFlag {
        public string ActivationArgument => "--host";

        public TachyonHostInstalledFlag() { }
    }
}