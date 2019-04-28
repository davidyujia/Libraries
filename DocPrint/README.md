# DocPrint

```cs
class Program
{
    static void Main(string[] args)
    {
        var printer = new Printer("");

        var strBytes = Encoding.UTF8.GetBytes("Test String.");

        printer.Print(strBytes);
    }
}
```