using Spa.Core.Extensions;
using Spa.Core.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Spa.UnitTests
{
    public class TranslationAudiServiceTests
    {
        [Fact]
        public void ShouldHaveAFailure()
        {
            var expected = 1;

            List<string> contents = new List<string>()
            {
                "<p>",
                "Hello".Indent(1),
                "</p>"
            };

            var sut = new TranslationAuditService();

            var result = sut.Audit(String.Join("\n", contents));

            Assert.Equal(expected, result.Count);
        }

        [Fact]
        public void ShouldHaveMultipleFailtures()
        {
            var expected = 2;

            List<string> contents = new List<string>()
            {

                "<div>",
                "<p>",
                "Hello".Indent(1),
                "</p>",
                "<p>",
                "Hello".Indent(1),
                "</p>",
                "</div>"
            };

            var sut = new TranslationAuditService();

            var result = sut.Audit(String.Join("\n", contents));

            Assert.Equal(expected, result.Count);
        }

        [Fact]
        public void ShouldHaveMultipleNestedFailtures()
        {
            var expected = 3;

            List<string> contents = new List<string>()
            {

                "<div>",
                "<p>",
                "Hello".Indent(1),
                "<span>fail<span>",
                "</p>",
                "<p>",
                "Hello".Indent(1),
                "</p>",
                "</div>"
            };

            var sut = new TranslationAuditService();

            var result = sut.Audit(String.Join("\n", contents));

            Assert.Equal(expected, result.Count);
        }

        [Fact]
        public void ShouldHavethreeFailures()
        {
            var expected = 3;

            var sut = new TranslationAuditService();

            var result = sut.AuditDirectory(@"");

            var total = 0;

            foreach(var fileResult in result)
            {
                total = total + fileResult.Value.Count;
            }

            Assert.Equal(expected, result.Count);
        }

        [Fact]
        public void ShouldNoFailures()
        {
            var expected = 0;

            List<string> contents = new List<string>()
            {

                "<div>",
                "<router-outlet></router-outlet>".Indent(1),
                "</div>"
            };

            var sut = new TranslationAuditService();

            var result = sut.Audit(String.Join("\n", contents));

            Assert.Equal(expected, result.Count);
        }
    }
}
