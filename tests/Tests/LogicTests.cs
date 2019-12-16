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
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
        }
        
        //[Fact]
        public async Task TestFbPosting()
        {
            var repositoryMock = new Mock<ISocialRepository>();

            repositoryMock
                .Setup(o => o.GetFacebookPages(It.IsAny<int>()))
                .Returns((int id) =>
                    Task.FromResult(new List<FacebookPage>()
                    {
                        new FacebookPage
                        {
                            Token = "token"
                        }
                    } as IReadOnlyCollection<FacebookPage>));
            
            ILogger logger = new SimpleLogger();
            ISocialRepository repository = repositoryMock.Object;
            
            var service = new FacebookCrosspostService(repository, logger);

            int categoryId = 1;
            string comment = "test";
            string link = "http://example.com";
            
            await service.Send(categoryId, comment, link, ImmutableList<string>.Empty, ImmutableList<string>.Empty);
        }


        //[Fact]
        public async Task TestTwitterPosting()
        {
            ILogger logger = new SimpleLogger();
            
            var socialRepositoryMock = new Mock<ISocialRepository>();

            var service = new TwitterCrosspostService(
                socialRepositoryMock.Object,
                logger);

            var categoryId = 1;
            var comment = "test";
            var link = "http://example.com";

            await service.Send(categoryId, comment, link, ImmutableList<string>.Empty, ImmutableList<string>.Empty);
        }

    }
}