using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Logging;
using Core.Managers;
using DAL;
using Moq;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
        }
        
        //[Fact]
        public async Task TestFbPosting()
        {
            var repositoryMock = new Mock<IRepository>();

            repositoryMock.Setup(o => o.GetFacebookPages(It.IsAny<int>())).Returns((int id) => new List<FacebookPage>()
            {
                new FacebookPage
                {
                    Token = "token"
                }
            });
            
            ILogger logger = new SimpleLogger();
            IRepository repository = repositoryMock.Object;
            
            var manager = new FacebookCrosspostManager(repository, logger);

            int categoryId = 1;
            string comment = "test";
            string link = "http://example.com";
            
            await manager.Send(categoryId, comment, link);
        }
    }
}