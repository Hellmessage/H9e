using System;

namespace H9e.Core {
    public static class H9eTry {
        public delegate T ExceptionEventDelegate<T>(Exception exception);
        public delegate void ExceptionEventDelegate(Exception exception);

        public static void Run(Action action) {
            try {
                action.Invoke();
            } catch (Exception) { 
                
            }
        }

        public static void Run(Action action, Action @catch, Action @finally) {
            try {
                action.Invoke();
            } catch (Exception) {
                @catch.Invoke();
            } finally {
                @finally?.Invoke();
            }
        }

        public static void Run(Action action, ExceptionEventDelegate @catch = null, Action @finally = null) {
            try {
                action.Invoke();
            } catch (Exception ex) {
                @catch?.Invoke(ex);
            } finally {
                @finally?.Invoke();
            }
        }

        public static T Run<T>(Func<T> func, ExceptionEventDelegate<T> @catch = null, Action @finally = null) {
            try {
                return func.Invoke();
            } catch (Exception ex) {
                if (@catch != null) {
                    return @catch.Invoke(ex);
                }
                return default;
            } finally {
                @finally?.Invoke();
            }
        }

        public static T Run<T>(Func<T> func, T @default) {
            try {
                return func.Invoke();
            } catch (Exception) {
                return @default;
            }
        }


    }
}
