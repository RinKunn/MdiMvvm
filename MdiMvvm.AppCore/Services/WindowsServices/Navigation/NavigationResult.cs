using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MdiMvvm.AppCore.ViewModelsBase;

namespace MdiMvvm.AppCore.Services.WindowsServices.Navigation
{
    public class NavigationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationResult"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public NavigationResult(ViewModelContext context, bool? result)
        {
            this.Context = context;
            this.Result = result;
        }

        public NavigationResult(string key, object value, bool? result)
        {
            ViewModelContext context = new ViewModelContext();
            context.AddValue(key, value);
            this.Context = context;
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationResult"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public NavigationResult(ViewModelContext context, Exception error)
        {
            this.Context = context;
            this.Error = error;
            this.Result = false;
        }

        public NavigationResult(string key, object value, Exception error)
        {
            ViewModelContext context = new ViewModelContext();
            context.AddValue(key, value);
            this.Context = context;
            this.Error = error;
            this.Result = false;
        }



        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public bool? Result { get; private set; }

        /// <summary>
        /// Gets an exception that occurred while navigating.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        public ViewModelContext Context { get; private set; }
    }
}
