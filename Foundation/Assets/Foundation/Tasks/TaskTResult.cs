using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Foundation.Tasks
{
    /// <summary>
    /// A task encapsulates future work that may be waited on.
    /// - Support running actions in background threads 
    /// - Supports running coroutines with return results
    /// - Use the WaitForRoutine method to wait for the task in a coroutine
    /// </summary>
    /// <example>
    /// <code>
    ///     var task = Task.Run(() =>
    ///     {
    ///        //Debug.Log does not work in
    ///        Debug.Log("Sleeping...");
    ///        Task.Delay(2000);
    ///        Debug.Log("Slept");
    ///    });
    ///    // wait for it
    ///    yield return StartCoroutine(task.WaitRoutine());
    ///
    ///    // check exceptions
    ///    if(task.IsFaulted)
    ///        Debug.LogException(task.Exception)
    ///</code>
    ///</example>
    public class UnityTask<TResult> : UnityTask
    {
        #region public fields

        /// <summary>
        /// get the result of the task. Blocking. It is recommended you yield on the wait before accessing this value
        /// </summary>
        public TResult Result;
        #endregion

        #region ctor

        Func<TResult> _function;

        public UnityTask()
        {

        }

        /// <summary>
        /// Returns the task in the Success state.
        /// </summary>
        /// <param name="result"></param>
        public UnityTask(TResult result)
            : this()
        {
            Status = TaskStatus.Success;
            Strategy = TaskStrategy.Custom;
            Result = result;
        }

        /// <summary>
        /// Creates a new background Task strategy
        /// </summary>
        /// <param name="function"></param>
        public UnityTask(Func<TResult> function)
            : this()
        {
            if (function == null)
                throw new ArgumentNullException("function");

            _function = function;
        }

        /// <summary>
        /// Creates a new task with a specific strategy
        /// </summary>
        public UnityTask(Func<TResult> function, TaskStrategy mode)
            : this()
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (mode == TaskStrategy.Coroutine)
                throw new ArgumentException("Mode can not be coroutine");

            _function = function;
            Strategy = mode;
        }

        /// <summary>
        /// Creates a new Coroutine  task
        /// </summary>
        public UnityTask(IEnumerator routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");


            _routine = routine;
            Strategy = TaskStrategy.Coroutine;
        }

        /// <summary>
        /// Creates a new Task in a Faulted state
        /// </summary>
        /// <param name="ex"></param>
        public UnityTask(Exception ex)
        {
            Exception = ex;
            Strategy = TaskStrategy.Custom;
            Status = TaskStatus.Faulted;
        }

        /// <summary>
        /// Creates a new task
        /// </summary>
        public UnityTask(TaskStrategy mode)
            : this()
        {
            Strategy = mode;
        }
        #endregion

        #region protected methods


        protected override void Execute()
        {
            try
            {
                if (_function != null)
                {
                    Result = _function();
                }
                Status = TaskStatus.Success;
                OnTaskComplete();
            }
            catch (Exception ex)
            {
                Exception = ex;
                Status = TaskStatus.Faulted;
                if (LogErrors)
                    Debug.LogException(ex);
            }
        }
        #endregion
    }
}