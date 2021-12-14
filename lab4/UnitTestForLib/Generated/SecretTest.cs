using System;
using NUnit.Framework;
using MainPart.Files;
using Moq;
using System.Collections.Generic;

[TestFixture]
class SecretTest
{
    private Secret _secret;
    [SetUp]
    public void SetUp()
    {
        _secret = new Secret();
    }
}