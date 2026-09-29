using System.Threading.Tasks;

public interface ITest
{
    Task DoAsync();
}

public class TestImpl : ITest
{
    Task ITest.DoAsync()
    {
        return Task.CompletedTask;
    }

    public async Task<int> DoAsync()
    {
        return 1;
    }
}

class Program
{
    static void Main() {}
}
