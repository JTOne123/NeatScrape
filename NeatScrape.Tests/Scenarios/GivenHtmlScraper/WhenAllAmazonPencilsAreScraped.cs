﻿using System.Collections.Generic;
using FluentAssertions;
using NeatScrape.Converters;
using NeatScrape.Scraping.Html;
using Xunit;

namespace NeatScrape.Tests.Scenarios.GivenHtmlScraper
{
    public class WhenAllAmazonPencilsAreScraped : GivenHtmlScraper
    {
        private ICollection<AmazonEntry> _results;

        public override void When()
        {
            var instruction = new HtmlScrapeInstruction<AmazonEntry>(config =>
            {
                var linkConverter = new LinkConverter("https://www.amazon.com");

                config
                    .ScrapeUrl("NeatScrape.Tests.Resources.amazon_pencils_page{{page}}.html")
                    .WithPaging("page", pagingStart: 1, pagingIncrement: 1, pagingEnd: 3)
                    .AsEntries(e => e.FromCssSelector(".s-item-container")
                        .MapProperty(p => p.Title, p => p.FromCssSelector("a.s-access-detail-page h2"))
                        .MapProperty(p => p.Url, p => p.FromCssSelector("a.s-access-detail-page").UsingConverter(linkConverter)));
            });

            _results = Scraper.Scrape(instruction).Result;
        }

        public class Then : IClassFixture<WhenAllAmazonPencilsAreScraped>
        {
            private readonly WhenAllAmazonPencilsAreScraped _data;

            public Then(WhenAllAmazonPencilsAreScraped data)
            {
                _data = data;
            }

            [Fact]
            public void Then90ResultsAreReturned()
            {
                _data._results.Should().HaveCount(90);
            }

            [Fact]
            public void ThenAllResultsShouldHaveTitle()
            {
                _data._results.Should().OnlyContain(x => !string.IsNullOrWhiteSpace(x.Title));
            }

            [Fact]
            public void ThenAllResultsShouldHaveUrl()
            {
                _data._results.Should().OnlyContain(x => !string.IsNullOrWhiteSpace(x.Url));
            }
        }
    }
}
