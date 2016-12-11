using System;

namespace SqlJuxt.TestDatabase
{
    /// <summary>
    /// A general implementation of the disposable pattern. 
    /// See http://coding.abel.nu/2012/01/disposable
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        /// Implementation of IDisposable.Dispose method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Is this instance disposed?
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Dispose worker method. See http://coding.abel.nu/2012/01/disposable
        /// </summary>
        /// <param name="disposing">Are we disposing? 
        /// Otherwise we're finalizing.</param>
        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~Disposable()
        {
            Dispose(false);
        }
    }
}