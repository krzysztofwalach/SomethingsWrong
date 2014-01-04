using System.Threading;
using SomethingsWrong.Lib;
using System;
using System.Collections.Generic;

namespace SomethingsWrong
{
    public class WrongnessDetector
    {
        private readonly IList<MonitorAction> _monitorActions;

        public WrongnessDetector(IList<MonitorAction> monitorActions)
        {
            _monitorActions = monitorActions;
        }

        public void CheckAll()
        {
            foreach (var action in _monitorActions)
            {
                PerformAction(action);
            }
        }

        private void PerformAction(MonitorAction action)
        {
            try
            {
                bool result = action.GetStatus();
                if (!result)
                {
                    string wrongnessMessage = string.Format("Somethings Wrong! {0} IS FAILING FAILED\n" +
                                                            "Found problem when: {1}\n",
                                                            action.GetStepName(), action.GetActionDetails());
                    SaySomethingsWrong(wrongnessMessage);
                }
            }
            catch (Exception ex)
            {
                string wrongnessMessage = string.Format("Somethings Wrong! {0} IS FAILING FAILED\n" +
                                                            "Found problem when: {1}\n" +
                                                            "details: {2}",
                                                            action.GetStepName(), action.GetActionDetails(), ex.Message);
                SaySomethingsWrong(wrongnessMessage);
            }
        }

        private void SaySomethingsWrong(string message)
        {
            Console.WriteLine(message);
            Thread t = new Thread(() => RunFlashLight(new TimeSpan(0,0,0,30)));
        }

        private void RunFlashLight(TimeSpan timeSpan)
        {
            
        }


    }
}
