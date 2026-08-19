namespace AScript.MewUIEditor;

internal static class SampleText
{
    public static readonly string[] Scripts =
        [
        """
        int sum(int a, int b) => a+b;
        int mul(int a, int b)
        {
            return a*b;
        }
        int fib(int n)
        {
            if (n <= 1) return n;
            return fib(n - 1) + fib(n - 2);
        }
        int x=6;
        fib(sum(mul(x, 3), 5)); //28657
        """
        ,
        """
        function sum(a,b) {
            return a+b;
        }
        sum(10,20)
        """
        ,
        """
        def sum(a,b):
            return a+b
        sum(10,20)
        """
        ,
        """
        function factorial(n)
            if n <= 1 then
                return 1
            end
                return n * factorial(n - 1)
        end
        factorial(5)
        """
        ,
        """
        select Name, Age from list where Age=10
        """
        ];
}
