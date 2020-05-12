using System;
using Akka.Configuration;
using BLUECATS.ToastNotifier;
using FluentAssertions;
using Xunit;

namespace ToastNotifier.Tests
{
    public class ValidationAuthoritySpec
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Should_Pass(int level)
        {
            // arrange
            var config = ConfigurationFactory.ParseString($"ui.notification.authority-level={level}");
            var validationAuthority = typeof(App).GetMethod("ValidationAuthority", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Action act = () => validationAuthority.Invoke(null, new []{config});

            // act, assert
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        public void Should_Throw_Exception(int level)
        {
            // arrange
            var config = ConfigurationFactory.ParseString($"ui.notification.authority-level={level}");
            var validationAuthority = typeof(App).GetMethod("ValidationAuthority", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Action act = () => validationAuthority.Invoke(null, new[] { config });

            // act, assert
            act.Should().Throw<Exception>()
                .WithInnerException<Exception>()
                .WithMessage("authority-level은 1~5까지 지정할 수 있습니다.");
        }
    }
}
