using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SomethingsWrong.Lib;

namespace SomethingsWrong.UnitTests
{
    [TestClass]
    public class CalendarTests
    {
        [TestMethod]
        public void ShouldDetectHolidayDayAsFree()
        {
            var dateTime = new DateTime(2014, 6, 8, 11, 12, 13);
            bool result = Calendar.IsHoliday(dateTime);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldDetectNormalDayAsNonFree()
        {
            var dateTime = new DateTime(2014, 6, 9, 12, 13, 14);
            bool result = Calendar.IsHoliday(dateTime);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldDetectWeekendDayAsFree()
        {
            var dateTime = new DateTime(2014, 1, 11, 1, 2, 3);
            bool result = Calendar.IsHoliday(dateTime);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldDetectTimeAsInsideWorkingHours()
        {
            var time = new DateTime(2014, 1, 10, 8, 31 , 0);
            bool result = Calendar.TimeIsInsideWorkingHours(time);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldDetectAfterNoonTimeAsInsideWorkingHours()
        {
            var time = new DateTime(2014, 1, 10, 16, 31, 0);
            bool result = Calendar.TimeIsInsideWorkingHours(time);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldDetectTimeAsOutsideWorkingHours()
        {
            var time = new DateTime(2014, 1, 10, 17, 31, 0);
            bool result = Calendar.TimeIsInsideWorkingHours(time);
            Assert.IsFalse(result);
        }
    }
}
