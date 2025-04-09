using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace H9e.Database.Base {
    public class H9eDBPool : IDisposable {

        private readonly H9eDBConfig DBUtils;
        private int MaxConnections;
        private readonly ConcurrentQueue<DbConnection> AvailableConnections;
        private int CurrentConnectionCount;
        private readonly Timer MaintenanceTimer;
        private readonly object LockObject = Guid.NewGuid();
        private bool IsDisposed = false;
        private readonly SemaphoreSlim TaskSemaphore;

        public H9eDBPool(H9eDBConfig config, int maxConnections) {
            DBUtils = config;
            MaxConnections = maxConnections;
            AvailableConnections = new ConcurrentQueue<DbConnection>();
            CurrentConnectionCount = 0;
            MaintenanceTimer = new Timer(MaintenanceCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            TaskSemaphore = new SemaphoreSlim(maxConnections, maxConnections);
        }

        private void MaintenanceCallback(object state) {
            lock (LockObject) {
                if (IsDisposed) {
                    return;
                }
                while (AvailableConnections.TryDequeue(out var connection)) {
                    if (!DBUtils.IsConnectionValid(connection)) {
                        connection.Dispose();
                        Interlocked.Decrement(ref CurrentConnectionCount);
                    } else {
                        AvailableConnections.Enqueue(connection);
                        break;
                    }
                }

                while (CurrentConnectionCount < MaxConnections) {
                    AvailableConnections.Enqueue(DBUtils.CreateNewConnection());
                    Interlocked.Increment(ref CurrentConnectionCount);
                }
            }
        }

        public void SetMaxConnections(int maxConnections) {
            lock (LockObject) {
                if (IsDisposed) {
                    return;
                }
                MaxConnections = maxConnections;
                TaskSemaphore.Release(maxConnections - TaskSemaphore.CurrentCount);
                while (CurrentConnectionCount > MaxConnections && AvailableConnections.TryDequeue(out var connection)) {
                    connection.Dispose();
                    Interlocked.Decrement(ref CurrentConnectionCount);
                }
                while (CurrentConnectionCount < MaxConnections) {
                    AvailableConnections.Enqueue(DBUtils.CreateNewConnection());
                    Interlocked.Increment(ref CurrentConnectionCount);
                }
            }
        }

        public DbConnection GetConnection() {
            TaskSemaphore.Wait();
            lock (LockObject) {
                if (IsDisposed) {
                    TaskSemaphore.Release();
                    return null;
                }
                if (AvailableConnections.TryDequeue(out var connection)) {
                    if (DBUtils.IsConnectionValid(connection)) {
                        return connection;
                    } else {
                        connection.Dispose();
                        Interlocked.Decrement(ref CurrentConnectionCount);
                    }
                }
                if (CurrentConnectionCount < MaxConnections) {
                    var newConnection = DBUtils.CreateNewConnection();
                    Interlocked.Increment(ref CurrentConnectionCount);
                    return newConnection;
                }
            }
            return GetConnection();
        }

        public void ReleaseConnection(DbConnection connection) {
            lock (LockObject) {
                if (IsDisposed) {
                    connection.Dispose();
                    TaskSemaphore.Release();
                    return;
                }
                if (DBUtils.IsConnectionValid(connection)) {
                    AvailableConnections.Enqueue(connection);
                } else {
                    connection.Dispose();
                    Interlocked.Decrement(ref CurrentConnectionCount);
                }
                TaskSemaphore.Release();
            }
        }

        public void Execute(Action<DbConnection> action) {
            var connection = GetConnection();
            if (connection == null) {
                return;
            }
            try {
                action.Invoke(connection);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            } finally {
                ReleaseConnection(connection);
            }
        }

        public async Task ExecuteAsync(Action<DbConnection> action) {
            var connection = GetConnection();
            if (connection == null) {
                return;
            }
            await Task.Run(() => {
                try {
                    action.Invoke(connection);
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                } finally {
                    ReleaseConnection(connection);
                }
            });
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) return;
            if (disposing) {
                MaintenanceTimer.Dispose();
                IsDisposed = true;
                while (AvailableConnections.TryDequeue(out var connection)) {
                    connection.Dispose();
                }
            }
            IsDisposed = true;
        }
    }
}
