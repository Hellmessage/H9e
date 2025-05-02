using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace H9e.Core {
    public class H9eTask {

        public static (bool status, T result) RunWaitForTimeout<T>(Func<T> action, TimeSpan time) {
            Task<T> task = Task.Run(() => {
                return action.Invoke();
            });
            if (Task.WhenAny(task, Task.Delay(time)).Result == task) {
                return (true, task.Result);
            } else {
                return (false, default);
            }
        }

        public static bool RunWaitForTimeout(Action action, TimeSpan time) {
            Task task = Task.Run(() => {
                action.Invoke();
            });
            return Task.WhenAny(task, Task.Delay(time)).Result == task;
        }

    }
}
