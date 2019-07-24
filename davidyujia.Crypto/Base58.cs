namespace davidyujia.Crypto
{
    public sealed class Base58 : BaseCode
    {
        protected override char[] GetCodeTable()
        {
            return new char[] {
                '1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','J',
            'K','L','M','N','P','Q','R','S','T',
            'U','V','W','X','Y','Z','a','b','c',
            'd','e','f','g','h','i','j','k','m',
            'n','o','p','q','r','s','t','u','v',
            'w','x','y','z' };
        }
    }
}