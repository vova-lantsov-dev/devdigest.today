using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Core;
using Core.Logging;
using Core.Services;
using Core.Services.Crosspost;
using Core.Repositories;
using DAL;
using Moq;
using Xunit;

namespace Tests
{
    public class LogicTest
    {
        [Fact]
        public void EMptyTest()
        {
            Assert.True(true);
        }
    }
}