using System.Threading;
using SomethingsWrong.Lib;
using System;
using System.Collections.Generic;

namespace SomethingsWrong
{
    public class WrongnessDetector
    {
        private readonly IList<MonitorAction> _monitorActions;
        private readonly IList<AlertAction> _alertActions;
        private const int defaultAlertDurationInSeconds = 60;

        public WrongnessDetector(IList<MonitorAction> monitorActions, IList<AlertAction> alertActions)
        {
            _monitorActions = monitorActions;
            _alertActions = alertActions;
        }

        public void CheckAll()
        {
            bool allActionsPassed = true;
            foreach (var action in _monitorActions)
            {
                bool result = PerformMonitorAction(action);
                if(!result)
                {
                    allActionsPassed = false;
                }
            }

            if(allActionsPassed)
            {
                Console.WriteLine("Everything is OK");
                StopRunningAlerts();
            }
        }

        private bool PerformMonitorAction(MonitorAction action)
        {
            bool passed = true;
            string wrongnessMessage = "";
            try
            {
                bool result = action.GetStatus();
                if (!result)
                {
                    wrongnessMessage = string.Format("Somethings Wrong! {0} IS FAILING\n" +
                                                            "Found problem when: {1}\n",
                                                            action.Name, action.GetActionDetails());
                    passed = false;

                }

            }
            catch (Exception ex)
            {
                wrongnessMessage = string.Format("Somethings Wrong! {0} IS FAILING\n" +
                                                            "Found problem when: {1}\n" +
                                                            "details: {2}",
                                                            action.Name, action.GetActionDetails(), ex.Message);
                passed = false;
            }

            if (!passed)
            {
                if (!action.MarkedAsFailing) //first fail
                {
                    Console.WriteLine(action.Name + " started to fail");
                    action.MarkedAsFailing = true;
                    Console.WriteLine(wrongnessMessage);
                    StartAlerts(action);
                }
                Console.WriteLine(action.GetActionDetails() + " continue failing");
                return false;
            }

            if(action.MarkedAsFailing)
            {
                Console.WriteLine(action.Name + " stopped to fail");
                action.MarkedAsFailing = false;
            }            
            return true;
        }

        private void StartAlerts(MonitorAction monitorAction)
        {
            foreach(AlertAction action in _alertActions)
            {
                action.Start(monitorAction);
            }
        }

        private void StopRunningAlerts()
        {
            foreach (AlertAction action in _alertActions)
            {
                if(action.IsRunning())
                {
                    action.Stop();
                }                
            }
        }
    }
}
