﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radoslav.Serialization;

namespace Radoslav.TaskProcessor.UnitTests
{
    [TestClass]
    public sealed class RedisTaskRepositoryXmlSerializerUnitTests : RedisTaskRepositoryUnitTestsBase
    {
        protected override IEntityBinarySerializer CreateSerializer()
        {
            return new EntityBinaryXmlSerializer();
        }
    }
}