using UnityEngine;

public class TestChild : TestBase
{
    public byte number;

    public TestChild(string _text,byte _number) : base(_text)
    {
        number = _number;
    }
}
