using System;
using NUnit.Framework;
using MainPart.Files;
using Moq;
using System.Collections.Generic;

[TestFixture]
class ClassForTest1Test
{
    private ClassForTest1 _classForTest1;
    [SetUp]
    public void SetUp()
    {
        _classForTest1 = new ClassForTest1();
    }
}