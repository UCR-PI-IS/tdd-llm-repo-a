namespace TestA { public record class R(int X, int Y); }
namespace TestB {
    public record class R(int X, int Y);
    public class S { public void M(TestA.R r) {} }
    public class T {
        public void Run() {
            var local = new R(1, 2);
            var s = new S();
            s.M(local);
        }
    }
}
