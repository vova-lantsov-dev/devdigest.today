using System;
using System.Collections.Generic;
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
            
            var manager = new FacebookCrosspostService(repository, logger);

            int categoryId = 1;
            string comment = "test";
            string link = "http://example.com";
            
            await manager.Send(categoryId, comment, link);
        }


        //[Fact]
        public async Task TestTwitterPosting()
        {
            ILogger logger = new SimpleLogger();
            
            var publicationRepositoryMock = new Mock<IPublicationRepository>();
            var socialRepositoryMock = new Mock<ISocialRepository>();

            publicationRepositoryMock
                .Setup(o => o.GetCategoryName(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    //
                    return Task.FromResult(".NET Core");
                });
                

            var manager = new TwitterCrosspostService(
                publicationRepositoryMock.Object,
                socialRepositoryMock.Object,
                logger);

            var categoryId = 1;
            var comment = "test";
            var link = "http://example.com";

            await manager.Send(categoryId, comment, link);
        }

    }
}