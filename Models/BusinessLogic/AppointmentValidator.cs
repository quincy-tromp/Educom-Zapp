﻿using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Zapp.Data;

namespace Zapp.Models.BusinessLogic
{
	public static class AppointmentValidator
	{
        /// <summary>
        /// Validates the AppointmentViewModel and adds ModelError if model is invalid
        /// </summary>
        /// <param name="model">The AppointmentViewModel to validate</param>
        /// <param name="modelState">The controller modelState in which to add errors </param>
        /// <param name="context">The ApplicationDbContext</param>
        public static void ValidateModel(AppointmentViewModel model, ModelStateDictionary modelState, ApplicationDbContext context)
        {
            if (model.Appointment.CustomerId == 0 || model.Appointment.CustomerId == null)
            {
                modelState.AddModelError("Appointment.CustomerId", "Selecteer een klant.");
            }
            if (model.Appointment.EmployeeId == "" || model.Appointment.EmployeeId == null)
            {
                modelState.AddModelError("Appointment.EmployeeId", "Selecteer een medewerker.");
            }
            if (!IsValidDate(model.Appointment.Scheduled))
            {
                modelState.AddModelError("Appointment.Scheduled", "De gekozen datum is niet beschikbaar.");
            }
            if (!IsValidTime(model.Appointment.Scheduled))
            {
                modelState.AddModelError("Appointment.Scheduled", "De gekozen tijd is niet beschikbaar.");
            }
            if (!IsEmployeeAvailable(context, model.Appointment.EmployeeId, model.Appointment.Scheduled, null))
            {
                modelState.AddModelError("Appointment.Scheduled", "Dit medewerker is niet beschikbaar op de gekozen tijdstip.");
            }
            if (model.AppointmentTasks.Count() == 0 || model.AppointmentTasks == null)
            {
                modelState.AddModelError("AppointmentTasks", "Voeg een taak toe.");
            }
        }

        /// <summary>
        /// Checks if date is valid
        /// </summary>
        /// <param name="dateTime">DateTime to check</param>
        /// <returns>True if date is valid or False if not</returns>
        private static bool IsValidDate(DateTime dateTime)
        {
            return (IsWeekday(dateTime) && dateTime >= DateTime.Today);
        }

        /// <summary>
        /// Checks if time is valid
        /// </summary>
        /// <param name="dateTime">DateTime to check</param>
        /// <returns>True if time is valid or False if not</returns>
        private static bool IsValidTime(DateTime dateTime)
        {
            return (IsBetweenWorkHours(dateTime, 8, 17) && dateTime > DateTime.Now);
        }

        /// <summary>
        /// Check if the day is Monday to Friday (1 to 5)
        /// </summary>
        /// <param name="dateTime">The day to check</param>
        /// <returns>True if day is between Monday and Friday</returns>
        private static bool IsWeekday(DateTime dateTime)
        {
            return dateTime.DayOfWeek >= DayOfWeek.Monday && dateTime.DayOfWeek <= DayOfWeek.Friday;
        }
        /// <summary>
        /// Check if the time is between startHour (inclusive) and endHour (exclusive)
        /// </summary>
        /// <param name="dateTime">The time to check</param>
        /// <param name="startHour">The startHour</param>
        /// <param name="endHour">The endHour</param>
        /// <returns>True if time is between startHour and endHour or False if not</returns>
        private static bool IsBetweenWorkHours(DateTime dateTime, int startHour, int endHour)
        {
            int hour = dateTime.Hour;
            return hour >= startHour && hour < endHour;
        }

        /// <summary>
        /// Checks if employee is available at scheduled time
        /// </summary>
        /// <param name="context">The ApplicationDbContext</param>
        /// <param name="employeeId">The Id of the employee</param>
        /// <param name="scheduled">The time to check for</param>
        /// <param name="appointmentId">The Id of the appointment</param>
        /// <returns>True if employee is available or False if not</returns>
        private static bool IsEmployeeAvailable(ApplicationDbContext context, string employeeId, DateTime scheduled, int? appointmentId)
        {
            var employeeSchedule = GetEmployeeSchedule(context, employeeId, scheduled.Date, appointmentId);
            if (IsEmployeeScheduleFree(employeeSchedule))
            {
                return true;
            }
            return (IsScheduledTimeAvailable(employeeSchedule, scheduled, -0.5d, 2.0));
        }

        /// <summary>
        /// Gets the schedule of a specified employee for a specified date
        /// </summary>
        /// <param name="context">The ApplicationDbContext</param>
        /// <param name="employeeId">The Id of the employee</param>
        /// <param name="date">The date</param>
        /// <returns>A list of DateTime objects representing appointments</returns>
        private static List<DateTime> GetEmployeeSchedule(ApplicationDbContext context, string employeeId, DateTime date, int? appointmentId)
        {
            return (context.Appointment
                    .Where(e => e.EmployeeId == employeeId && e.Scheduled.Date == date && e.Id != appointmentId)
                    .Select(e => e.Scheduled))
                    .ToList();
        }

        /// <summary>
        /// Checks if employee schedule is empty
        /// </summary>
        /// <param name="employeeSchedule">The list of employee appointments for the day</param>
        /// <returns>True if schedule is empty or False if not</returns>
        private static bool IsEmployeeScheduleFree(List<DateTime> employeeSchedule)
        {
            return (employeeSchedule == null || employeeSchedule.Count() == 0);
        }

        /// <summary>
        /// Checks if employee is available for scheduled time
        /// </summary>
        /// <param name="employeeSchedule">The list of employee appointments for the day</param>
        /// <param name="scheduledTime">The time to check for</param>
        /// <param name="timeBefore">Time before an appointment start</param>
        /// <param name="timeAfter">Time after an appointment start</param>
        /// <returns>True if employee is available for scheduled time or False if not</returns>
        private static bool IsScheduledTimeAvailable(List<DateTime> employeeSchedule, DateTime scheduledTime, double timeBefore, double timeAfter)
        {
            foreach (var appointment in employeeSchedule)
            {
                var prepStart = appointment.AddHours(timeBefore).TimeOfDay;
                var start = appointment.TimeOfDay;
                var end = appointment.AddHours(timeAfter).TimeOfDay;

                if (scheduledTime.TimeOfDay >= prepStart && scheduledTime.TimeOfDay <= start)
                {
                    return false;
                }
                if (scheduledTime.TimeOfDay >= start && scheduledTime.TimeOfDay <= end)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

