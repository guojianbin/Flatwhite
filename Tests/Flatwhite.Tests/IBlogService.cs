﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Flatwhite.Tests
{
    [TestHandleException]
    [OutputCache(Duration = 50)]
    public interface IBlogService
    {
        object GetById(Guid postId);
        
        Task<object> GetByIdAsync(Guid userId);

        [TestMethodFilter]
        [TestMethod2Filter]
        Task<string> IOAsync();


        [TestMethodFilter]
        [TestMethod2Filter]
        string IOSync();

        [NoCache] // This should be applied if EnableInterfaceInterceptors
        IEnumerable<object> GetComments(Guid postId, int count);
    }

    public class BlogService : IBlogService
    {
        [OutputCache(Duration = 2)]
        public virtual object GetById(Guid postId)
        {
            InvokeCount++;
            return new {};
        }

        public async Task<object> GetByIdAsync(Guid userId)
        {
            await Task.Delay(1000);
            InvokeCount++;
            return new { };
        }
        
        public Task<string> IOAsync()
        {
            //throw new Exception("");
            InvokeCount++;
            throw new Exception();
            return Task.FromResult("Hello");
            //Console.WriteLine($"{DateTime.Now} {nameof(BlogService)} IOAsync");
            //return await new WebClient().DownloadStringTaskAsync(new Uri("https://www.nuget.org/packages/Flatwhite"));
        }

        public string IOSync()
        {
            InvokeCount++;
            throw new Exception();
        }

        [OutputCache(Duration = 2)]  // This should be applied if EnableClassInterceptors
        public virtual IEnumerable<object> GetComments(Guid postId, int count)
        {
            InvokeCount++;
            yield break;
        }

        public int InvokeCount { get; private set; }
    }
}