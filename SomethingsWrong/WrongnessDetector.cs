using SomethingsWrong.Lib;
using System;
using System.Collections.Generic;
using System.Net;

namespace SomethingsWrong
{
    public class WrongnessDetector
    {
        private readonly IList<MonitorAction> _monitorActions;
        private readonly IList<AlertAction> _alertActions;

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
                MultiLogger.Debug("Everything's OK");
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
            catch(WebException ex)
            {
                if(!action.FailAtNetworkException)
                {
                    MultiLogger.Debug("skipping WebException for " + action.Name);
                    passed = true;
                }
                else
                {
                    wrongnessMessage = string.Format("Something's Wrong! {0} IS FAILING\n" +
                                                            "Found problem when: {1}\n" +
                                                            "details: {2}",
                                                            action.Name, action.GetActionDetails(), ex.Message);
                    passed = false;
                }
            }
            catch (Exception ex)
            {
                wrongnessMessage = string.Format("Something's Wrong! {0} IS FAILING\n" +
                                                            "Found problem when: {1}\n" +
                                                            "details: {2}",
                                                            action.Name, action.GetActionDetails(), ex.Message);
                passed = false;                
            }

            if (!passed)
            {
                if (!action.MarkedAsFailing) //first fail
                {
                    MultiLogger.MonitoredAppFail(action.Name + " started to fail");
                    action.MarkedAsFailing = true;
                    MultiLogger.MonitoredAppFail(wrongnessMessage);
                    StartAlerts(action);
                }
                MultiLogger.MonitoredAppFail(action.GetActionDetails() + " continue failing");
                return false;
            }

            if(action.MarkedAsFailing)
            {
                MultiLogger.MonitoredAppFail(action.Name + " stopped failing");
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
