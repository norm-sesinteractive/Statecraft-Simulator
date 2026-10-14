using System;
using NUnit.Framework;
using Statecraft.Sim.Core;

namespace Statecraft.Sim.Tests
{
    /// <summary>Calendar math: ticks map to years and months.</summary>
    public class SimDateTests
    {
        [Test]
        public void FirstTickIsYearOneMonthOne()
        {
            var date = new SimDate(0);
            Assert.That(date.Year, Is.EqualTo(1));
            Assert.That(date.Month, Is.EqualTo(1));
        }

        [TestCase(11L, 1, 12)]
        [TestCase(12L, 2, 1)]
        [TestCase(13L, 2, 2)]
        [TestCase(1199L, 100, 12)]
        public void TickMapsToYearAndMonth(long tick, int year, int month)
        {
            var date = new SimDate(tick);
            Assert.That(date.Year, Is.EqualTo(year));
            Assert.That(date.Month, Is.EqualTo(month));
        }

        [Test]
        public void FromYearMonthRoundTrips()
        {
            for (int year = 1; year <= 5; year++)
            for (int month = 1; month <= 12; month++)
            {
                var date = SimDate.FromYearMonth(year, month);
                Assert.That(date.Year, Is.EqualTo(year));
                Assert.That(date.Month, Is.EqualTo(month));
            }
        }

        [Test]
        public void NextAdvancesOneMonth()
        {
            Assert.That(SimDate.FromYearMonth(3, 12).Next, Is.EqualTo(SimDate.FromYearMonth(4, 1)));
        }

        [Test]
        public void NegativeTickIsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SimDate(-1));
        }
    }
}
