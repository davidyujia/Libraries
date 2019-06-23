# DocPrint

```cs
class Program
{
    static void Main(string[] args)
    {
        var printer = new Printer(@"\\Server01\Prt-6B Color");

        var strBytes = Encoding.UTF8.GetBytes("Test String.");

        printer.Print(strBytes);
    }
}
```